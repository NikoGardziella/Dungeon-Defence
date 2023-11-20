
namespace DungeonDefence
{
	using System.Collections;
	using System.Collections.Generic;
    using System.Linq;
    using Pathfinding;
    using UnityEngine;

    public class BuildGrid : MonoBehaviour
	{
		// SIZE OF THE GRID
		private static BuildGrid _instance = null; public static BuildGrid instance { get { return _instance; } }

		public int _rows = GameConstants._ROWS;
		public int _columns = GameConstants._COLUMNS;
		private float _cellSize = 1.0f;
		public float cellSize { get { return _cellSize; } }
		private Pathfinding.Grid unlimitedGrid = null;
		private AStarSearch search = null;
		private Pathfinding.Grid grid = null;
		private AStarSearch unlimitedSearch = null;


	
		void Start()
		{
			_rows = GameConstants._ROWS;
			_columns = GameConstants._COLUMNS;
			unlimitedGrid = new Pathfinding.Grid(Data.gridSize + (Data.battleGridOffset * 2), Data.gridSize + (Data.battleGridOffset * 2));
			unlimitedSearch = new AStarSearch(unlimitedGrid);
			grid = new Pathfinding.Grid(Data.gridSize + (Data.battleGridOffset * 2), Data.gridSize + (Data.battleGridOffset * 2));
			search = new AStarSearch(grid);
		
		}

		private void SetBlockedTiles(int oldX, int oldY, int newX, int newY)
		{
			Debug.Log("set blockedTiles count: " + buildings.Count);
			for (int i = 0; i < buildings.Count; i++)
			{
				if(buildings[i].data.warX > 0 && buildings[i].data.warY > 0)
				{
					if(buildings[i].data.id == Data.BuildingID.dungeonwall)
					{
						Ui_Debug.instance.DrawBlockedTiles(buildings[i].data.warX, buildings[i].data.warY);

						grid[buildings[i].data.warX + (Data.battleGridOffset ), buildings[i].data.warY + (Data.battleGridOffset)].Blocked = true;
					}

				}
				//search.AddBlockedTiles(new Pathfinding.Vector2Int(buildings[i].data.warX, buildings[i].data.warY));
			}
			for (int i = 0; i < CameraController.instance.brushBuildings.brushBuildings.Count; i++)
			{
				Ui_Debug.instance.DrawBlockedTiles( CameraController.instance.brushBuildings.brushBuildings[i].x,  CameraController.instance.brushBuildings.brushBuildings[i].y);
				grid[CameraController.instance.brushBuildings.brushBuildings[i].x + (Data.battleGridOffset ), CameraController.instance.brushBuildings.brushBuildings[i].y + (Data.battleGridOffset)].Blocked = true;
			
				//search.AddBlockedTiles(new Pathfinding.Vector2Int(buildings[i].data.warX, buildings[i].data.warY));
			}
			// some buildings are not being checked

			Ui_Debug.instance.DrawBlockedTiles(newX, newY);

			grid[oldX  + (Data.battleGridOffset ),oldY + (Data.battleGridOffset )].Blocked = false;
			grid[newX + (Data.battleGridOffset ),newY + (Data.battleGridOffset )].Blocked = true;
		}

		private void ClearBlockedTiles()
		{
			for (int x = 0; x < _rows; x++)
			{
				for (int y = 0; y < _columns; y++)
				{
					grid[x,y].Blocked = false;
					
				}
			}
		}

		public List<Building> buildings = new List<Building>();

		public Building GetBuilding(long databaseID)
		{
			for (int i = 0; i < buildings.Count; i++)
			{
				if(buildings[i].databaseID == databaseID)
				{
					return buildings[i];
				}
			}
			return null;
		}
		public List<Unit> units = new List<Unit>();		

		public Unit GetUnit(long databaseID)
		{
			for (int i = 0; i < units.Count; i++)
			{
				if(units[i].databaseID == databaseID)
				{
					return units[i];
				}
			}
			return null;
		}

		public Vector3 GetStartPosition(int x, int y)
		{
			Vector3 position = transform.position;
			position += (transform.right.normalized * x * _cellSize) + (transform.forward.normalized * y * _cellSize);
			return position;
		}

		public Vector3 GetCenterPosition(int x, int y, int rows, int columns)
		{
			Vector3 position = GetStartPosition(x,y);
			position += ((transform.right.normalized * columns * _cellSize) / 2.0f)+ (transform.forward.normalized * rows * _cellSize) / 2.0f;
			return position;
		}

		public bool IsWorldPositionIsOnPlane(Vector3 position, int x, int y, int rows, int columns)
		{			
			position = transform.InverseTransformPoint(position);
			Rect rect = new Rect(x,y,columns, rows);
			if(rect.Contains(new Vector2(position.x, position.z)))
			{
				return true;
			}
			return false;
		}
		public Vector3 GetEndPosition(int x, int y, int rows, int columns)
		{
			Vector3 position = GetStartPosition(x,y);
			position += (transform.right.normalized * columns * _cellSize) + (transform.forward.normalized * rows * _cellSize);
			return position;
		}
		public Vector3 GetEndPosition(Building building)
		{

			return GetEndPosition(building.currentX, building.currentY, building.rows, building.columns);
		}

		// add check with units



		public bool IsPathToPlayerStart(int oldX, int oldY, int newX, int newY)
		{
			
			//grid = new Pathfinding.Grid(Data.gridSize + (Data.battleGridOffset * 2), Data.gridSize + (Data.battleGridOffset * 2));

			if(Ui_Debug.instance.IsDebugging)
			{
				Ui_Debug.instance.ClearDebugTiles();
			}
			Path path = new Path();
			Pathfinding.Vector2Int start;
			start.X = 0;
			start.Y = 0;
			Pathfinding.Vector2Int end;
			end.X = 25;
			end.Y = 25;

		//	List<Cell> points = search.Find(start, end).ToList();
			ClearBlockedTiles();
			SetBlockedTiles(oldX, oldY, newX, newY);
			if(!path.Create(ref search, new BattleVector2Int(1, 1), new BattleVector2Int(25, 25)))
			{
				path = null;
				Debug.Log("Path is invalid");
				return false;
			}
			//Debug.Log("Path length" + path.length + " ||  x: " + x + " y " + y  + " block count " + path.blocks.Count);
			return true;
		}

		public bool CanPlaceBuilding(Building building, int x, int y)
		{

		
			if(building.currentX < 0 || building.currentY < 0 || building.currentX + building.columns > _columns || building.currentY + building.rows > _rows)
			{
				return false;
			}
			for (int i = 0; i < buildings.Count; i++)
			{
				if(buildings[i] != building)
				{
					Rect rect1 = new Rect(buildings[i].currentX, buildings[i].currentY,buildings[i].columns, buildings[i].rows);				
					Rect rect2 = new Rect(building.currentX, building.currentY,building.columns, building.rows);
					if(rect2.Overlaps(rect1))
					{
						return false;
					}

				}
			}
			return true;
		}

		public bool CanPlaceCoordinates(int x, int y)
		{
			if(x < 0 || y < 0 || x > _columns || y > _rows)
			{
				return false;
			}
			for (int i = 0; i < buildings.Count; i++)
			{

				Rect rect1 = new Rect(buildings[i].currentX, buildings[i].currentY,buildings[i].columns, buildings[i].rows);				
				Rect rect2 = new Rect(x, y,1, 1);
				if(rect2.Overlaps(rect1))
				{
					return false;
				}

			}
			for (int i = 0; i < CameraController.instance.brushBuildings.brushBuildings.Count ; i++)
			{
				Rect rect1 = new Rect(CameraController.instance.brushBuildings.brushBuildings[i].x, CameraController.instance.brushBuildings.brushBuildings[i].y, 1, 1);				
				Rect rect2 = new Rect(x, y,1, 1);
				if(rect2.Overlaps(rect1))
				{
					return false;
				}
			}
			return true;
		}

		// Checks only against buildings not Units
		public bool CanPlaceUnit(Unit unit, int x, int y)
		{
			if(unit.currentX < 0 || unit.currentY < 0 || unit.currentX > _columns || unit.currentY > _rows)
			{
				return false;
			}
			for (int i = 0; i < buildings.Count; i++)
			{
				if(buildings[i] != unit)
				{
					Rect rect1 = new Rect(buildings[i].currentX, buildings[i].currentY,buildings[i].columns, buildings[i].rows);				
					Rect rect2 = new Rect(unit.currentX, unit.currentY, 1, 1);
					if(rect2.Overlaps(rect1))
					{
						return false;
					}

				}
			}
			return true;
		}


		public void Clear()
		{
			for (int i = 0; i < buildings.Count; i++)
			{
				if(buildings[i])
				{
					Destroy(buildings[i].gameObject);
				}
			}
			buildings.Clear();
		}
		public void ClearUnits()
		{
			for (int i = 0; i < units.Count; i++)
			{
				if(units[i])
				{
					Destroy(units[i].gameObject);
				}
			}
			units.Clear();
		}

			public void ClearDungeonUnits()
		{
			for (int i = 0; i < units.Count; i++)
			{
				if(units[i])
				{
					Destroy(units[i].gameObject);
				}
			}
			units.Clear();
		}


		
		

		public class Path
		{

			public Path()
			{
				length = 0;
				points = null;
				blocks = new List<Tile>();
			}
			public bool Create(ref AStarSearch search, BattleVector2Int start, BattleVector2Int end)
			{
				
				points = search.Find(new Pathfinding.Vector2Int(start.x, start.y), new Pathfinding.Vector2Int(end.x, end.y)).ToList();
				Ui_Debug.instance.DrawPath(points);
				
				if (!IsValid(ref points, new Pathfinding.Vector2Int(start.x, start.y), new Pathfinding.Vector2Int(end.x, end.y)))
				{
					points = null;
					return false;
				}
				else
				{
					this.start.x = start.x;
					this.start.y = start.y;
					this.end.x = end.x;
					this.end.y = end.y;
					return true;
				}
			}
			public static bool IsValid(ref List<Cell> points, Pathfinding.Vector2Int start, Pathfinding.Vector2Int end)
			{
				if(points == null)
					Debug.Log("points == null");
				if(!points.Any())
					Debug.Log("!points.Any()");
				if(!points.Last().Location.Equals(end))
				{

					Debug.Log("!points.Last().Location.Equals(end) new end: " + points.Last().Location.X + "  " + points.Last().Location.X);
				}
				if(!points.First().Location.Equals(start))
					Debug.Log("!points.First().Location.Equals(start)");
				Debug.Log("poins count" + points.Count);
				if (points == null || !points.Any() || !points.Last().Location.Equals(end) || !points.First().Location.Equals(start))
				{
					return false;
				}
				return true;
			}
			public BattleVector2Int start;
			public BattleVector2Int end;
			public List<Cell> points = null;
			public float length = 0;
			public List<Tile> blocks = null;
			// public float blocksHealth = 0;
		}
		public class Tile
		{
			public Tile(Data.BuildingID id, BattleVector2Int position, int index = -1)
			{
				this.id = id;
				this.position = position;
				this.index = index;
			}
			public Data.BuildingID id;
			public BattleVector2Int position;
			public int
			 index = -1;
		}

		public struct BattleVector2Int
		{
			public int x;
			public int y;

			public BattleVector2Int(int x, int y) { this.x = x; this.y = y; }
		}

		


		#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.white;
			for (int i = 0; i <= _rows; i++)
			{
				Vector3 point = transform.position + transform.forward.normalized * _cellSize *(float)i;
				Gizmos.DrawLine(point, point + transform.right.normalized * _cellSize * (float)_columns);
			}
			for (int i = 0; i <= _columns; i++)
			{
				Vector3 point = transform.position + transform.right.normalized * _cellSize *(float)i;
				Gizmos.DrawLine(point, point + transform.forward.normalized * _cellSize * (float)_rows);
			}
		}
		#endif
	}
}
