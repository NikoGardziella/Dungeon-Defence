namespace DungeonDefence
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using DevelopersHub.RealtimeNetworking.Client;

	public class Player : MonoBehaviour
	{
		public enum RequestId
		{
			AUTH = 1,
			SYNC = 2,
			BUILD = 3
		}


		private void Start()
		{
			RealtimeNetworking.OnPacketReceived += ReceivePacket;
			ConnectToServer();
		}


		private void ReceivePacket(Packet packet)
		{
			int id = packet.ReadInt();

			switch ((RequestId)id)
			{
				case RequestId.AUTH:
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
			UI_Main.instance._goldText.text = player.gold.ToString();
			UI_Main.instance._elixirText.text = player.elixir.ToString();
			UI_Main.instance._gemsText.text = player.gems.ToString();
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
							Building b = Instantiate(prefab, Vector3.zero, Quaternion.identity);
							b.PlacedOnGrid(player.buildings[i].x, player.buildings[i].y);
							b._baseArea.gameObject.SetActive(false);

							UI_Main.instance._grid.buildings.Add(b);
						}
					}
				}			
			}
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
				RealtimeNetworking.OnDisconnectedFromServer -= DisconnectedFromServer;
		}
	}
}


