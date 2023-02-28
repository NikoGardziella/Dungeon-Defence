namespace DungeonDefence
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using TMPro;
	using UnityEngine.UI;

	public class UI_Main : MonoBehaviour
	{
		[SerializeField] public GameObject _elements = null;

		[SerializeField] public TextMeshProUGUI _goldText = null;
		[SerializeField] public TextMeshProUGUI _elixirText = null;
		[SerializeField] public TextMeshProUGUI _gemsText = null;

		[SerializeField] public Button _shopButton = null;
		[SerializeField] public Button _battleButton = null;


		[SerializeField] public BuildGrid _grid = null;
		[SerializeField] public Building[] _buildingPrefabs = null;

		[Header("Buttons")]
		public Transform buttoonsParent = null;
		public UI_Button buttonCollectGold = null;
		public UI_Button buttonCollectElixir = null;

		public UI_Button buttonCollectDarkElixir = null;

		public UI_Bar barBuild = null;


		private static UI_Main _instance = null; public static UI_Main instance {get {return _instance; } }
		private bool _active = true; public bool isActive {get { return _active; } }

		private void Awake()
		{
			_instance = this;
			_elements.SetActive(true);
		}

		private void Start()
		{
			_shopButton.onClick.AddListener(ShopButtonClicked);
			_battleButton.onClick.AddListener(BattleButtonClicked);

		}

		private void ShopButtonClicked()
		{
		//	UI_Build.instance.Cancel();
			UI_Shop.instance.SetStatus(true);
			SetStatus(false);
		}

		private void BattleButtonClicked()
		{
			//UI_Build.instance.Cancel();
			UI_Search.instance.SetStatus(true);
			SetStatus(false);
		}

		private void OnLeave()
		{
			UI_Build.instance.Cancel();

		}

		public void SetStatus(bool status)
		{
			if(!status)
			{
				OnLeave();
			}
			_active = status;
			_elements.SetActive(status);
		}

		public Building GetBuildingPrefab(Data.BuildingID id)
		{
			for (int i = 0; i < _buildingPrefabs.Length; i++)
			{
				if(_buildingPrefabs[i].id == id)
				{
					return _buildingPrefabs[i];
				}
				
			}
			return null;
		}

		public void Clear()
		{

		}

	}

}
