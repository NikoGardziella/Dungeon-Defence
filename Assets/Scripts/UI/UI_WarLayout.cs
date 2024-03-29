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
		
		[HideInInspector] public int lastX = 25;
		[HideInInspector] public int lastY = 25;

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
				Placeunits();
				PlaceDungeonUnits();
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

		

		private void PlaceDungeonUnits()
		{
			Debug.Log("Placing Dungeon Units: "+ Player.instance.data.dungeonUnits.Count);
			ClearItems();
			UI_Main.instance._grid.ClearUnits();
			for (int i = 0; i < Player.instance.data.dungeonUnits.Count; i++)
			{				
				if(Player.instance.data.dungeonUnits[i].x >= 0 && Player.instance.data.dungeonUnits[i].y >= 0)
				{
					Unit prefab = UI_Main.instance.GetUnitPrefab(Player.instance.data.dungeonUnits[i].id);
					if (prefab)
					{
						Debug.Log("placing "+Player.instance.data.dungeonUnits[i].id);
						Unit unit = Instantiate(prefab, Vector3.zero, Quaternion.identity);
						unit.databaseID = Player.instance.data.dungeonUnits[i].databaseID;
						unit.data.id = Player.instance.data.dungeonUnits[i].id;
						unit.PlacedOnGrid(Player.instance.data.dungeonUnits[i].x, Player.instance.data.buildings[i].y);
						unit._baseArea.gameObject.SetActive(false); 
						UI_Main.instance._grid.units.Add(unit);
					}
					else
						Debug.Log("no prefab for: " + Player.instance.data.dungeonUnits[i].id);
				}
				else
				{
					Debug.Log("bad coordinates for: "+ Player.instance.data.dungeonUnits[i].databaseID + " x : " + Player.instance.data.dungeonUnits[i].x + " y: " + Player.instance.data.dungeonUnits[i].y);
					//UI_WarLayoutBuilding building = Instantiate(_listPrefab, _listGrid);
					//building.Initialized(Player.instance.data.buildings[i]);
					//buildingItems.Add(building) ;// NOT necessary for dungeon!
				}
			}
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
						building.data.warX = Player.instance.data.buildings[i].warX;
						building.data.warY = Player.instance.data.buildings[i].warY;
						building.PlacedOnGrid(Player.instance.data.buildings[i].warX, Player.instance.data.buildings[i].warY);
						building.BuildingInitRotation(Player.instance.data.buildings[i].yRotation);

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
		private void Placeunits()
		{
			UI_Main.instance._grid.ClearUnits();
			for (int i = 0; i < Player.instance.data.units.Count; i++)
			{				
				if(Player.instance.data.units[i].positionX >= 0 && Player.instance.data.units[i].positionY >= 0)
				{
					Unit prefab = UI_Main.instance.GetUnitPrefab(Player.instance.data.units[i].id);
					if (prefab)
					{
						Unit unit = Instantiate(prefab, Vector3.zero, Quaternion.identity);
						unit.databaseID = Player.instance.data.units[i].databaseID;
						unit.id = Player.instance.data.units[i].id;
						unit.PlacedOnGrid(Player.instance.data.units[i].positionX, Player.instance.data.units[i].positionY);
						unit._baseArea.gameObject.SetActive(false); 
						UI_Main.instance._grid.units.Add(unit);
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
								//Debug.Log(building.yRotation);
								building.BuildingInitRotation(Player.instance.data.buildings[i].yRotation);
								building.SetBuildingSize(Player.instance.data.buildings[i].size);
								building.data = Player.instance.data.buildings[i];
								//Debug.Log("building id: " + building.data.id);
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
					if (Player.instance.data.dungeonUnits[i].x >= 0 && Player.instance.data.dungeonUnits[i].y >= 0)
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
								unit.PlacedOnGrid(Player.instance.data.dungeonUnits[i].x, Player.instance.data.dungeonUnits[i].y);
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