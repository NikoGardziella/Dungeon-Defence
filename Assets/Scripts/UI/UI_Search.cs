namespace DungeonDefence
{
	using DevelopersHub.RealtimeNetworking.Client;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;

	public class UI_Search : MonoBehaviour
	{

		[SerializeField] private GameObject _elements = null;
		[SerializeField] private Button _closeButton = null;
		[SerializeField] private Button _findButton = null;
		[SerializeField] private Button _findDungeonButton = null;


		private static UI_Search _instance = null; public static UI_Search instance { get { return _instance; } }
		private bool _active = true; public bool isActive { get { return _active; } }
		private long lastTarget = 0;

		private void Awake()
		{
			_instance = this;
			_elements.SetActive(false);
			
		}

		private void Start()
		{
			_closeButton.onClick.AddListener(Close);
			_findButton.onClick.AddListener(Find);
			_findDungeonButton.onClick.AddListener(FindDungeon);
		}

		public void SetStatus(bool status)
		{
			if (status)
			{
				lastTarget = 0;
			}
			_active = status;
			_elements.SetActive(status);
		}

		private void Close()
		{
			SetStatus(false);
			UI_Main.instance.SetStatus(true);
		}

		public void Find()
		{
			Packet packet = new Packet();
			packet.Write((int)Player.RequestId.BATTLEFIND);
			Sender.TCP_Send(packet);
		}
		public void FindDungeon()
		{
			Packet packet = new Packet();
			packet.Write((int)Player.RequestId.FINDDUNGEON);
			Sender.TCP_Send(packet);
		}

		public void FindResponded(long target, Data.OpponentData opponent)
		{
			if(target > 0 && opponent != null && target != lastTarget)
			{
				SetStatus(false);
				Debug.Log("opponent:"+ opponent.opponentName);
				bool attack =  UI_Battle.instance.Display(opponent.buildings, target, Data.BattleType.normal, opponent.dungeonUnits);
				if(attack)
				{
					lastTarget = target;
				}
				else
				{
					UI_Main.instance.SetStatus(true);
				}
			}
			else
			{
				UI_Battle.instance.NoTarget();
				Debug.Log("No target found.");
			}
		}
		public void FindRespondedDungeon(long target, Data.OpponentData opponent)
		{
			if(target > 0 && opponent != null && target != lastTarget)
			{
				Debug.Log("dungeon opponent:"+ opponent.opponentName );
				SetStatus(false);
			   bool attack =  UI_Battle.instance.Display(opponent.buildings, target, Data.BattleType.dungeon, opponent.dungeonUnits);
				if(attack)
				{
					lastTarget = target;
				}
				else
				{
					UI_Main.instance.SetStatus(true);
				}
			}
			else
			{
				UI_Battle.instance.NoTarget();
				Debug.Log("No target found.");
			}
		}


	}
}