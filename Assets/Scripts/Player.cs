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
			AUTH = 1
		}


		private void Start()
		{
			RealtimeNetworking.OnLongReceived += ReceiveLong;
			ConnectToServer();
		}

		private void ReceiveLong(int id, long value)
		{
			switch (id)
			{
				case 1:
					Debug.Log(value);
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


