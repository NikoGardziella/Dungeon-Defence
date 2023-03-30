namespace  DungeonDefence
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.InputSystem;

	public class UI_Player : MonoBehaviour
	{
		private static UI_Player _instance = null; public static UI_Player instance { get { return _instance; } }

		private Control _inputs = null;
		private bool _moving = false;
		[SerializeField] private float movementSpeed = 10f;
		public Vector2 move;

		private int _columns = 45;
		private int _rows = 45;
		private float _cellSize = 1;

		private int _X = 0;
		private int _Y = 0;

		private int _originalX = 0;
		private int _originalY = 0;

		private float _GridX = 0; public float GridX { get { return _GridX; } }
		private float _GridY = 0; public float GridY { get { return _GridY; } }
		void Update()
		{
			MovePlayer();
		}

	

		public List<Building> buildings = new List<Building>();
		public List<Building> _buildings { get { return _buildings; } set  {buildings = value;  } }
		private char[] GridStr;

		

		public void OnMove(InputAction.CallbackContext context)
		{
			move = context.ReadValue<Vector2>();
		}

		public void MovePlayer()
		{
			
			Vector3 movement = new Vector3(move.x, 0f, move.y);
			if(movement != Vector3.zero)
			{
				if(CanMove(GridToWorldPosition(transform.position.x, transform.position.z)))
				{
					if(CheckTile(GridToWorldPosition(transform.position.x, transform.position.z)) == 2)
						Debug.Log("player hit trap");
					transform.Translate(movement * movementSpeed * Time.deltaTime, Space.World);
				}
				else
				{
					//movementSpeed = -0.5f;
				}
				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15f);
			}
		}

		Vector2 SetGridCoord(Vector2 position)
		{
			position.x = (position.x + (_columns / 2));
			position.y = (position.y + (_rows / 2));
			return position;
		}

		public int CheckTile(Vector2 position)
		{
			Vector3 tempPos;
			int result;
			result = 0;
			tempPos = UI_Main.instance._grid.transform.InverseTransformPoint(new Vector3(position.x,0, position.y)); 
			position.x = tempPos.x;
			position.y = tempPos.z;
			Debug.Log("playerx: " + position.x  + " playerY: " + position.y);
			for (int x = 0; x < 45 + 1; x++)
			{
				for (int y = 0; y < 45 + 1; y++)
				{
					if(UI_Battle.instance.CollisionGrid[x,y] == 2)
						Debug.Log("trapX" + x + "trapY: " + y);
				}
			}
			if(UI_Battle.instance.CollisionGrid[(int)position.x, (int)position.y] == 2)
				result = 2;
			return result;
		}

	
		public bool CanMove(Vector2 position)
		{
			int vecX = (int)move.x;
			int vecY = (int)move.y;

			Vector3 tempPos;
			tempPos = UI_Main.instance._grid.transform.InverseTransformPoint(new Vector3(position.x,0, position.y)); 
			position.x = tempPos.x + 1;
			position.y = tempPos.z + 1;
			int x = (int)(position.x);
			int y = (int)(position.y);
			//Debug.Log("x:" +x + "  y:" + y);

			if(move.x > 0 && move.y < 0) // v>
			{
				if(y - 1 < 0)
					return false;
				
				else if(UI_Battle.instance.CollisionGrid[x, y - 1] == 1)
					return false;
			}
			if (move.x == 0 && move.y < 0) // v
			{
				if(y - 1 < 0)
					return false;
				if(UI_Battle.instance.CollisionGrid[x - 1, y - 1] == 1)
					return false;			
			}
			if(move.x < 0 && move.y < 0) // <v
			{
				if(UI_Battle.instance.CollisionGrid[x - 1, y] == 1)
					return false;	
			}
			if(move.x < 0 && move.y == 0) // <
			{
				if(UI_Battle.instance.CollisionGrid[x - 1, y + 1] == 1)
					return false;	
			}

			if(move.x < 0 && move.y > 0) // <^
			{
				if(UI_Battle.instance.CollisionGrid[x, y + 1] == 1)
					return false;	
			}
			if(move.x == 0 && move.y > 0) // ^
			{
				if(UI_Battle.instance.CollisionGrid[x + 1, y + 1] == 1)
					return false;	
			}
			if(move.x > 0 && move.y > 0) // ^>
			{
				if(UI_Battle.instance.CollisionGrid[x + 1, y] == 1)
					return false;	
			}
			if(move.x > 0 && move.y == 0) // >
			{
				if(y - 1 < 0)
					return false;
				if(UI_Battle.instance.CollisionGrid[x + 1, y - 1] == 1)
					return false;		
			}
			
			
		
		


		/*	if(x - move.x > 0 && y + move.y > 0)
				return true;
			else if(move.x < 0 && y + move.y < 0)
				return true;
			else if(y + move.y < 0)
					return false;
			else if(y - move.x < 0)
					return false; */

		//	if(UI_Battle.instance.CollisionGrid[x + vecX, y + vecY] == 1)
		//				return false;


		/*	if(move.x > 0 && move.y < 0)
			{
				if(y - 1 < 0)
					return false;
			}
			else if(move.x > 0)
			{
				if(y - 1 < 0)
					return false;
			}
			else if(move.y < 0)
			{
				if(y - 1 < 0)
					return false;
			} */


	/*		if(move.x > 0 || move.y > 0)
			{			
				if(position.x + 1 > _rows || position.y + 1 > _columns)
					return false;
				if(move.y > 0)
				{
					if(UI_Battle.instance.CollisionGrid[x + 1, y + 1] == 1)
						return false;
				}				
				//else if(UI_Battle.instance.CollisionGrid[x + 1, y + 1] == 1)
				//	return false;
			}

			if(move.x < 0 || move.y < 0)
			{
				if(position.x - 1 < 0 || position.y - 1 < 0)
					return false;

				if(move.y < 0)
				{
					if(UI_Battle.instance.CollisionGrid[x + 1, y + 1] == 1)
						return false;					
				}				
			//	else if(UI_Battle.instance.CollisionGrid[x - 1, y] == 1)
			//		return false;
			}

			if(move.y > 0)
			{
				if(position.y > _columns)
					return false;
				else if(UI_Battle.instance.CollisionGrid[x, y + 1] == 1)
					return false;
			}
			else if(move.y < 0)
			{
				if(position.y < 0)
					return false;
				else if(UI_Battle.instance.CollisionGrid[x, y - 1] == 1)
					return false;
			}				*/
				
			
			
		

		/* //This was by was the worst
			Vector3 tempPos;
			tempPos = UI_Main.instance._grid.transform.InverseTransformPoint(new Vector3(position.x,0, position.y)); 
			position.x = tempPos.x + 1;
			position.y = tempPos.z + 1;
			for (int i = 0; i < UI_Battle.instance.buildingsOnGrid.Count; i++)
			{
				
					Rect rect1 = new Rect(UI_Battle.instance.buildingsOnGrid[i].building.currentX, UI_Battle.instance.buildingsOnGrid[i].building.currentX, UI_Battle.instance.buildingsOnGrid[i].building.rows, UI_Battle.instance.buildingsOnGrid[i].building.columns);				
					Rect rect2 = new Rect(position.x, position.y, 1, 1);
					if(rect2.Overlaps(rect1))
					{
						return false;
					}
			} */

			return true;
		}
 		private static Vector2 GridToWorldPosition(float x, float y)
        {
            return new Vector2(x * Data.gridCellSize + Data.gridCellSize / 2f,y * Data.gridCellSize + Data.gridCellSize / 2f);
        }

		public void UpdateGridPosition(Vector3 basePosition, Vector3 currentPosition)
		{
			Vector3 dir = UI_Main.instance._grid.transform.TransformPoint(currentPosition) - UI_Main.instance._grid.transform.TransformPoint(basePosition);
			int xDis = Mathf.RoundToInt(dir.z / UI_Main.instance._grid.cellSize);
			int yDis = Mathf.RoundToInt(-dir.x / UI_Main.instance._grid.cellSize);

			_GridX = transform.position.x + xDis;
			_GridY = _Y + yDis;
		
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
	}
}