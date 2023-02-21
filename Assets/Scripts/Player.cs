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
			RealtimeNetworking.OnLongReceived += ReceiveLong;
			RealtimeNetworking.OnStringReceived += ReceiveString;
			ConnectToServer();
		}

		private void ReceiveLong(int id, long value)
		{
			switch (id)
			{
				case 1:
					Debug.Log(value);
					Sender.TCP_Send((int)RequestId.SYNC, SystemInfo.deviceUniqueIdentifier);
					break;
			}
		}
		private void ReceiveString(int id, string value)
		{
			switch (id)
			{
				case 2:
					Data.Player player = Data.Deserialize<Data.Player>(value);
					UI_Main.instance._goldText.text = player.gold.ToString();
					UI_Main.instance._elixirText.text = player.elixir.ToString();
					UI_Main.instance._gemsText.text = player.gems.ToString();
					break;
			}
		}

		private void ConnectionResponse(bool succesfull)
		{
			if(succesfull)
			{
				RealtimeNetworking.OnDisconnectedFromServer += DisconnectedFromServer;
				string device = SystemInfo.deviceUniqueIdentifier;
				Sender.TCP_Send((int)RequestId.AUTH, device);
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


