namespace DungeonDefence
{
	using DevelopersHub.RealtimeNetworking.Client;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using TMPro;
	using UnityEngine;
	using UnityEngine.UI;

	public class UI_Battle : MonoBehaviour
	{
		

		Battle battle = null;
		public bool isStarted = false;
		private bool readyToStart = false;
		[SerializeField] private GameObject _endPanel = null;
		[SerializeField] private GameObject _dungeonPanel = null;
		[SerializeField] private GameObject _dungeonBattlePanel = null;

		[SerializeField] private GameObject _normalPanel = null;
		public GameObject TestCollision = null;


		[SerializeField] public TextMeshProUGUI _timerText = null;
		[SerializeField] public TextMeshProUGUI _battleTimeText = null;

		[SerializeField] public TextMeshProUGUI _percentageText = null;
		[SerializeField] public TextMeshProUGUI _lootGoldText = null;
		[SerializeField] public TextMeshProUGUI _lootElixirText = null;
		[SerializeField] public TextMeshProUGUI _lootDarkText = null;
		[SerializeField] private UI_Bar healthBarPrefab = null;
		[SerializeField] private RectTransform healthBarGrid = null;
		[SerializeField] private BattleUnit[] battleUnits = null;
		[SerializeField] private Button _findButton = null;
		[SerializeField] private Button _findDungeonButton = null;
		[SerializeField] private Button _AttackDungeonButton = null;


		[SerializeField] private Button _closeButton = null;
		[SerializeField] private Button _okButton = null;
		[SerializeField] private Button _surrenderButton = null;
		[SerializeField] private UI_SpellEffect spellEffectPrefab = null;
		[SerializeField] private UI_Projectile projectilePrefab = null;
		[SerializeField] private GameObject normalLayout = null;
		[SerializeField] private GameObject dungeonLayout = null;
		private List<BattleUnit> unitsOnGrid = new List<BattleUnit>();
		public List<BuildingOnGrid> buildingsOnGrid = new List<BuildingOnGrid>();
		private List<BattleUnit> dungeonUnitsOnGrid = new List<BattleUnit>();

		private DateTime baseTime;
		private List<ItemToAdd> toAddUnits = new List<ItemToAdd>();
		private List<ItemToAdd> toAddSpells = new List<ItemToAdd>();
		private long target = 0;
		private bool surrender = false;
		private Data.BattleType _battleType = Data.BattleType.normal;
		public UI_Player m_Player;
		public Battle.BattlePlayer MainPlayer;

		public class BuildingOnGrid
		{
			public long id = 0;
			public int index = -1;
			public Building building = null;
			public UI_Bar healthBar = null;
		}
		public class DungeonUnitOnGrid
		{
			public long id = 0;
			public int index = -1;
			public Unit dungeonUnit = null;
			public UI_Bar healthBar = null;
			public Transform transform;
		}

		private class ItemToAdd
		{
			public ItemToAdd(long id, int x, int y)
			{
				this.id = id;
				this.x = x;
				this.y = y;
			}
			public long id;
			public int x;
			public int y;
		}

		public int[,] CollisionGrid = null;
		private int _rows = GameConstants._ROWS;
		private int _columns = GameConstants._COLUMNS;

		private void Start()
		{
			_closeButton.onClick.AddListener(Close);
			_findButton.onClick.AddListener(Find);
			_findDungeonButton.onClick.AddListener(FindDungeon);
			_okButton.onClick.AddListener(CloseEndPanel);
			_surrenderButton.onClick.AddListener(Surrender);
			_AttackDungeonButton.onClick.AddListener(StartBattle);
			dungeonLayout.SetActive(false);
			_dungeonPanel.SetActive(false);
			InitCollisionGrid();

			//CreateCollisionTest();
		}



		void CreateCollisionTest()
		{
			for (int i = 0; i < 10; i++)
			{
				Instantiate(TestCollision, new Vector3(25, 1, 25 + i),Quaternion.Euler(0f,45f,0f));
				CollisionGrid[25,25 + i] = 0;
			}
		}

		void InitCollisionGrid()
		{
			CollisionGrid = new int[(_rows + 1) * GameConstants._COLLISION_GRID_PRECISION, (_columns + 1) * GameConstants._COLLISION_GRID_PRECISION];
			for (int y = 0; y < _rows * GameConstants._COLLISION_GRID_PRECISION; y++)
			{
				for (int x = 0; x < _columns * GameConstants._COLLISION_GRID_PRECISION; x++)
				{
					CollisionGrid[y,x] = 0;				
				}
			}
		}

		private void CloseEndPanel()
		{
			Close();
		}

		private void MessageResponded(int layoutIndex, int buttonIndex)
		{
			if (layoutIndex == 1)
			{
				MessageBox.Close();
			}
		}

		private void Close()
		{
			CameraController.instance.dungeonLayout.SetActive(false);
			CameraController.instance.normalLayout.SetActive(true);

			Player.instance.SyncData(Player.instance.data);
			isStarted = false;
			readyToStart = false;
			SetStatus(false);
			UI_Main.instance.SetStatus(true);
		}

		public void Find()
		{
			readyToStart = false;
			_findButton.gameObject.SetActive(false);
			_closeButton.gameObject.SetActive(false);
			UI_Search.instance.Find();
		}
		public void FindDungeon()
		{
			readyToStart = false;
			_findDungeonButton.gameObject.SetActive(false);
			_closeButton.gameObject.SetActive(false);
			UI_Search.instance.FindDungeon();
		}

		List<Data.Building> startbuildings = new List<Data.Building>();
		List<Data.Unit> StartDungeonUnits = new List<Data.Unit>();

		List<Battle.Building> battleBuildings = new List<Battle.Building>();
		List<Battle.Unit> battleDungeonUnits = new List<Battle.Unit>();
		List<Battle.Unit> _dungeonunits = new List<Battle.Unit>();

		public void NoTarget()
		{
			Close();
			MessageBox.Open(1, 0.8f, true, MessageResponded, new string[] { "There is no target to attack at this moment. Please try again later." }, new string[] { "OK" });
		}
		List<Data.Building> _Buildings = new List<Data.Building>();

		public bool Display(List<Data.Building> buildings, long defender, Data.BattleType battleType, List<Data.Unit> dungeonUnits)
		{
		Debug.Log(message: " display");
			
			ClearSpells();
			ClearUnits();
			for (int i = 0; i < Player.instance.data.units.Count; i++)
			{
				if (!Player.instance.data.units[i].ready)
				{
					continue;
				}
				int k = -1;
				for (int j = 0; j < units.Count; j++)
				{
					if (units[j].id == Player.instance.data.units[i].id)
					{
						k = j;
						break;
					}
				}
				if (k < 0)
				{
					k = units.Count;
					UI_BattleUnit bu = Instantiate(unitsPrefab, battleItemsGrid);
					bu.Initialize(Player.instance.data.units[i].id);
					units.Add(bu);
				}
				units[k].Add(Player.instance.data.units[i].databaseID);
			}
			

			for (int i = 0; i < Player.instance.data.spells.Count; i++)
			{
				if (!Player.instance.data.spells[i].ready)
				{
					continue;
				}
				int k = -1;
				for (int j = 0; j < spells.Count; j++)
				{
					if (spells[j].id == Player.instance.data.spells[i].id)
					{
						k = j;
						break;
					}
				}
				if (k < 0)
				{
					k = spells.Count;
					UI_BattleSpell bs = Instantiate(spellsPrefab, battleItemsGrid);
					bs.Initialize(Player.instance.data.spells[i].id);
					spells.Add(bs);
				}
				spells[k].Add(Player.instance.data.spells[i].databaseID);
			}
			
			if (units.Count <= 0)
			{
				MessageBox.Open(1, 0.8f, true, MessageResponded, new string[] { "You do not have any units for battle.." }, new string[] { "OK" });
				return false;
			}
			_battleType = battleType;
			Debug.Log("batttletype:"+ _battleType);
			int townhallLevel = 1;
			Debug.Log("building count:" + buildings.Count);
			for (int i = 0; i < buildings.Count; i++)
			{
				if (buildings[i].id == Data.BuildingID.townhall)
				{
					townhallLevel = buildings[i].level;
					if (_battleType == Data.BattleType.normal)
					{
						normalLayout.SetActive(true);
						dungeonLayout.SetActive(false);
						_dungeonPanel.SetActive(false);
						m_Player.gameObject.SetActive(false);
						break;
					}
				}
				if (_battleType == Data.BattleType.dungeon)
				{
					_dungeonPanel.SetActive(true);
					normalLayout.SetActive(false);
					dungeonLayout.SetActive(true);
					buildings[i].x = buildings[i].warX;
					buildings[i].y = buildings[i].warY;
					m_Player.gameObject.SetActive(true);
				
				/*
					if(buildings[i].warX > 0 && buildings[i].warY > 0)
					{
						Data.Building building = new Data.Building();
						building = buildings[i];
						_Buildings.Add(building);
						Debug.Log("addin building");
					}					
					buildings.Clear();
					buildings = _Buildings;
					*/
				}
			}
			
			target = defender;
			startbuildings = buildings;
			StartDungeonUnits = dungeonUnits;
			battleBuildings.Clear();
			spellEffects.Clear();

			for (int i = 0; i < buildings.Count; i++)
			{
				if (buildings[i].x < 0 || buildings[i].y < 0)
				{
					
					continue;
				}
				Battle.Building building = new Battle.Building();
				building.building = buildings[i];
				switch (building.building.id)
				{
					case Data.BuildingID.townhall:
						building.lootGoldStorage = Data.GetStorageGoldAndElixirLoot(townhallLevel, building.building.goldStorage);
						building.lootElixirStorage = Data.GetStorageGoldAndElixirLoot(townhallLevel, building.building.elixirStorage);
						building.lootDarkStorage = Data.GetStorageDarkElixirLoot(townhallLevel, building.building.darkStorage);
						break;
					case Data.BuildingID.goldmine:
						building.lootGoldStorage = Data.GetMinesGoldAndElixirLoot(townhallLevel, building.building.goldStorage);
						break;
					case Data.BuildingID.goldstorage:
						building.lootGoldStorage = Data.GetStorageGoldAndElixirLoot(townhallLevel, building.building.goldStorage);
						break;
					case Data.BuildingID.elixirmine:
						building.lootElixirStorage = Data.GetMinesGoldAndElixirLoot(townhallLevel, building.building.elixirStorage);
						break;
					case Data.BuildingID.elixirstorage:
						building.lootElixirStorage = Data.GetStorageGoldAndElixirLoot(townhallLevel, building.building.elixirStorage);
						break;
					case Data.BuildingID.darkelixirmine:
						building.lootDarkStorage = Data.GetMinesDarkElixirLoot(townhallLevel, building.building.darkStorage);
						break;
					case Data.BuildingID.darkelixirstorage:
						building.lootDarkStorage = Data.GetStorageDarkElixirLoot(townhallLevel, building.building.darkStorage);
						break;
				}
				battleBuildings.Add(building);
			}

			for (int i = 0; i < dungeonUnits.Count; i++)
			{
				if (dungeonUnits[i].x < 0 || dungeonUnits[i].y < 0)
				{
					//continue;
				}
				Battle.Unit unit = new Battle.Unit();
				unit.unit = dungeonUnits[i];
				//Vector2 tempVec = GridToWorldPosition(new Vector2(unit.unit.x, unit.unit.y));
				Vector3 tempVec = UI_Main.instance._grid.transform.TransformPoint(dungeonUnits[i].x,0,dungeonUnits[i].y);
				unit.unit.positionX = dungeonUnits[i].x;
				unit.unit.positionY = dungeonUnits[i].y;
				unit.unit.databaseID = dungeonUnits[i].databaseID;
				unit.unit.x = (int)tempVec.x;
				unit.unit.y = (int)tempVec.z;
				battleDungeonUnits.Add(unit);
				Debug.Log("unit.unit.x " + unit.unit.x +"unit.unit.y " + unit.unit.y + " || " + "unit.unit.positionX " + unit.unit.positionX +"unit.unit.positionY " + unit.unit.positionY + " || " + " databaseID: "+ unit.unit.databaseID  + " ID: "+ unit.unit.id );
			}
			

			_timerText.text = TimeSpan.FromSeconds(Data.battlePrepDuration).ToString(@"mm\:ss");
			_battleTimeText.text = "Time to Prepare";


			ClearBuildingsOnGrid();
			ClearUnitsOnGrid();
			//Vector3 tempPos = Vector3.zero;
			UI_Main.instance._grid.Clear();

			for (int i = 0; i < battleBuildings.Count; i++)
			{
				Building prefab = UI_Main.instance.GetBuildingPrefab(battleBuildings[i].building.id);
				if (prefab)
				{
					BuildingOnGrid building = new BuildingOnGrid();
					building.building = Instantiate(prefab, Vector3.zero, Quaternion.identity);
					building.building.databaseID = battleBuildings[i].building.databaseID;
					building.building.PlacedOnGrid(battleBuildings[i].building.x, battleBuildings[i].building.y);
					building.building._baseArea.gameObject.SetActive(false);
					

					building.healthBar = Instantiate(healthBarPrefab, healthBarGrid);
					building.healthBar.bar.fillAmount = 1;
					building.healthBar.gameObject.SetActive(false);

					building.building.data = battleBuildings[i].building;
					building.id = battleBuildings[i].building.databaseID;
					building.index = i;

					//tempPos = building.building.gameObject.transform.position;

					//UI_Player.instance._buildings.Add(building.building);
					//UI_Player.instance.buildings.Insert(i, building.building);
					buildingsOnGrid.Add(building);
				}

				battleBuildings[i].building.x += Data.battleGridOffset;
				battleBuildings[i].building.y += Data.battleGridOffset;				

				if(battleBuildings[i].building.id == Data.BuildingID.dungeontrap)
				{
					CollisionGrid[battleBuildings[i].building.x * GameConstants._COLLISION_GRID_PRECISION, battleBuildings[i].building.y * GameConstants._COLLISION_GRID_PRECISION] = 2;
				}
				else
				{
					for (int y = 0; y < battleBuildings[i].building.rows * GameConstants._COLLISION_GRID_PRECISION + 1; y++)
					{
						for (int x = 0; x < battleBuildings[i].building.rows * GameConstants._COLLISION_GRID_PRECISION+ 1; x++)
						{
							CollisionGrid[battleBuildings[i].building.x * GameConstants._COLLISION_GRID_PRECISION + x,battleBuildings[i].building.y * GameConstants._COLLISION_GRID_PRECISION + y] = i;
						}
					}
				}
			//	Instantiate(TestCollision, new Vector3(tempPos.x, 1, tempPos.z),Quaternion.Euler(0f,45f,0f));
				//Debug.Log("collisiongrid X :" + battleBuildings[i].building.y  + "collisiongrid Y " + battleBuildings[i].building.y);				
			}
			Debug.Log("buildingsOnGrid count" + buildingsOnGrid.Count + "battleBuildings.Count; " + battleBuildings.Count);

			for (int i = 0; i < battleDungeonUnits.Count; i++)
			{
				BattleUnit prefab = UI_Main.instance.GetBattleUnitPrefab(battleDungeonUnits[i].unit.id);
				if (prefab)
				{
					BattleUnit unit = new BattleUnit();
					unit = Instantiate(prefab, Vector3.zero, Quaternion.identity);
					unit.baseArea.gameObject.SetActive(false);
				//	unit.data.databaseID = battleDungeonUnits[i].unit.databaseID;
				//	unit.PlacedOnGrid(battleDungeonUnits[i].unit.x, battleDungeonUnits[i].unit.y);
				//	unit.dungeonUnit._baseArea.gameObject.SetActive(false);
				//	Debug.Log("battleDungeonUnits[i].unit.health " + battleDungeonUnits[i].unit.health);
					unit.data = battleDungeonUnits[i].unit;
					unit.healthBar = Instantiate(healthBarPrefab, healthBarGrid);
					unit.healthBar.bar.fillAmount = 1;
					unit.healthBar.gameObject.SetActive(false);
					unit.data.databaseID = battleDungeonUnits[i].unit.databaseID;
					
					//unit.dungeonUnit = battleDungeonUnits[i].;
					unit.id = battleDungeonUnits[i].unit.id;
					unit.index = i;
					Debug.Log("unit damage" + unit.data.damage + " unit health" + unit.data.health + " unit range: " + unit.data.attackRange);
					//tempPos = unit.dungeonUnit.gameObject.transform.position;
					//UI_Player.instance._buildings.Add(building.building);
					//UI_Player.instance.buildings.Insert(i, building.building);
					
					dungeonUnitsOnGrid.Add(unit);
				}
			//	Instantiate(TestCollision, new Vector3(tempPos.x, 1, tempPos.z),Quaternion.Euler(0f,45f,0f));
				//Debug.Log("collisiongrid X :" + battleBuildings[i].building.y  + "collisiongrid Y " + battleBuildings[i].building.y);				
			}
			Debug.Log("dungeonUnitsOnGrid lenght" + dungeonUnitsOnGrid.Count);
			//_findButton.gameObject.SetActive(_battleType == Data.BattleType.normal);
			_normalPanel.gameObject.SetActive(_battleType == Data.BattleType.normal);
			_dungeonPanel.gameObject.SetActive(_battleType == Data.BattleType.dungeon);
			//_findDungeonButton.gameObject.SetActive(_battleType == Data.BattleType.normal);
			_closeButton.gameObject.SetActive(true);
			_surrenderButton.gameObject.SetActive(false);
			baseTime = DateTime.Now;
			SetStatus(true);
			Debug.Log("dungeonUnitsOnGrid" + dungeonUnitsOnGrid.Count + " dungeonUnits: " + dungeonUnits.Count);

			toAddSpells.Clear();
			toAddUnits.Clear();
			battle = new Battle();
			MainPlayer = new Battle.BattlePlayer();
			MainPlayer.MainPlayer = Instantiate(m_Player, new Vector3(0,1,0), Quaternion.identity);
			MainPlayer.MainPlayer.buildingOnGrid = buildingsOnGrid;
			Debug.Log("_dungeonunits count before battle.Initialize" + battleDungeonUnits.Count);
			battle.Initialize(battleBuildings, battleDungeonUnits,MainPlayer, DateTime.Now, BuildingAttackCallBack, BuildingDestroyedCallBack, BuildingDamageCallBack, StarGained, null, UnitDiedCallBack, DungeonUnitAttackCallBack, DungeonUnitDamageCallBack);
			Debug.Log("battle initialized");
			for (int i = 0; i < battle._dungeonunits.Count; i++)
			{
				Debug.Log("posX:" + battle._dungeonunits[i].position.x + "posY:" + battle._dungeonunits[i].position.y);
				//Debug.Log("battle._dungeonunits[i].pathTraveledTime" + battle._dungeonunits[i].pathTraveledTime + "battle._dungeonunits[i].pathTime" + battle._dungeonunits[i].pathTime+ "battle._dungeonunits[i].points" + battle._dungeonunits[i].path.points);
			}
			_percentageText.text = (battle.percentage * 100f).ToString("F2") + "%";
			UpdateLoots();
			
			surrender = false;
			readyToStart = true;
			isStarted = false;
			//PrintCollisionGrid();
			return true;
		}		

		void PrintCollisionGrid()
		{
			Debug.Log("Print collision grid");
			char[] GridStr = new char[(45 + 1) * (45 + 1)];;
			

			for (int y = 0; y < _columns; y++)
			{
				for (int x = 0; x < _columns; x++)
				{
					GridStr[y + x * _columns] = (char)CollisionGrid[x,y];
				}
				GridStr[y + _columns] = '\n';
			}
			new string (GridStr);
			print(GridStr);
			Debug.Log("END of collision grid");
		}

		private void UpdateLoots()
		{
			var looted = battle.GetlootedResources();
			_lootGoldText.text = looted.Item1 + "/" + looted.Item4;
			_lootElixirText.text = looted.Item2 + "/" + looted.Item5;
			_lootDarkText.text = looted.Item3 + "/" + looted.Item6;
		}

		

		private void StartBattle()
		{
			_battleTimeText.text = "Time until end of battle";
			_timerText.text = TimeSpan.FromSeconds(Data.battleDuration).ToString(@"mm\:ss");
			_findButton.gameObject.SetActive(false);
			_closeButton.gameObject.SetActive(false);
			_dungeonPanel.SetActive(false);
			_surrenderButton.gameObject.SetActive(true);
			readyToStart = false;
			baseTime = DateTime.Now;
			Packet packet = new Packet();
			packet.Write((int)Player.RequestId.BATTLESTART);
			Data.OpponentData opponent = new Data.OpponentData();
			opponent.id = target;
			opponent.buildings = startbuildings;
			opponent.dungeonUnits = StartDungeonUnits;
			string data = Data.Serialize<Data.OpponentData>(opponent);
			packet.Write(data);
			packet.Write((int)_battleType);
			Sender.TCP_Send(packet);
			//PrintCollisionGrid();
		}

		public void BattleEnded(int stars, int unitsDeployed, int lootedGold, int lootedElixir, int lootedDark, int trophies, int frame)
		{
			_findButton.gameObject.SetActive(false);
			_closeButton.gameObject.SetActive(false);
			_surrenderButton.gameObject.SetActive(false);
			var looted = battle.GetlootedResources();
			Debug.Log("Battle Ended.");
			Debug.Log("Frame -> Client:" + battle.frameCount + " Server:" + frame);
			Debug.Log("Stars -> Client:" + battle.stars + " Server:" + stars);
			Debug.Log("Units Deployed -> Client:" + battle.unitsDeployed + " Server:" + unitsDeployed);
			Debug.Log("Looted Gold -> Client:" + looted.Item1 + " Server:" + lootedGold);
			Debug.Log("Looted Elixir -> Client:" + looted.Item2 + " Server:" + lootedElixir);
			Debug.Log("Looted Dark Elixir -> Client:" + looted.Item3 + " Server:" + lootedDark);
			Debug.Log("Trophies -> Client:" + battle.GetTrophies() + " Server:" + trophies);
			_dungeonPanel.SetActive(false);
			_dungeonBattlePanel.gameObject.SetActive(true);
			_endPanel.SetActive(true);
			m_Player.gameObject.SetActive(false);

			Destroy(MainPlayer.MainPlayer.gameObject);
		}

		public void StartBattleConfirm(bool confirmed, List<Data.BattleStartBuildingData> buildings, int winTrophies, int loseTrophies)
		{
			if (confirmed)
			{
				battle.winTrophies = winTrophies;
				battle.loseTrophies = loseTrophies;
				for (int i = 0; i < battle._buildings.Count; i++)
				{
					for (int j = 0; j < buildings.Count; j++)
					{
						if (battle._buildings[i].building.databaseID == buildings[j].databaseID)
						{
							battle._buildings[i].lootGoldStorage = buildings[j].lootGoldStorage;
							battle._buildings[i].lootElixirStorage = buildings[j].lootElixirStorage;
							battle._buildings[i].lootDarkStorage = buildings[j].lootDarkStorage;
						}
					}
				}
				isStarted = true;
			}
			else
			{
				Debug.Log("Battle is not confirmed by the server.");
			}
		}

		private void Surrender()
		{
			surrender = true;
		}

		public void EndBattle(bool surrender, int surrenderFrame)
		{
			_findButton.gameObject.SetActive(false);
			_closeButton.gameObject.SetActive(false);
			_surrenderButton.gameObject.SetActive(false);
			_dungeonBattlePanel.gameObject.SetActive(false);
			battle.end = true;
			battle.surrender = surrender;
			isStarted = false;
			Packet packet = new Packet();
			packet.Write((int)Player.RequestId.BATTLEEND);
			packet.Write(surrender);
			packet.Write(surrenderFrame);
			Sender.TCP_Send(packet);
		}

		[SerializeField] private GameObject _elements = null;
		[SerializeField] private RectTransform battleItemsGrid = null;
		[SerializeField] private UI_BattleUnit unitsPrefab = null;
		[SerializeField] private UI_BattleSpell spellsPrefab = null;
		private static UI_Battle _instance = null; public static UI_Battle instance { get { return _instance; } }
		private bool _active = false; public bool isActive { get { return _active; } }

		[HideInInspector] public int selectedUnit = -1;
		[HideInInspector] public int selectedSpell = -1;

		private List<UI_BattleUnit> units = new List<UI_BattleUnit>();
		private List<UI_BattleSpell> spells = new List<UI_BattleSpell>();
		
		public void SpellSelected(Data.SpellID id)
		{
			if (selectedUnit >= 0)
			{
				units[selectedUnit].Deselect();
				selectedUnit = -1;
			}
			if (selectedSpell >= 0)
			{
				spells[selectedSpell].Deselect();
				selectedSpell = -1;
			}
			for (int i = 0; i < spells.Count; i++)
			{
				if (spells[i].id == id)
				{
					selectedSpell = i;
					break;
				}
			}
			if (selectedSpell >= 0 && spells[selectedSpell].count <= 0)
			{
				spells[selectedSpell].Deselect();
				selectedSpell = -1;
			}
		}

		public void UnitSelected(Data.UnitID id)
		{
			if (selectedUnit >= 0)
			{
				units[selectedUnit].Deselect();
				selectedUnit = -1;
			}
			if (selectedSpell >= 0)
			{
				spells[selectedSpell].Deselect();
				selectedSpell = -1;
			}
			for (int i = 0; i < units.Count; i++)
			{
				if (units[i].id == id)
				{
					selectedUnit = i;
					break;
				}
			}
			if (selectedUnit >= 0 && units[selectedUnit].count <= 0)
			{
				units[selectedUnit].Deselect();
				selectedUnit = -1;
			}
		}

		public void PlaceUnit(int x, int y)
		{
			if (battle != null)
			{
				if(selectedUnit >= 0 && units[selectedUnit].count > 0 && battle.CanAddUnit(x, y))
				{
					if (!isStarted)
					{
						if (!readyToStart)
						{
							return;
						}
						StartBattle();
					}
					long id = units[selectedUnit].Get();
					if (id >= 0)
					{
						if (units[selectedUnit].count <= 0)
						{
							units[selectedUnit].Deselect();
							selectedUnit = -1;
						}
						toAddUnits.Add(new ItemToAdd(id, x, y));
					}
				}
				else if (selectedSpell >= 0 && spells[selectedSpell].count > 0 && battle.CanAddSpell(x, y))
				{
					if (!isStarted)
					{
						if (!readyToStart)
						{
							return;
						}
						StartBattle();
					}
					long id = spells[selectedSpell].Get();
					if (id >= 0)
					{
						if (spells[selectedSpell].count <= 0)
						{
							spells[selectedSpell].Deselect();
							selectedSpell = -1;
						}
						toAddSpells.Add(new ItemToAdd(id, x, y));
					}
				}
			}
		}

		private void Awake()
		{
			_instance = this;
			_elements.SetActive(false);
		}

		private void ClearUnits()
		{
			for (int i = 0; i < units.Count; i++)
			{
				if (units[i])
				{
					Destroy(units[i].gameObject);
				}
			}
			units.Clear();
		}

		private void ClearSpells()
		{
			for (int i = 0; i < spells.Count; i++)
			{
				if (spells[i])
				{
					Destroy(spells[i].gameObject);
				}
			}
			spells.Clear();
		}

		public void SetStatus(bool status)
		{
			Debug.Log("status:"+status);
			if (!status)
			{
				ClearSpells();
				ClearBuildingsOnGrid();
				ClearUnitsOnGrid();
				ClearUnits();
				ClearDungeonUnitsOnGrid();
			}
			else
			{
				_endPanel.SetActive(false);
			}
			Player.inBattle = status;
			_active = status;
			_elements.SetActive(status);
		}

		private void Update()
		{
			if (battle != null && battle.end == false)
			{
				if (isStarted)
				{
					TimeSpan span = DateTime.Now - baseTime;

					if (_timerText != null)
					{
						_timerText.text = TimeSpan.FromSeconds(Data.battleDuration - span.TotalSeconds).ToString(@"mm\:ss");
					}

					int frame = (int)Math.Floor(span.TotalSeconds / Data.battleFrameRate);
					if (frame > battle.frameCount)
					{
						if (toAddUnits.Count > 0 || toAddSpells.Count > 0)
						{
							Data.BattleFrame battleFrame = new Data.BattleFrame();
							battleFrame.frame = battle.frameCount + 1;

							if (toAddUnits.Count > 0)
							{
								for (int i = toAddUnits.Count - 1; i >= 0; i--)
								{
									for (int j = 0; j < Player.instance.data.units.Count; j++)
									{
										if (Player.instance.data.units[j].databaseID == toAddUnits[i].id)
										{
											battle.AddUnit(Player.instance.data.units[j], toAddUnits[i].x, toAddUnits[i].y, UnitSpawnCallBack, UnitAttackCallBack, UnitDiedCallBack, UnitDamageCallBack, UnitHealCallBack);
											Data.BattleFrameUnit bfu = new Data.BattleFrameUnit();
											bfu.id = Player.instance.data.units[j].databaseID;
											bfu.x = toAddUnits[i].x;
											bfu.y = toAddUnits[i].y;
											battleFrame.units.Add(bfu);
											break;
										}
									}
									toAddUnits.RemoveAt(i);
								}
							}

							if (toAddSpells.Count > 0)
							{
								for (int i = toAddSpells.Count - 1; i >= 0; i--)
								{
									for (int j = 0; j < Player.instance.data.spells.Count; j++)
									{
										if (Player.instance.data.spells[j].databaseID == toAddSpells[i].id)
										{
											Data.Spell spell = Player.instance.data.spells[j];
											Player.instance.AssignServerSpell(ref spell);
											battle.AddSpell(spell, toAddSpells[i].x, toAddSpells[i].y, SpellSpawnCallBack, SpellPalseCallBack, SpellEndCallBack);
											Data.BattleFrameSpell bfs = new Data.BattleFrameSpell();
											bfs.id = spell.databaseID;
											bfs.x = toAddSpells[i].x;
											bfs.y = toAddSpells[i].y;
											battleFrame.spells.Add(bfs);
											break;
										}
									}
									toAddSpells.RemoveAt(i);
								}
							}

							Packet packet = new Packet();
							packet.Write((int)Player.RequestId.BATTLEFRAME);
							string frameData = Data.Serialize<Data.BattleFrame>(battleFrame);
							packet.Write(frameData);
							Sender.TCP_Send(packet);
						}
					//	Debug.Log("player x:" +  battle.MainPlayer.position.x + "player Y:" +  battle.MainPlayer.position.y);
						battle.ExecuteFrame();
					//write every 50th frame
					if(frame % 50 == 0)
					{
						for (int i = 0; i < battle._dungeonunits.Count; i++)
						{
							if(battle._dungeonunits[i].path == null)
								Debug.Log("unit" + i + " has not path");
						//	Debug.Log(" battle._dungeonunits[i] target." +  battle._dungeonunits[i].target);
							Debug.Log("unit:" + i + "  unit position.x: " + battle._dungeonunits[i].position.x + "position.y: " + battle._dungeonunits[i].position.y + " || " + "target:" + battle._dungeonunits[i].target + " || " + "blocket tiles: " + battle.pubBlocked  + " || " + "path.points.Count;: " + battle.pubpathLen  + " || " + "unit health: " + battle.pubUnitHealth + " || " + "pubSwordUnitDis: " + battle.pubSwordUnitDis+ " || " + "attackTimer" + battle._dungeonunits[i].attackTimer);
							Debug.Log("pathTime" + battle._dungeonunits[i].pathTime + "pathTraveledTime" + battle._dungeonunits[i].pathTraveledTime);
						//	Debug.Log("battle._dungeonunits[i].points" + battle._dungeonunits[i].path.points + "battle._dungeonunits[i].length" + battle._dungeonunits[i].path.length); 
						}
						Debug.Log("playerX:" + battle.MainPlayer.position.x + "playerY:" + battle.MainPlayer.position.y +" || " + "WorldX:" + battle.MainPlayer.MainPlayer.WorldX + "WorldY:" + battle.MainPlayer.MainPlayer.WorldY + " || " +"GridX:" + battle.MainPlayer.MainPlayer.GridX + "GridY:" + battle.MainPlayer.MainPlayer.GridY + "|| " +"Player attackTimer:" + battle.MainPlayer.attackTimer + "health:" + battle.MainPlayer.health + "  " + "|| "+ "frame" + frame); 
					//	Debug.Log("path1 endX" + battle.pubPath1.end.x + "path1 endY" + battle.pubPath1.end.y);
//						Debug.Log("path1 startX" + battle.pubPath1.start.x + "path1 startY" + battle.pubPath1.start.y + "distance:" + battle.pubDistance + " || " + " distance to end of path:" + battle.pubDistanceToPathEnd);
						
					}
						if ((float)battle.frameCount * Data.battleFrameRate >= battle.duration || Math.Abs(battle.percentage - 1d) <= 0.0001d)
						{
							EndBattle(false, battle.frameCount);
						}
						else if (MainPlayer.health < 0 || surrender || (!battle.IsAliveUnitsOnGrid() && !HaveUnitLeftToPlace()))
						{
							EndBattle(true, battle.frameCount);
						}
					}
				}
				else if (readyToStart)
				{
					TimeSpan span = DateTime.Now - baseTime;
					if(span.TotalSeconds >= Data.battlePrepDuration)
					{
						StartBattle();
					}
					else
					{
						_timerText.text = TimeSpan.FromSeconds(Data.battlePrepDuration - span.TotalSeconds).ToString(@"mm\:ss");
					}
				}
				UpdateUnits();
				UpdateDungeonUnits();
				UpdatePlayer();
			//	UpdateBuildings();
			}
		}

		private bool HaveUnitLeftToPlace()
		{
			if (units.Count > 0)
			{
				for (int i = 0; i < units.Count; i++)
				{
					if (units[i].count > 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		private BattleUnit GetUnitPrefab(Data.UnitID id)
		{
			for (int i = 0; i < battleUnits.Length; i++)
			{
				if(battleUnits[i].id == id)
				{
					return battleUnits[i];
				}
			}
			return null;
		}

		public void ClearUnitsOnGrid()
		{
			for (int i = 0; i < unitsOnGrid.Count; i++)
			{
				if (unitsOnGrid[i])
				{
					Destroy(unitsOnGrid[i].gameObject);
				}
			}
			unitsOnGrid.Clear();
		}
		public void ClearDungeonUnitsOnGrid()
		{
			for (int i = 0; i < dungeonUnitsOnGrid.Count; i++)
			{
				if (dungeonUnitsOnGrid[i])
				{
					Destroy(dungeonUnitsOnGrid[i].gameObject);
				}
			}
			dungeonUnitsOnGrid.Clear();
		}

		public void ClearBuildingsOnGrid()
		{
			Debug.Log("clearing buidlings on grid");
			for (int i = 0; i < buildingsOnGrid.Count; i++)
			{
				if (buildingsOnGrid[i].building != null)
				{
					Destroy(buildingsOnGrid[i].building.gameObject);
				}
			}
			buildingsOnGrid.Clear();
		}

		#region Events
		private void UpdateUnits()
		{
			for (int i = 0; i < unitsOnGrid.Count; i++)
			{
				if (battle._units[unitsOnGrid[i].index].health > 0)
				{
					Vector3 position = new Vector3(battle._units[unitsOnGrid[i].index].positionOnGrid.x, 0, battle._units[unitsOnGrid[i].index].positionOnGrid.y);
					unitsOnGrid[i].transform.localPosition = position;
				  
					if(battle._units[unitsOnGrid[i].index].health < battle._units[unitsOnGrid[i].index].unit.health)
					{
						unitsOnGrid[i].healthBar.gameObject.SetActive(true);
						unitsOnGrid[i].healthBar.bar.fillAmount = battle._units[unitsOnGrid[i].index].health / battle._units[unitsOnGrid[i].index].unit.health;
						unitsOnGrid[i].healthBar.rect.anchoredPosition = GetUnitBarPosition(unitsOnGrid[i].transform.position);
					}
					else
					{
						unitsOnGrid[i].healthBar.gameObject.SetActive(false);
					}
				}
			}
		}


		private float timer = 0;
		private float attackTimer = 0.5f;
		private void UpdatePlayer()
		{
			if(MainPlayer.MainPlayer.PlayerIsAttacking)
			{
				if(timer <= 0)
				{
					timer = attackTimer;
					battle.PlayerAttack();
				}
				else
				{
					timer -= Time.deltaTime;
				}

			}
		}

		private void UpdateBuildings()
		{
		//	Debug.Log("buildingsOnGrid.Count" + buildingsOnGrid.Count +" battle._buildings.Count" + battle._buildings.Count);
		
			for (int i = 0; i < buildingsOnGrid.Count; i++)
			{
				if(buildingsOnGrid[i].healthBar != null)
				{
					if (battle._buildings[i].health > 0)
					{
						if (battle._buildings[buildingsOnGrid[i].index].health < battle._buildings[buildingsOnGrid[i].index].building.health)
						{
							buildingsOnGrid[i].healthBar.gameObject.SetActive(true);
							buildingsOnGrid[i].healthBar.bar.fillAmount = battle._buildings[buildingsOnGrid[i].index].health / battle._buildings[buildingsOnGrid[i].index].building.health;
							buildingsOnGrid[i].healthBar.rect.anchoredPosition = GetUnitBarPosition(UI_Main.instance._grid.GetEndPosition(buildingsOnGrid[i].building));
						}
					}
				}
			}
		}

		private static Vector2 GridToWorldPosition(Vector2 position)
		{
			return new Vector2(position.x * Data.gridCellSize + Data.gridCellSize / 2f,position.y * Data.gridCellSize + Data.gridCellSize / 2f);
		}
		private void UpdateDungeonUnits()
		{
			for (int i = 0; i < dungeonUnitsOnGrid.Count; i++)
			{
				if (battle._dungeonunits[dungeonUnitsOnGrid[i].index].health > 0)
				{
					//Vector2 position = GridToWorldPosition(new Vector2( battle._dungeonunits[i].position.x, battle._dungeonunits[i].position.y));
					//Vector3 position = new Vector3(battle._dungeonunits[i].position.x, 0, battle._dungeonunits[i].position.y);
				
					Vector3 tempPos;					
					tempPos = UI_Main.instance._grid.transform.TransformPoint(new Vector3(battle._dungeonunits[dungeonUnitsOnGrid[i].index].position.x,0, battle._dungeonunits[dungeonUnitsOnGrid[i].index].position.y)); 
					dungeonUnitsOnGrid[i].transform.position = tempPos;
					
					
				//	Vector3 position = new Vector3(battle._dungeonunits[i].positionOnGrid.x, 0, battle._dungeonunits[i].positionOnGrid.y);
				//	unitsOnGrid[i].transform.localPosition = position;


					
				  
					if(battle._dungeonunits[dungeonUnitsOnGrid[i].index].health < 100)
					{
						dungeonUnitsOnGrid[i].healthBar.gameObject.SetActive(true);
						//dungeonUnitsOnGrid[i].healthBar.bar.fillAmount = battle._dungeonunits[dungeonUnitsOnGrid[i].index].health / battle._dungeonunits[dungeonUnitsOnGrid[i].index].unit.health;
						dungeonUnitsOnGrid[i].healthBar.bar.fillAmount = battle._dungeonunits[dungeonUnitsOnGrid[i].index].health / battle._dungeonunits[dungeonUnitsOnGrid[i].index].unit.health;
						dungeonUnitsOnGrid[i].healthBar.rect.anchoredPosition = GetUnitBarPosition(dungeonUnitsOnGrid[i].transform.position);
					}
					else
					{
						dungeonUnitsOnGrid[i].healthBar.gameObject.SetActive(false);
					}
				}
			}
		}

		private Vector2 GetUnitBarPosition(Vector3 position)
		{
			Vector3 planDownLeft = CameraController.instance.planeDownLeft;
			Vector3 planTopRight = CameraController.instance.planeTopRight;

			float w = planTopRight.x - planDownLeft.x;
			float h = planTopRight.z - planDownLeft.z;

			float endW = position.x - planDownLeft.x;
			float endH = position.z - planDownLeft.z;

			return new Vector2(endW / w * Screen.width, endH / h * Screen.height);
		}

		public List<UI_SpellEffect> spellEffects = new List<UI_SpellEffect>();

		public void SpellSpawnCallBack(long databaseID, Data.SpellID id, Battle.BattleVector2 target, float radius)
		{
			Vector3 position = new Vector3(target.x, 0, target.y);
			position = UI_Main.instance._grid.transform.TransformPoint(position);
			UI_SpellEffect effect = Instantiate(spellEffectPrefab, position, Quaternion.identity);
			effect.Initialize(id, databaseID, radius);
			spellEffects.Add(effect);
		}

		public void SpellPalseCallBack(long id)
		{
			for (int i = 0; i < spellEffects.Count; i++)
			{
				if(spellEffects[i].DatabaseID == battle._spells[i].spell.databaseID)
				{
					spellEffects[i].Pulse();
					break;
				}
			}
		}

		public void SpellEndCallBack(long id)
		{
			for (int i = 0; i < spellEffects.Count; i++)
			{
				if (spellEffects[i].DatabaseID == id)
				{
					spellEffects[i].End();
					break;
				}
			}
		}

		public void UnitSpawnCallBack(long id)
		{
			int u = -1;
			for (int i = 0; i < battle._units.Count; i++)
			{
				if (battle._units[i].unit.databaseID == id)
				{
					u = i;
					break;
				}
			}
			if (u >= 0)
			{
				BattleUnit prefab = GetUnitPrefab(battle._units[u].unit.id);
				if (prefab)
				{
					BattleUnit unit = Instantiate(prefab, UI_Main.instance._grid.transform);
					unit.transform.localPosition = new Vector3(battle._units[u].positionOnGrid.x, 0, battle._units[u].positionOnGrid.y);
					unit.Initialize(u, battle._units[u].unit.databaseID, battle._units[u].unit);

					unit.healthBar = Instantiate(healthBarPrefab, healthBarGrid);
					unit.healthBar.bar.fillAmount = 1;
					unit.healthBar.gameObject.SetActive(false);

					unitsOnGrid.Add(unit);
				}
			}
		}

		public void StarGained()
		{

		}

		public void UnitAttackCallBack(long id, long target)
		{
			int u = -1;
			int b = -1;
			for (int i = 0; i < unitsOnGrid.Count; i++)
			{
				if (unitsOnGrid[i].databaseID == id)
				{
					if (unitsOnGrid[i].data.attackRange > 0 && unitsOnGrid[i].data.rangedSpeed > 0)
					{
						u = i;
					}
					break;
				}
			}
			if(u >= 0)
			{
				for (int i = 0; i < buildingsOnGrid.Count; i++)
				{
					if (buildingsOnGrid[i].building.databaseID == target)
					{
						b = i;
						break;
					}
				}
				if(b >= 0)
				{
					UI_Projectile projectile = Instantiate(projectilePrefab);
					projectile.Initialize(unitsOnGrid[u].transform.position + Vector3.up * 0.1f, buildingsOnGrid[b].building.transform, unitsOnGrid[u].data.rangedSpeed * Data.gridCellSize);
				}
			}
		}

		public void DungeonUnitAttackCallBack(long id, long target)
		{
			int u = -1;
			for (int i = 0; i < dungeonUnitsOnGrid.Count; i++)
			{
				if (dungeonUnitsOnGrid[i].data.databaseID == id)
				{
					if (dungeonUnitsOnGrid[i].data.attackRange > 0 && dungeonUnitsOnGrid[i].data.rangedSpeed > 0)
					{
						u = i;
					}
					break;
				}
			}
			if(u >= 0)
			{

					UI_Projectile projectile = Instantiate(projectilePrefab);
					projectile.Initialize(dungeonUnitsOnGrid[u].transform.position + Vector3.up * 0.1f, MainPlayer.MainPlayer.gameObject.transform ,dungeonUnitsOnGrid[u].data.rangedSpeed * Data.gridCellSize);

			}
		}

		public void UnitDiedCallBack(long id)
		{
			Debug.Log("UnitDiedCallBack called" + id + "|| " + "dungeonUnitsOnGrid.Count "+ dungeonUnitsOnGrid.Count);
			for (int i = 0; i < dungeonUnitsOnGrid.Count; i++)
			{
				Debug.Log("dungeonUnitsOnGrid databaseID:" + dungeonUnitsOnGrid[i].databaseID );
				if(dungeonUnitsOnGrid[i].data.databaseID == id)
				{
					Destroy(dungeonUnitsOnGrid[i].healthBar.gameObject);
					Destroy(dungeonUnitsOnGrid[i].gameObject);
					dungeonUnitsOnGrid.RemoveAt(i);
					Debug.Log("Unit" + id + " diededed");
					break;
				}
			}
		}

		public void UnitDamageCallBack(long id, float damage)
		{
			Debug.Log("UnitDamageCallBack");
		}
		public void DungeonUnitDamageCallBack(long id, float damage)
		{
			Debug.Log("DUngeonUnitDamageCallBack");
		}

		public void UnitHealCallBack(long id, float health)
		{

		}

		public void BuildingAttackCallBack(long id, long target)
		{
			int u = -1;
			int b = -1;
			for (int i = 0; i < buildingsOnGrid.Count; i++)
			{
				if (buildingsOnGrid[i].id == id)
				{
					if (buildingsOnGrid[i].building.data.radius > 0 && buildingsOnGrid[i].building.data.rangedSpeed > 0)
					{
						b = i;
					}
					break;
				}
			}
			if (b >= 0)
			{
				for (int i = 0; i < unitsOnGrid.Count; i++)
				{
					if (unitsOnGrid[i].databaseID == target)
					{
						u = i;
						break;
					}
				}
				if (u >= 0)
				{
					UI_Projectile projectile = Instantiate(projectilePrefab);
					projectile.Initialize(buildingsOnGrid[b].building.transform.position + Vector3.up * 0.1f, unitsOnGrid[u].transform, buildingsOnGrid[b].building.data.rangedSpeed * Data.gridCellSize);
				}
			}
		}

		public void BuildingDamageCallBack(long id, float damage)
		{
			UpdateLoots();
		}

		public void BuildingDestroyedCallBack(long id, double percentage)
		{
			if(percentage > 0)
			{
				_percentageText.text = (battle.percentage * 100f).ToString("F2") + "%";
			}
			for (int i = 0; i < buildingsOnGrid.Count; i++)
			{
				if (buildingsOnGrid[i].id == id)
				{
					Destroy(buildingsOnGrid[i].healthBar.gameObject);
					Destroy(buildingsOnGrid[i].building.gameObject);
					buildingsOnGrid.RemoveAt(i);
					break;
				}
			}
		}
		#endregion

	}
}