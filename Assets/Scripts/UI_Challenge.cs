namespace DungeonDefence
{
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UI_Challenge : MonoBehaviour
    {
        public Data.ChallengeType challengeType = Data.ChallengeType.daily;

        public TextMeshProUGUI firstReq;
        public TextMeshProUGUI secondReq;

        [SerializeField] public int reqKills = 0;
        [SerializeField] public int reqGames = 0;
        [SerializeField] public int kills = 0;
        [SerializeField] public int games = 0;
        [SerializeField] public bool completed = false;
        [SerializeField] public bool active = false;


        
        public int challengeIndex = 0;
        [SerializeField] private long _databaseID = 1; public long databaseID  { get { return _databaseID; } set { _databaseID = value; }  }



        

    }

}