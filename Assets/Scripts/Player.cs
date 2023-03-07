namespace DungeonDefence
{
	using System.Collections.Generic;
	using UnityEngine;
	using DevelopersHub.RealtimeNetworking.Client;
    using UnityEngine.SceneManagement;
    using System;

    public class Player : MonoBehaviour
	{

		public Data.Player data = new Data.Player();
		private static Player _instance = null; public static Player instance {get {return _instance; } }
	  	public Data.InitializationData initializationData = new Data.InitializationData();

		public enum RequestId
		{
			AUTH = 1,
			SYNC = 2,
			BUILD = 3,
			REPLACE = 4,
			COLLECT = 5,
			PREUPGRADE = 6,
			UPGRADE = 7,
			INSTANTBUILD = 8,
			TRAIN = 9,
			CANCELTRAIN = 10,
			BATTLEFIND = 11,
			BATTLESTART = 12,
			BATTLEFRAME = 13,
			BATTLEEND = 14,
			OPENCLAN = 15,
			GETCLANS = 16,
			JOINCLAN = 17,
			LEAVECLAN = 18,
			EDITCLAN = 19,
			CREATECLAN = 20,
			OPENWAR = 21,
			STARTWAR = 22,
			CANCELWAR = 23,
			WARSTARTED = 24,
			WARATTACK = 25,
			WARREPORTLIST = 26,
			WARREPORT = 27,
			JOINREQUESTS = 28,
			 JOINRESPONSE = 29,
			  GETCHATS = 30,
			   SENDCHAT = 31

		}



		private bool connected = false;
		private bool updating = false;
		private bool _inBattle = false; public static bool inBattle {get {return instance._inBattle; }  set { instance._inBattle = value;  } }

		private void Start()
		{
			RealtimeNetworking.OnPacketReceived += ReceivePacket;
			InitializeConnection();
		}
		private void Awake()
		{
			_instance = this;
		}
		

		private float syncTime = 5.0f; //how often sync player data with server

		private float timer = 0;

	  	private void Update()
        {
            if (connected)
            {
                if (!_inBattle)
                {
                    if (timer <= 0)
                    {
                        updating = true;
                        timer = syncTime;
                        SendSyncRequest();
                    }
                    else
                    {
                        timer -= Time.deltaTime;
                    }
                }
                data.nowTime = data.nowTime.AddSeconds(Time.deltaTime);
            }
        }


		private void ReceivePacket(Packet packet)
		{

			try
			{
				int id = packet.ReadInt();
			long databaseID = 0;
			int response = 0;
			switch ((RequestId)id)
			{
				case RequestId.AUTH:
					connected = true;
					updating = true;
					timer = 0;
					string authData = packet.ReadString();
					initializationData = Data.Deserialize<Data.InitializationData>(authData);
					SendSyncRequest();
					break;
				case RequestId.SYNC:
					string playerData = packet.ReadString();
					Data.Player playerSyncData = Data.Deserialize<Data.Player>(playerData);
					SyncData(playerSyncData);
					updating = false;
					break;
				case RequestId.BUILD:
					response = packet.ReadInt();
					switch (response)
					{
						case 0:
							Debug.Log("unknown error");
							break;
						case 1:
							Debug.Log("Placed Succesfully");
							RushSyncRequest();
							break;
						case 2:
							Debug.Log("no resources");
							break;
						case 3:
							Debug.Log("Max level");
							break;
						case 4:
							Debug.Log("Place taken");
							break;
						case 5:
							Debug.Log("no builders");
							break;
						case 6:
							Debug.Log("Maximum level reached");
							break;						
					}
					break;
				case RequestId.REPLACE:
					int replaceResponse = packet.ReadInt();
					int replaceX = packet.ReadInt();
					int replaceY = packet.ReadInt();
					long replaceID = packet.ReadLong();

					for (int i = 0; i < UI_Main.instance._grid.buildings.Count; i++)
					{
						if(UI_Main.instance._grid.buildings[i].databaseID == replaceID)
						{
							switch (replaceResponse)
							{
								case 0:
									Debug.Log("no building");
									break;
								case 1:
									Debug.Log("Replaced Succesfully");
									UI_Main.instance._grid.buildings[i].PlacedOnGrid(replaceX, replaceY);			
									if(UI_Main.instance._grid.buildings[i] != Building.selectedInstance)
									{
										
									}
									RushSyncRequest();
									break;
								case 2:
									Debug.Log("Place taken");
									break;
							}
							UI_Main.instance._grid.buildings[i].waitinReplaceRepsonce = false;
							break;
						}
					}
					break;
				case RequestId.COLLECT:
					long db = packet.ReadLong();
					int collectedAmmount = packet.ReadInt();
					Debug.Log("collecterd:" + collectedAmmount);

					for (int i = 0; i < UI_Main.instance._grid.buildings.Count; i++)
					{
						if(db ==  UI_Main.instance._grid.buildings[i].data.databaseID)
						{
							UI_Main.instance._grid.buildings[i].collecting = false; 
							switch (UI_Main.instance._grid.buildings[i].id)
							{
								case Data.BuildingID.goldmine:
									UI_Main.instance._grid.buildings[i].data.goldStorage -= collectedAmmount;
									break;
								case Data.BuildingID.elixirmine:
									UI_Main.instance._grid.buildings[i].data.elixirStorage -= collectedAmmount;
									break;
								case Data.BuildingID.darkelixirmine:
									UI_Main.instance._grid.buildings[i].data.darkStorage -= collectedAmmount;
									break;
							}
							//UI_Main.instance._grid.buildings[i].data.storage -= collectedAmmount;
							UI_Main.instance._grid.buildings[i].AdjustUI();
						}
					}
					break;
				case RequestId.PREUPGRADE:
					databaseID = packet.ReadLong();
					string re = packet.ReadString();
					Data.ServerBuilding ServerBuildingData = Data.Deserialize<Data.ServerBuilding>(re);
					UI_BuildingUpgrade.instance.Open(ServerBuildingData, databaseID);
					break;
				case RequestId.UPGRADE:
					response = packet.ReadInt();
					switch (response)
					{
						case 0:
							Debug.Log("unknown error");
							break;
						case 1:
							Debug.Log("Upgrade started");
							RushSyncRequest();
							break;
						case 2:
							Debug.Log("no resources");
							break;
						case 3:
							Debug.Log("Max level");
							break;
						case 5:
							Debug.Log("no builders");
							break;	
						case 6:
							Debug.Log("Maximum level reached");
							break;						
					}
					break;
				case RequestId.INSTANTBUILD:
					response = packet.ReadInt();
					if(response == 2)
					{
						Debug.Log("not enough gems for instant build");
					}
					else if(response == 1)
					{
						Debug.Log("Instant build succesfull");
						RushSyncRequest();
					}
					else
					{
						Debug.Log("Instabuild not possible");
						//UI_BuildingUpgrade.instance.Close();
					}
					break;
				case RequestId.TRAIN:
					response = packet.ReadInt();
					if(response == 2)
					{
						Debug.Log("no resources");
					}
					else if(response == 3)
					{
						Debug.Log("no capacity to train unit");
					}
					else if(response == 4)
					{
						Debug.Log("Server unit not found");
					}
					else if(response == 1)
					{
						Debug.Log("Training started");
						RushSyncRequest();
					}
					else
					{
						Debug.Log("Instabuild not possible");
						//UI_BuildingUpgrade.instance.Close();
					}
					break;

				case RequestId.CANCELTRAIN:
					response = packet.ReadInt();
					if(response == 2)
					{
						Debug.Log("not enough gems for instant build");
					}
					else if(response == 1)
					{
						Debug.Log("Instant build succesfull");
						RushSyncRequest();
					}
					else
					{
						Debug.Log("Instabuild not possible");
						//UI_BuildingUpgrade.instance.Close();
					}
					break;

				case RequestId.BATTLEFIND:
					long targetID = packet.ReadLong();
					Data.OpponentData opponent = null;
					if(targetID > 0)
					{
						string d = packet.ReadString();
						opponent = Data.Deserialize<Data.OpponentData>(d);
					}
					UI_Search.instance.FindResponded(targetID, opponent);
					break;

				case RequestId.BATTLESTART:
					bool matched = packet.ReadBool();
					bool attack = packet.ReadBool();
					bool confirmed = matched && attack;
					List<Data.BattleStartBuildingData> buildings = null;
					int winTrophies = 0;
					int loseTrophies = 0;
					if(confirmed)
					{
						winTrophies = packet.ReadInt();
						loseTrophies = packet.ReadInt();
						string BattleStartBuildingData = packet.ReadString();
						buildings = Data.Deserialize<List<Data.BattleStartBuildingData>>(BattleStartBuildingData);
					}
					UI_Battle.instance.StartBattleConfirm(confirmed, buildings, winTrophies, loseTrophies);
					break;

				case RequestId.BATTLEEND:
					int stars = packet.ReadInt();;
					int unitsDeployed = packet.ReadInt();;
					int lootedGold = packet.ReadInt();;
					int lootedElixir = packet.ReadInt();;
					int lootedDark = packet.ReadInt();;
					int trophies = packet.ReadInt();;
					int frame = packet.ReadInt();
					UI_Battle.instance.BattleEnded(stars, unitsDeployed, lootedGold, lootedElixir, lootedDark, trophies, frame);
					break;

				case RequestId.OPENCLAN:
					bool haveClan = packet.ReadBool();
					Data.Clan clan = null;
					List<Data.ClanMember> warMembers = null;
					if (haveClan)
					{
						string clanData = packet.ReadString();
						clan = Data.Deserialize<Data.Clan>(clanData);
						if(clan.war.id > 0)
						{
							string warData = packet.ReadString();
							warMembers = Data.Deserialize<List<Data.ClanMember>>(warData);
						}
					}
					UI_Clan.instance.ClanOpen(clan, warMembers);
					break;
				case RequestId.GETCLANS:
					string clansData = packet.ReadString();
					Data.ClansList clans = Data.Deserialize<Data.ClansList>(clansData);
					UI_Clan.instance.ClansListOpen(clans);
					break;
				case RequestId.CREATECLAN:
					response = packet.ReadInt();
					UI_Clan.instance.CreateResponse(response);
					break;
				case RequestId.JOINCLAN:
					response = packet.ReadInt();
					UI_Clan.instance.JoinResponse(response);
					break;
				case RequestId.LEAVECLAN:
					response = packet.ReadInt();
					UI_Clan.instance.LeaveResponse(response);
					break;
				case RequestId.EDITCLAN:
					response = packet.ReadInt();
					UI_Clan.instance.EditResponse(response);
					break;
				 case RequestId.OPENWAR:
					string clanWarData = packet.ReadString();
					Data.ClanWarData war = Data.Deserialize<Data.ClanWarData>(clanWarData);
					UI_Clan.instance.WarOpen(war);
					break;
				case RequestId.STARTWAR:
					response = packet.ReadInt();
					UI_Clan.instance.WarStartResponse(response);
					break;
				case RequestId.CANCELWAR:
					response = packet.ReadInt();
					UI_Clan.instance.WarSearchCancelResponse(response);
					break;
				case RequestId.WARSTARTED:
					databaseID = packet.ReadInt();
					UI_Clan.instance.WarStarted(databaseID);
					break;
				case RequestId.WARATTACK:
					databaseID = packet.ReadLong();
					Data.OpponentData warOpponent = null;
					if(databaseID > 0)
					{
						string s = packet.ReadString();
						warOpponent = Data.Deserialize<Data.OpponentData>(s);
					}
					UI_Clan.instance.AttackResponse(databaseID, warOpponent);
					break;
				case RequestId.WARREPORTLIST:
					string warReportsData = packet.ReadString();
					List<Data.ClanWarData> warReports = Data.Deserialize<List<Data.ClanWarData>>(warReportsData);
					UI_Clan.instance.OpenWarHistoryList(warReports);
					break;
				case RequestId.WARREPORT:
					bool hasReport = packet.ReadBool();
					Data.ClanWarData warReport = null;
					if (hasReport)
					{
						string warReportData = packet.ReadString();
						warReport = Data.Deserialize<Data.ClanWarData>(warReportData);
					}
					UI_Clan.instance.WarOpen(warReport, true);
					break;
				case RequestId.JOINREQUESTS:
					string requstsData = packet.ReadString();
					List<Data.JoinRequest> requests = Data.Deserialize<List<Data.JoinRequest>>(requstsData);
					UI_Clan.instance.OpenRequestsList(requests);
					break;
				case RequestId.JOINRESPONSE:
					response = packet.ReadInt();
					if(UI_ClanJoinRequest.active != null)
					{
						UI_ClanJoinRequest.active.Response(response);
						UI_ClanJoinRequest.active = null;
					}
					break;
				case RequestId.SENDCHAT:
					response = packet.ReadInt();
					UI_Chat.instance.ChatSendResponse(response);
					break;
				case RequestId.GETCHATS:
					string chatsData = packet.ReadString();
					List<Data.CharMessage> messages = Data.Deserialize<List<Data.CharMessage>>(chatsData);
					int chatType = packet.ReadInt();
					UI_Chat.instance.ChatSynced(messages, (Data.ChatType)chatType);
					break;
			}
			}
			catch (System.Exception ex)
			{
				Debug.Log(ex.Message);				
			}

			
		}

        private void OpenWarHistoryList(List<Data.ClanWarData> warReports)
        {
            throw new NotImplementedException();
        }

        public void SendSyncRequest()
		{
			Packet p = new Packet();
			p.Write((int)RequestId.SYNC);
			p.Write(SystemInfo.deviceUniqueIdentifier);
			Sender.TCP_Send(p);
		}
		[HideInInspector] public int gold = 0;
		[HideInInspector] public int maxGold = 0;

		[HideInInspector] public int elixir = 0;
		[HideInInspector] public int maxElixir = 0; 
		[HideInInspector] public int darkElixir = 0;
		[HideInInspector] public int maxDarkElixir = 0;
		[HideInInspector] public int gems = 0;


		public void SyncData(Data.Player player)
		{
			data = player;

			gold = 0;
			maxGold = 0;

			elixir = 0;
			maxElixir = 0;

			darkElixir = 0;
			maxDarkElixir = 0;

			int gems = player.gems;

			if (player.buildings != null && player.buildings.Count > 0)
			{
				for (int i = 0; i < player.buildings.Count; i++)
				{
					switch (player.buildings[i].id)
					{
						case Data.BuildingID.townhall:
							maxGold += player.buildings[i].goldCapacity;
							gold += player.buildings[i].goldStorage;
							maxElixir += player.buildings[i].elixirCapacity;
							elixir += player.buildings[i].elixirStorage;
							maxDarkElixir += player.buildings[i].darkCapacity;
							darkElixir += player.buildings[i].darkStorage;
							break;
						case Data.BuildingID.goldstorage:
							maxGold += player.buildings[i].goldCapacity;
							gold += player.buildings[i].goldStorage;
							break;
						case Data.BuildingID.elixirstorage:
							maxElixir += player.buildings[i].elixirCapacity;
							elixir += player.buildings[i].elixirStorage;
							break;
						case Data.BuildingID.darkelixirstorage:
							maxDarkElixir += player.buildings[i].darkCapacity;
							darkElixir += player.buildings[i].darkStorage;
							break;
					}
				}
			}

			for (int i = 0; i < player.units.Count; i++)
			{

			}

			UI_Main.instance._goldText.text = gold + "/" + maxGold;
			UI_Main.instance._elixirText.text = elixir + "/" + maxElixir;
			UI_Main.instance._gemsText.text = gems.ToString();

			if (UI_Main.instance.isActive && !UI_WarLayout.instance.isActive)
			{
				UI_Main.instance.DataSynced();
			}
			else if (UI_WarLayout.instance.isActive)
			{
				UI_WarLayout.instance.DataSynced();
			}
			else if (UI_Train.instance.isOpen)
			{
				UI_Train.instance.Sync();
			}
		}


		public void RushSyncRequest()
		{
			timer = 0;
		}


		private void InitializeConnection()
		{
			RealtimeNetworking.OnDisconnectedFromServer += DisconnectedFromServer;
			string device = SystemInfo.deviceUniqueIdentifier;
			Packet packet = new Packet();
			packet.Write((int)RequestId.AUTH);
			packet.Write(device);
			Sender.TCP_Send(packet);
		}

	

		private void DisconnectedFromServer()
		{
			ThreadDispatcher.instance.Enqueue(() => Disconnected() );
		}

		private void Disconnected()
		{
			connected = false;
			RealtimeNetworking.OnDisconnectedFromServer -= DisconnectedFromServer;
			MessageBox.Open(0, 0.8f, false, MessageResponded, new string[] { "Failed to connect to server. Please check you internet connection and try again." }, new string[] { "Try Again" });

		}

		private void MessageResponded(int layoutIndex, int buttonIndex)
		{
			if (layoutIndex == 0)
			{
				SceneManager.LoadSceneAsync(0);
			}
		}
	}
}


