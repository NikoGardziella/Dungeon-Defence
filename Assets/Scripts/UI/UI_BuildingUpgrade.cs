namespace DungeonDefence
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using TMPro;
	using DevelopersHub.RealtimeNetworking.Client;

	public class UI_BuildingUpgrade : MonoBehaviour
	{
		private static UI_BuildingUpgrade _instance = null; public static UI_BuildingUpgrade instance {get {return _instance; } }

		[SerializeField] public GameObject _elements = null;
		[SerializeField] private Button _closeButton = null;
		[SerializeField] private TextMeshProUGUI reqGold = null;
		[SerializeField] private TextMeshProUGUI reqElixir = null;

		[SerializeField] private TextMeshProUGUI reqDarkElixir = null;

		[SerializeField] private TextMeshProUGUI reqGems = null;

		[SerializeField] private TextMeshProUGUI reqTime = null;

		[SerializeField] private Button _upgradeButton = null;

		private long id = 0;

		private void Awake()
		{
			_instance = this;
			_elements.SetActive(false);
		}

		private void Start()
		{
			_closeButton.onClick.AddListener(Close);
			_upgradeButton.onClick.AddListener(UpgradeBuilding);
		}

		public void Close()
		{
			_elements.SetActive(false);
		}
		public void Open(Data.ServerBuilding building, long databaseID)
		{
			id = databaseID;
			if(string.IsNullOrEmpty(building.id))
			{

			}
			else
			{
				reqGold.text = building.requiredGold.ToString();
				reqElixir.text = building.requiredElixir.ToString();
				reqDarkElixir.text = building.requiredDarkElixir.ToString();
				reqGems.text = building.requiredGems.ToString();
				reqTime.text = building.buildTime.ToString();
			}
			_elements.SetActive(true);
			if(building.requiredElixir <= 0)
			{
				reqElixir.gameObject.SetActive(false);
			}
			else
			{
				reqElixir.gameObject.SetActive(true);
			}
			if(building.requiredGold <= 0)
			{
				reqGold.gameObject.SetActive(false);
			}
			else
			{
				reqGold.gameObject.SetActive(true);
			}
			if(building.requiredDarkElixir <= 0)
			{
				reqDarkElixir.gameObject.SetActive(false);
			}
			else
			{
				reqDarkElixir.gameObject.SetActive(true);
			}
		}

		private void UpgradeBuilding()
		{
			Packet packet = new Packet();
			packet.Write((int)Player.RequestId.UPGRADE);
			packet.Write(id);
			Sender.TCP_Send(packet);
			Close();
		}

		public void ShowInfo()
		{
			Debug.Log("Info button clicked");
		}
	}
}
