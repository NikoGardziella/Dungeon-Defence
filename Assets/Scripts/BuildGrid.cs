namespace DungeonDefence
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class BuildGrid : MonoBehaviour
	{
		// SIZE OF THE GRID
		private static BuildGrid _instance = null; public static BuildGrid instance { get { return _instance; } }

		public int _rows = GameConstants._ROWS;
		public int _columns = GameConstants._COLUMNS;
		private float _cellSize = 1.0f;
		public float cellSize { get { return _cellSize; } }

	
		void Start()
		{
			_rows = GameConstants._ROWS;
			_columns = GameConstants._COLUMNS;
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
