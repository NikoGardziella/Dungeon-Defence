
namespace DungeonDefence
{

    //using System.Runtime.InteropServices.WindowsRuntime;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using DevelopersHub.RealtimeNetworking.Client;
    using UnityEngine;
    using UnityEngine.Rendering.Universal;
    using UnityEngine.UI;

    public class UI_Challenges : MonoBehaviour
    {

        [SerializeField] public GameObject _elements = null;
      		[SerializeField] private Button _closeButton = null;


        [SerializeField] public GameObject _challenge = null;
        [SerializeField] public GameObject _content = null;
        private List<UI_Challenge> activeDailyChallenges = null;        
        private static UI_Challenges _instance = null; public static UI_Challenges instance {get {return _instance; } }
		[SerializeField] public UI_Challenge[] _challengePrefabs = null;

        private int MAX_CHALLENGES = 1;
        private int MAX_DAILY_CHALLENGES = 1;
        [SerializeField] public int kills = 0;
        [SerializeField] public int games = 0;

        // Start is called before the first frame update
        void Start()
        {
			_closeButton.onClick.AddListener(CloseChallenges);
        }
        private void Awake()
		{
			_instance = this;
			_elements.SetActive(false);
            activeDailyChallenges = new List<UI_Challenge>();
           // UpdateChallenges();
		}

        // Update is called once per frame
        void Update()
        {
            
        }

        public void UpdateChallenges()
        {
           // int highestIndex;
          /*  if(activeDailyChallenges.Count < MAX_DAILY_CHALLENGES)
            {
                for (int i = 0; i < MAX_DAILY_CHALLENGES; i++)
                {
                    highestIndex = GethighestChallengeIndex();
                    GetNextDailyChallenges(highestIndex);
                }
            }
            */
            GetNextDailyChallenges(0);
        }

       
        private void GetNextDailyChallenges(int CurrentHighestChallengeId)
        {
            int count = GetChallengesCount(Data.GetChallenge.ActiveNotCompleted);
            Debug.Log("getting challenges count: " + count);
            Packet packet = new Packet();
            packet.Write((int)Player.RequestId.GETCHALLENGES);
            packet.Write((int)Data.ChallengeType.daily);
            packet.Write((int)count);
			Sender.TCP_Send(packet);

        }

        // Next challenge is always the highest index of current challenges + 1

        private int GethighestChallengeIndex()
        {
            int highestIndex = 0;

            for (int i = 0; i < activeDailyChallenges.Count; i++)
            {
                if(activeDailyChallenges[i].challengeIndex > highestIndex)
                {
                    highestIndex = activeDailyChallenges[i].challengeIndex;
                }
            }
            return highestIndex; 
        }

        private int GetChallengesCount(Data.GetChallenge type)
        {
            int count = 0;
            if(type == Data.GetChallenge.ActiveNotCompleted)
            {
                for (int i = 0; i < activeDailyChallenges.Count; i++)
                {
                    if(activeDailyChallenges[i].completed != true && activeDailyChallenges[i].active == true)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        public void SyncChallengeData(List<Data.DailyChallenge> dailyChallenges)
        {
            Debug.Log("syncing challenges. count:" +  dailyChallenges.Count);
            for (int i = 0; i < dailyChallenges.Count; i++)
            {
                Debug.Log("challenge database id:" + dailyChallenges[i].database_id);
                
                UI_Challenge challenge = GetChallenge(dailyChallenges[i].database_id);
                if (challenge != null)
                {
                    Debug.Log("existing challenge found id:" + dailyChallenges[i].database_id);
                }
                else
                {
                    UI_Challenge prefab = GetChallengePrefab(dailyChallenges[i].challengeType);
                    if (prefab)
                    {
                        Debug.Log("adding challenge:" + dailyChallenges[i].database_id);
                        challenge = Instantiate(prefab, Vector3.zero, Quaternion.identity,_content.transform);
                        challenge.firstReq.text = dailyChallenges[i].kills.ToString() +"/" + dailyChallenges[i].req_kills.ToString();
                        challenge.secondReq.text = dailyChallenges[i].games.ToString() +"/" + dailyChallenges[i].req_games.ToString();
                        challenge.kills = dailyChallenges[i].kills;
                        challenge.games = dailyChallenges[i].games;
                        challenge.reqGames = dailyChallenges[i].req_games;
                        challenge.reqKills = dailyChallenges[i].req_kills;
                        challenge.databaseID = dailyChallenges[i].database_id;
                        challenge.completed = false;
                        challenge.active = true;
                        activeDailyChallenges.Add(challenge);
                    }
                    else
                    {
                        Debug.Log("prefaberror");
                    }
                }
                
            }
        }

        private UI_Challenge GetChallenge(int database_id)
        {
            if(activeDailyChallenges == null)
                return null;
            for (int i = 0; i < activeDailyChallenges.Count; i++)
            {
                if(activeDailyChallenges[i].databaseID == database_id)
                {
                    return activeDailyChallenges[i];
                }
            }
            return null;
        }
        private UI_Challenge GetChallengePrefab(Data.ChallengeType challengeType)
        {
            for (int i = 0; i < _challengePrefabs.Length; i++)
            {
                if(_challengePrefabs[i].challengeType == challengeType)
                {
                    return _challengePrefabs[i];
                }
            }
            return null;
        }


        public void SetStatus(bool status)
		{
			_elements.SetActive(status);
            if(status == true)
            {
                UpdateChallenges();
                CheckChallengeCriteria();
            }
		}

        private void CloseChallenges()
		{
			SetStatus(false);
			UI_Main.instance.SetStatus(true);
		}

        private void CheckChallengeCriteria()
        {
            for (int i = 0; i < activeDailyChallenges.Count; i++)
            {
                if(activeDailyChallenges[i].games == activeDailyChallenges[i].reqGames && activeDailyChallenges[i].reqGames != 0)
                {
                    CompleteChallenge(activeDailyChallenges[i].databaseID);
                    activeDailyChallenges[i].completed = true;

                }

                if(activeDailyChallenges[i].kills == activeDailyChallenges[i].reqKills && activeDailyChallenges[i].reqKills != 0)
                {
                    CompleteChallenge(activeDailyChallenges[i].databaseID);
                    activeDailyChallenges[i].completed = true;
                }
            }

        }

        private void ResetChallenge(long challenge)
        {
            Packet packet = new Packet();
            packet.Write((int)Player.RequestId.RESETCHALLENGE);
            packet.Write((int)Data.ChallengeType.daily);
            packet.Write((long)challenge);
			Sender.TCP_Send(packet);
        }

        private void CompleteChallenge(long challenge)
        {
            Debug.Log(challenge + " challenge completed");
            Packet packet = new Packet();
            packet.Write((int)Player.RequestId.COMPLETECHALLENGE);
            packet.Write((int)Data.ChallengeType.daily);
            packet.Write((long)challenge);
			Sender.TCP_Send(packet);
        }

        public void UpdateChallengeStats(int newKills, int newGames)
        {
            if(newGames > 0)
            {
                for (int i = 0; i < activeDailyChallenges.Count; i++)
                {
                    if(activeDailyChallenges[i].completed != true)
                    {
                        activeDailyChallenges[i].games += newGames;
                        activeDailyChallenges[i].secondReq.text = activeDailyChallenges[i].games.ToString() +"/" + activeDailyChallenges[i].reqGames.ToString();
                    }
                }
            }
            if(newKills > 0)
            {
                for (int i = 0; i < activeDailyChallenges.Count; i++)
                {
                    if(activeDailyChallenges[i].completed != true)
                    {
                        activeDailyChallenges[i].kills += newKills;
                        activeDailyChallenges[i].firstReq.text = activeDailyChallenges[i].kills.ToString() +"/" + activeDailyChallenges[i].reqKills.ToString();
                    }
                }
            }

        }


    }
}