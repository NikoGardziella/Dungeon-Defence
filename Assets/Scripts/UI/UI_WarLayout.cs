namespace DungeonDefence
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;

	public class UI_WarLayout : MonoBehaviour
	{

		[SerializeField] public GameObject _elements = null;
		[SerializeField] private Button _backButton = null;
		[SerializeField] private UI_WarLayoutBuilding _listPrefab = null;
		[SerializeField] private RectTransform _listGrid = null;

		private List<UI_WarLayoutBuilding> buildingItems = new List<UI_WarLayoutBuilding>();
		private static UI_WarLayout _instance = null; public static UI_WarLayout instance { get { return _instance; } }
		private bool _active = false; public bool isActive { get { return _active; } }
		private long _placingID = 0; public long placingID { get { return _placingID; } set { _placingID = value; } }
		[SerializeField] public Building[] _dungeonBuildingPrefabs = null;
		[HideInInspector] public GameObject placingItem = null;
		private Data.Building _dungeonwall = null;
		

		private void Awake()
		{
			_instance = this;
			_elements.SetActive(false);
			
		}

		private void Start()
		{
			_backButton.onClick.AddListener(Back);
			
		}

		private void SetDungeonBuildings()
		{
			
		}

		public void SetStatus(bool status)
		{
			_placingID = 0;
			if (status)
			{
				//UI_Main.instance.SetStatus(false); // make new UI for dungeon
				PlaceBuildings();
				UI_BuildingOptions.instance.SetStatus(status);
			}
			_active = status;
			_elements.SetActive(status);
		}

		private void Back()
		{
			ClearItems();
			UI_Main.instance._grid.Clear();
			UI_Main.instance.SetStatus(true);
			CameraController.instance.dungeonLayout.SetActive(false);
			CameraController.instance.normalLayout.SetActive(true);
			SetStatus(false);
		}

		private void PlaceBuildings()
		{
			Debug.Log("Placebuildings");
			ClearItems();
			UI_Main.instance._grid.Clear();
			for (int i = 0; i < Player.instance.data.buildings.Count; i++)
			{				
				if(Player.instance.data.buildings[i].warX >= 0 && Player.instance.data.buildings[i].warY >= 0)
				{
					Building prefab = UI_Main.instance.GetBuildingPrefab(Player.instance.data.buildings[i].id);
					if (prefab)
					{
						Building building = Instantiate(prefab, Vector3.zero, Quaternion.identity);
						building.databaseID = Player.instance.data.buildings[i].databaseID;
						building.data.id = Player.instance.data.buildings[i].id;
						building.PlacedOnGrid(Player.instance.data.buildings[i].warX, Player.instance.data.buildings[i].warY);
						building._baseArea.gameObject.SetActive(false); 
						UI_Main.instance._grid.buildings.Add(building);
					}
				}
				else
				{
					//UI_WarLayoutBuilding building = Instantiate(_listPrefab, _listGrid);
					//building.Initialized(Player.instance.data.buildings[i]);
					//buildingItems.Add(building) ;// NOT necessary for dungeon!
				}
			}
		}

		

		public void ClearItems()
		{
			for (int i = 0; i < buildingItems.Count; i++)
			{
				if (buildingItems[i])
				{
					Destroy(buildingItems[i].gameObject);
				}
			}
			buildingItems.Clear();
		}

		public void DataSynced()
		{
			if (Player.instance.data.buildings != null && Player.instance.data.buildings.Count > 0)
			{
				for (int i = 0; i < Player.instance.data.buildings.Count; i++)
				{
					if (Player.instance.data.buildings[i].warX >= 0 && Player.instance.data.buildings[i].warY >= 0)
					{
						Building building = UI_Main.instance._grid.GetBuilding(Player.instance.data.buildings[i].databaseID);
						if (building != null)
						{
							
						}
						else
						{
							Building prefab = UI_Main.instance.GetBuildingPrefab(Player.instance.data.buildings[i].id);
							if (prefab)
							{
								building = Instantiate(prefab, Vector3.zero, Quaternion.identity);
								building.databaseID = Player.instance.data.buildings[i].databaseID;
								building.PlacedOnGrid(Player.instance.data.buildings[i].warX, Player.instance.data.buildings[i].warY);
								building._baseArea.gameObject.SetActive(false);
								UI_Main.instance._grid.buildings.Add(building);
							}
						}
					}
				}
			}
			if (Player.instance.data.dungeonUnits != null && Player.instance.data.dungeonUnits.Count > 0)
			{
				for (int i = 0; i < Player.instance.data.dungeonUnits.Count; i++)
				{
					if (Player.instance.data.dungeonUnits[i].position_x >= 0 && Player.instance.data.dungeonUnits[i].position_y >= 0)
					{
						Unit unit = UI_Main.instance._grid.GetUnit(Player.instance.data.dungeonUnits[i].databaseID);
						if (unit != null)
						{
							
						}
						else
						{
							Unit prefab = UI_Main.instance.GetUnitPrefab(Player.instance.data.dungeonUnits[i].id);
							if (prefab)
							{
								unit = Instantiate(prefab, Vector3.zero, Quaternion.identity);
								unit.databaseID = Player.instance.data.dungeonUnits[i].databaseID;
								unit.PlacedOnGrid(Player.instance.data.dungeonUnits[i].position_x, Player.instance.data.dungeonUnits[i].position_y);
								unit._baseArea.gameObject.SetActive(false);
								UI_Main.instance._grid.units.Add(unit);
							}
						}
					}
				}
			}


		}

	}
}