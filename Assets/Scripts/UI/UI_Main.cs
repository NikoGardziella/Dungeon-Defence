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
		[SerializeField] public Button _chatButton = null;
		[SerializeField] public Button _settingsButton = null;
		[SerializeField] public Button _wallBrushButton = null;

		[SerializeField] public Button _wallBrushRemoveButton = null;
		[SerializeField] public Button _challengesButton = null;


		[SerializeField] public BuildGrid _grid = null;
		[SerializeField] public Building[] _buildingPrefabs = null;
		[SerializeField] public Unit[] _unitPrefabs = null;
		[SerializeField] public BattleUnit[] _battleUnitPrefabs = null;


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
			_chatButton.onClick.AddListener(ChatButtonClicked);
			_settingsButton.onClick.AddListener(SettingsButtonClicked);
			_wallBrushButton.onClick.AddListener(WallBrushButtonClicked);
			_wallBrushRemoveButton.onClick.AddListener(WallBrushRemoveButtonClicked);
			_challengesButton.onClick.AddListener(ChallengesButtonClicked);


		}

		private void ChallengesButtonClicked()
		{
			//UI_Challenges.instance.UpdateChallenges();
			UI_Challenges.instance.SetStatus(true);
		}
		private void WallBrushButtonClicked()
		{
			if(UI_Build.instance.wallBrush)
			{
				UI_Build.instance.wallBrush = false;
				_wallBrushButton.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
				CameraController.instance.SendBrushBuildingsToServer();
			}
			else
			{
				UI_Build.instance.wallBrush = true;
				_wallBrushButton.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
				_wallBrushRemoveButton.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f); 
				UI_Build.instance.wallBrushRemove = false;

			}
		}
		private void WallBrushRemoveButtonClicked()
		{
			if(UI_Build.instance.wallBrushRemove)
			{
				UI_Build.instance.wallBrushRemove = false;
				_wallBrushRemoveButton.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f); 

			}
			else
			{

				CameraController.instance.SendBrushBuildingsToServer();
				UI_Build.instance.wallBrushRemove = true;
				_wallBrushRemoveButton.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
				_wallBrushButton.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

				UI_Build.instance.wallBrush = false;

			}
		}

		private void SettingsButtonClicked()
		{
			UI_Settings.instance.Open();
		}
		private void ShopButtonClicked()
		{
		//	UI_Build.instance.Cancel();
			UI_Shop.instance.SetStatus(true);
			SetStatus(false);
		}
		private void ChatButtonClicked()
		{
			UI_Chat.instance.Open();
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
			else
			{
				Player.instance.RushSyncRequest();
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

		public Unit GetUnitPrefab(Data.UnitID id)
		{
			for (int i = 0; i < _unitPrefabs.Length; i++)
			{
				if(_unitPrefabs[i].id == id)
				{
					return _unitPrefabs[i];
				}
				
			}
			return null;
		}
		public BattleUnit GetBattleUnitPrefab(Data.UnitID id)
		{
			for (int i = 0; i < _battleUnitPrefabs.Length; i++)
			{
				if(_battleUnitPrefabs[i].id == id)
				{
					return _battleUnitPrefabs[i];
				}
			}
			return null;
		}
		
		public void Clear()
		{

		}
		

		
    public void DataSynced()
        {
            if (Player.instance.data.buildings != null && Player.instance.data.buildings.Count > 0)
            {
                for (int i = 0; i < Player.instance.data.buildings.Count; i++)
                {
                    Building building = _grid.GetBuilding(Player.instance.data.buildings[i].databaseID);
                    if (building != null)
                    {

                    }
                    else
                    {
						Building prefab = GetBuildingPrefab(Player.instance.data.buildings[i].id);
						if (prefab)
						{
							
							building = Instantiate(prefab, Vector3.zero, Quaternion.identity);
							building.databaseID = Player.instance.data.buildings[i].databaseID;

							building.PlacedOnGrid(Player.instance.data.buildings[i].x, Player.instance.data.buildings[i].y);
							building._baseArea.gameObject.SetActive(false);
							building.BuildingInitRotation(Player.instance.data.buildings[i].yRotation);

							_grid.buildings.Add(building);
						}
                    }

                    if (building.buildBar == null)
                    {
                        building.buildBar = Instantiate(barBuild, buttoonsParent);
                        building.buildBar.gameObject.SetActive(false);
                    }

                    building.data = Player.instance.data.buildings[i];
                    switch (building.id)
                    {
                        case Data.BuildingID.goldmine:
                            if (building.collectButton == null)
                            {
                                building.collectButton = Instantiate(buttonCollectGold, buttoonsParent);
                                building.collectButton.button.onClick.AddListener(building.Collect);
                                building.collectButton.gameObject.SetActive(false);
                            }
                            break;
                        case Data.BuildingID.elixirmine:
                            if (building.collectButton == null)
                            {
                                building.collectButton = Instantiate(buttonCollectElixir, buttoonsParent);
                                building.collectButton.button.onClick.AddListener(building.Collect);
                                building.collectButton.gameObject.SetActive(false);
                            }
                            break;
                        case Data.BuildingID.darkelixirmine:
                            if (building.collectButton == null)
                            {
                                building.collectButton = Instantiate(buttonCollectDarkElixir, buttoonsParent);
                                building.collectButton.button.onClick.AddListener(building.Collect);
                                building.collectButton.gameObject.SetActive(false);
                            }
                            break;
                    }
                    building.AdjustUI();
                }
            }
        }

	}

}
