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
				UI_Main.instance.SetStatus(false);
				PlaceBuildings();
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
						building.PlacedOnGrid(Player.instance.data.buildings[i].warX, Player.instance.data.buildings[i].warY);
						building._baseArea.gameObject.SetActive(false); 
						UI_Main.instance._grid.buildings.Add(building);
						BuildGrid.instance.CollisionGrid[Player.instance.data.buildings[i].warX + Player.instance.data.buildings[i].warY * BuildGrid.instance._columns] = 1;
					}
				}
				else
				{
					UI_WarLayoutBuilding building = Instantiate(_listPrefab, _listGrid);
					building.Initialized(Player.instance.data.buildings[i]);
					buildingItems.Add(building);
				}
			}
			PrintCollisionGrid();
		}

		void PrintCollisionGrid()
		{
			Debug.Log(BuildGrid.instance.CollisionGrid.Length);
			for (int i = 0; i < BuildGrid.instance.CollisionGrid.Length; i++)
			{
				Debug.Log(BuildGrid.instance.CollisionGrid[i]);
				if(45 % i == 0)
					Debug.Log("\n");
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
		}

	}
}