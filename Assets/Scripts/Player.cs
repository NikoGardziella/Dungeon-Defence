namespace DungeonDefence
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using DevelopersHub.RealtimeNetworking.Client;

	public class Player : MonoBehaviour
	{

		public Data.Player data = new Data.Player();
		private static Player _instance = null; public static Player instance {get {return _instance; } }

		public enum RequestId
		{
			AUTH = 1,
			SYNC = 2,
			BUILD = 3,
			REPLACE = 4,
			COLLECT = 5
		}


		private void Start()
		{
			RealtimeNetworking.OnPacketReceived += ReceivePacket;
			ConnectToServer();
		}
		private void Awake()
		{
			_instance = this;
		}

		private bool connected = false;
		private float timer = 0;

		private void Update()
		{
			if(connected)
			{
				//Every 3 seconds synd player data with server
				if(timer >= 3)
				{
					timer = 0;
					SendSyncRequest();
				}
				else
				{
					timer += Time.deltaTime;
				}
			}
		}


		private void ReceivePacket(Packet packet)
		{
			int id = packet.ReadInt();

			switch ((RequestId)id)
			{
				case RequestId.AUTH:
					connected = true;
					timer = 0;
					long accountID = packet.ReadLong();
					SendSyncRequest();
					break;
				case RequestId.SYNC:
					string playerData = packet.ReadString();
					Data.Player playerSyncData = Data.Deserialize<Data.Player>(playerData);
					SyncData(playerSyncData);
					break;
				case RequestId.BUILD:
					int response = packet.ReadInt();
					switch (response)
					{
						case 0:
							Debug.Log("no resources");
							break;
						case 1:
							Debug.Log("Placed Succesfully");
							SendSyncRequest();
							break;
						case 2:
							Debug.Log("Place taken");
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
							UI_Main.instance._grid.buildings[i].data.storage -= collectedAmmount;
							UI_Main.instance._grid.buildings[i].AdjustUI();
						}
					}
					break;
			}
		}

		public void SendSyncRequest()
		{
			Packet p = new Packet();
			p.Write((int)RequestId.SYNC);
			p.Write(SystemInfo.deviceUniqueIdentifier);
			Sender.TCP_Send(p);
		}

		private void SyncData(Data.Player player)
		{
			data = player;
			int gold = 0;
			int maxGold = 0;

			int elixir = 0;
			int maxElixir = 0;

			int darkElixir = 0;
			int maxDarkElixir = 0;

			int gems = player.gems;

			if(player.buildings != null && player.buildings.Count > 0)
			{
				for (int i = 0; i < player.buildings.Count; i++)
				{
					Building building = UI_Main.instance._grid.GetBuilding(player.buildings[i].databaseID);
					if(building != null)
					{

					}
					else
					{
						Building prefab = UI_Main.instance.GetBuildingPrefab(player.buildings[i].id);
						if(prefab)
						{
							building = Instantiate(prefab, Vector3.zero, Quaternion.identity);
							building.databaseID = player.buildings[i].databaseID;
							building.PlacedOnGrid(player.buildings[i].x, player.buildings[i].y);
							building._baseArea.gameObject.SetActive(false);

							UI_Main.instance._grid.buildings.Add(building);
						}
					}

					if(building.buildBar == null)
					{
						building.buildBar = Instantiate(UI_Main.instance.barBuild, UI_Main.instance.buttoonsParent);				
						building.buildBar.gameObject.SetActive(false);
						building.buildBar.building = building;
					}

					building.data = player.buildings[i];
					switch(building.id)
					{
						case Data.BuildingID.townhall:

							break;
						case Data.BuildingID.goldmine:
							if(building.collectButton == null)
							{
								building.collectButton = Instantiate(UI_Main.instance.buttonCollectGold, UI_Main.instance.buttoonsParent);
								building.collectButton.button.onClick.AddListener(building.Collect);
							}
							break;
						case Data.BuildingID.goldstorage:
							maxGold += building.data.capacity;
							gold += building.data.storage;
							break;
						case Data.BuildingID.elixirmine:
							if(building.collectButton == null)
							{
								building.collectButton = Instantiate(UI_Main.instance.buttonCollectElixir, UI_Main.instance.buttoonsParent);
								building.collectButton.button.onClick.AddListener(building.Collect);
							}
							break;
						case Data.BuildingID.elixirstorage:
							maxElixir += building.data.capacity;
							elixir += building.data.storage;
							break;
						case Data.BuildingID.darkelixirmine:
							if(building.collectButton == null)
							{
								building.collectButton = Instantiate(UI_Main.instance.buttonCollectDarkElixir, UI_Main.instance.buttoonsParent);
								building.collectButton.button.onClick.AddListener(building.Collect);
							}
							break;
						case Data.BuildingID.darkelixirstorage:
							maxDarkElixir += building.data.capacity;
							darkElixir += building.data.storage;
							break;
					}
					building.AdjustUI(); 
				}			
			}
			UI_Main.instance._goldText.text = gold + "/" + maxGold;
			UI_Main.instance._elixirText.text =  elixir + "/" + maxElixir;
			UI_Main.instance._gemsText.text = gems.ToString();
		}

		private void ConnectionResponse(bool succesfull)
		{
			if(succesfull)
			{
				RealtimeNetworking.OnDisconnectedFromServer += DisconnectedFromServer;
				string device = SystemInfo.deviceUniqueIdentifier;
				Packet packet = new Packet();
				packet.Write((int)RequestId.AUTH);
				packet.Write(device);
				Sender.TCP_Send(packet);
			}
			else
			{

			}
			RealtimeNetworking.OnConnectingToServerResult -= ConnectionResponse;

		}

		private void ConnectToServer()
		{
			RealtimeNetworking.OnConnectingToServerResult += ConnectionResponse;
			RealtimeNetworking.Connect();
		
		}

		private void DisconnectedFromServer()
		{
			connected = false;
			RealtimeNetworking.OnDisconnectedFromServer -= DisconnectedFromServer;
		}
	}
}


