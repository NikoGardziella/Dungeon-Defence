namespace  DungeonDefence
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.InputSystem;
	

    public class UI_Player : MonoBehaviour
	{
		public Animator anim;
		private static UI_Player _instance = null; public static UI_Player instance { get { return _instance; } }
		public List<Data.Building> buildings = new List<Data.Building>();
		public List<UI_Battle.BuildingOnGrid> buildingOnGrid = new List<UI_Battle.BuildingOnGrid>();
		public GameObject MeleeWeapon;

		//private Control _inputsLeft = null;
		//private Control _inputsRight = null;

		private bool _moving = false;
		[SerializeField] private float movementSpeed = 10f;
		public Vector2 move;
		public Vector2 attackDir;
		private Vector3 attackVector;


		private int _columns = GameConstants._COLUMNS;
		private int _rows = GameConstants._COLUMNS;
		private float _cellSize = 1;

		private int _X = 0;
		private int _Y = 0;

		private int _originalX = 0;
		private int _originalY = 0;

		private float _GridX = 0; public float GridX { get { return _GridX; } }
		private float _GridY = 0; public float GridY { get { return _GridY; } }
		private float _WeaponGridX = 0; public float WeaponGridX { get { return _WeaponGridX; } }
		private float _WeaponGridY = 0; public float WeaponGridY { get { return _WeaponGridY; } }
		private float _WorldX = 0; public float WorldX { get { return _WorldX; } }
		private float _WorldY = 0; public float WorldY { get { return _WorldY; } }
		public Vector2 PlayerGridPosition;

		void Update()
		{
			
			UpdatePlayerActions();
		}

		void Start()
		{
			Vector3 tempPos;
			tempPos = UI_Main.instance._grid.transform.InverseTransformPoint(new Vector3(gameObject.transform.position.x,0, gameObject.transform.position.y)); 
			_WorldX = gameObject.transform.position.x;
			_WorldY = gameObject.transform.position.z;
			_GridX = tempPos.x;
			_GridY = tempPos.z;
			_columns = GameConstants._COLUMNS;
			_rows = GameConstants._COLUMNS;
		}

	

	//	public List<Building> buildings = new List<Building>();
	//	public List<Building> _buildings { get { return _buildings; } set  {buildings = value;  } }
		//private char[] GridStr;		

		public void OnMove(InputAction.CallbackContext context)
		{
			move = context.ReadValue<Vector2>();
		}

		public void OnAttack(InputAction.CallbackContext context)
		{
			attackDir = context.ReadValue<Vector2>();
		}



		public bool PlayerIsAttacking;
		void UpdatePlayerActions()
		{
			MovePlayer();
			AttackDirection();
			//PlayerAnimations();
			UpdateGridPos();			
		}

		public void PlayerAnimations()
		{
			if(Input.GetKeyDown(KeyCode.Space))
			{
				anim.SetTrigger("Attack");
				PlayerIsAttacking = true;
			}
			else
			{
				anim.SetTrigger("Idle");
				PlayerIsAttacking = false;
			}
		}

		public void AttackDirection()
		{
			//attackVector = Vector3.forward;
			//attackVector = (Vector3.up * attackDir.y + Vector3.left * attackDir.x);
			if(attackDir != Vector2.zero)
			{
				//attackVector.x = attackDir.x;
				//attackVector.z = attackDir.y;
				PlayerIsAttacking = true;
				anim.SetTrigger("Attack");
				MeleeWeapon.transform.rotation = Quaternion.LookRotation(new Vector3(attackDir.x, 0, attackDir.y),Vector3.up);
			}
			else
			{
				anim.SetTrigger("Idle");
				PlayerIsAttacking = false;
			}
			//MeleeWeapon.transform.Rotate(new Vector3(0, attackDir.x ,0));
			//MeleeWeapon.transform.LookAt(MeleeWeapon.transform, new Vector3(attackDir.x, 0,attackDir.y));
			//MeleeWeapon.transform.RotateAround(MeleeWeapon.transform.position, MeleeWeapon.transform.up  , Time.deltaTime *  150f * attackDir.y);
		}

		public void MovePlayer()
		{			
			Vector3 movement = new Vector3(move.x, 0f, move.y);
			if(movement != Vector3.zero)
			{
			/*	if(CanMove(GridToWorldPosition(transform.position.x, transform.position.z)))
				{
					if(CheckTile(GridToWorldPosition(transform.position.x, transform.position.z)) == 2)
						Debug.Log("player hit trap");
				//	transform.Translate(movement * movementSpeed * Time.deltaTime, Space.World);
					movementSpeed = 10.0f;		
					_WorldX = gameObject.transform.position.x;
					_WorldY = gameObject.transform.position.z;
				}
				else
				{
					movementSpeed = 0.0f;
				} */
				/*PlayerGridPosition = WorldToGridPosition(GridToWorldPosition(transform.position.x, transform.position.z));
				if(CheckCollisionBoxes(GetBuilding(PlayerGridPosition),PlayerGridPosition))
				{
					movementSpeed = 10.0f;
				}
				else
					movementSpeed = 0.0f; */
				
				transform.Translate(movement * movementSpeed * Time.deltaTime, Space.World);
				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15f);
			}
			
		}

		private void UpdateGridPos()
		{
			Vector3 tempPos;
			tempPos = UI_Main.instance._grid.transform.InverseTransformPoint(new Vector3(gameObject.transform.position.x,0, gameObject.transform.position.z)); 
			_GridX = tempPos.x;
			_GridY = tempPos.z;
			tempPos = UI_Main.instance._grid.transform.InverseTransformPoint(new Vector3(MeleeWeapon.transform.position.x,0, MeleeWeapon.transform.position.z)); 
			_WeaponGridX = tempPos.x;
			_WeaponGridY = tempPos.z;
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
			
		//	Debug.Log("playerx: " + position.x  + " playerY: " + position.y);
			for (int x = 0; x < 45 + 1; x++)
			{
				for (int y = 0; y < 45 + 1; y++)
				{
					//if(UI_Battle.instance.CollisionGrid[x,y] == 2)
						//Debug.Log("trapX" + x + "trapY: " + y);
				}
			}
			if(UI_Battle.instance.CollisionGrid[(int)position.x, (int)position.y] == 2)
				result = 2;
			return result;
		}

		
		/* Just tried to improve collisions. This can probably be removed
			public float PlayerColliderSize = 0.1f;
			public float BuildingColliderSize = 0.5f;

			public float PlayerTopLeft = 0.5f;
			public float PlayerTopRight = 0.5f;
			public float PlayerDownLeft = 0.5f;
			public float PlayerDownRight = 0.5f;
			public float BuildingTopLeft = 0.5f;
			public float BuildingTopRight = 0.5f;
			public float BuildingDownLeft = 0.5f;
			public float BuildingDownRight = 0.5f; 
		
		

		public bool CheckCollisionBoxes(UI_Battle.BuildingOnGrid building ,Vector2 PlayerPos)
		{
			if(building == null)
				return true;
			Debug.Log("building id" + building.id +" || " + "pos :X" + building.building.data.warX + " pos Y: "  + building.building.data.warY);
			PlayerTopLeft = (float)PlayerPos.y + PlayerColliderSize;
			PlayerTopRight = (float)PlayerPos.y + PlayerColliderSize;
			PlayerDownLeft = (float)PlayerPos.x - PlayerColliderSize;
			PlayerDownRight = (float)PlayerPos.x - PlayerColliderSize;
			BuildingTopLeft =  (float)building.building.data.warY + BuildingColliderSize;
			BuildingTopRight = (float)building.building.data.warY + BuildingColliderSize;
			BuildingDownLeft = (float)building.building.data.warX - BuildingColliderSize;
			BuildingDownRight = (float)building.building.data.warX - BuildingColliderSize;

			if(PlayerTopLeft < BuildingTopLeft || PlayerTopRight < BuildingTopRight || PlayerDownLeft > BuildingDownLeft || PlayerDownRight > BuildingDownRight)
				return false;			
			return true;
		} */

		public Vector2 WorldToGridPosition(Vector2 position)
		{
			Vector3 tempPos;
			Vector2 result;
			tempPos = UI_Main.instance._grid.transform.InverseTransformPoint(new Vector3(position.x,0, position.y)); 
			result.x = tempPos.x + 1;
			result.y = tempPos.z + 1;
			return result;
		}

		public bool CanMove(Vector2 position)
		{

			int vecX = (int)move.x;
			int vecY = (int)move.y;

			Vector3 tempPos;
			tempPos = UI_Main.instance._grid.transform.InverseTransformPoint(new Vector3(position.x,0, position.y)); 
			_GridX = tempPos.x;
			_GridY = tempPos.z;			
			position.x = tempPos.x + 1f;
			position.y = tempPos.z + 1;
			int x = (int)(position.x);
			int y = (int)(position.y);
			//Debug.Log("x:" +x + "  y:" + y);

			
			if(move.x > 0 && move.y < 0) // v>
			{
				if(y - 1 < 0)
					return false;
				
				else if(UI_Battle.instance.CollisionGrid[x * GameConstants._COLLISION_GRID_PRECISION, y * GameConstants._COLLISION_GRID_PRECISION - 1] == 1 && UI_Battle.instance.CollisionGrid[x * GameConstants._COLLISION_GRID_PRECISION, y * GameConstants._COLLISION_GRID_PRECISION - 19] == 1)
					return false;
			}
			if (move.x == 0 && move.y < 0) // v
			{
				if(y - 1 < 0 || x - 1 < 0)
					return false;
				if(UI_Battle.instance.CollisionGrid[x * GameConstants._COLLISION_GRID_PRECISION - 1, y * GameConstants._COLLISION_GRID_PRECISION - 1] == 1 && UI_Battle.instance.CollisionGrid[x * GameConstants._COLLISION_GRID_PRECISION - 19, y * GameConstants._COLLISION_GRID_PRECISION - 19] == 1)
					return false;			
			}
			if(move.x < 0 && move.y < 0) // <v
			{
				if(x - 1 < 0)
					return false;
				if(UI_Battle.instance.CollisionGrid[x * GameConstants._COLLISION_GRID_PRECISION - 1, y * GameConstants._COLLISION_GRID_PRECISION] == 1 && UI_Battle.instance.CollisionGrid[x * GameConstants._COLLISION_GRID_PRECISION - 19, y * GameConstants._COLLISION_GRID_PRECISION] == 1)
					return false;	
			}
			if(move.x < 0 && move.y == 0) // <
			{
				if(x - 1 < 0)
					return false;
				if(UI_Battle.instance.CollisionGrid[x * GameConstants._COLLISION_GRID_PRECISION - 1, y * GameConstants._COLLISION_GRID_PRECISION + 1] == 1 && UI_Battle.instance.CollisionGrid[x * GameConstants._COLLISION_GRID_PRECISION - 19, y * GameConstants._COLLISION_GRID_PRECISION + 19] == 1 )
					return false;	
			}

			if(move.x < 0 && move.y > 0) // <^
			{
				if(y + 1 > _rows)
					return false;
				if(UI_Battle.instance.CollisionGrid[x * GameConstants._COLLISION_GRID_PRECISION, y * GameConstants._COLLISION_GRID_PRECISION + 1] == 1 && UI_Battle.instance.CollisionGrid[x * GameConstants._COLLISION_GRID_PRECISION, y * GameConstants._COLLISION_GRID_PRECISION + 19] == 1)
					return false;	
			}
			if(move.x == 0 && move.y > 0) // ^
			{
				if(y + 1 > _rows)
					return false;
				if(UI_Battle.instance.CollisionGrid[x * GameConstants._COLLISION_GRID_PRECISION + 1, y * GameConstants._COLLISION_GRID_PRECISION + 1] == 1 && UI_Battle.instance.CollisionGrid[x * GameConstants._COLLISION_GRID_PRECISION + 19, y * GameConstants._COLLISION_GRID_PRECISION + 19] == 1 )
					return false;	
			}
			if(move.x > 0 && move.y > 0) // ^>
			{
				if(x + 1 > _columns)
					return false;
				if(UI_Battle.instance.CollisionGrid[x * GameConstants._COLLISION_GRID_PRECISION + 1, y * GameConstants._COLLISION_GRID_PRECISION] == 1 && UI_Battle.instance.CollisionGrid[x * GameConstants._COLLISION_GRID_PRECISION + 19, y * GameConstants._COLLISION_GRID_PRECISION] == 1)
					return false;	
			}
			if(move.x > 0 && move.y == 0) // >
			{
				if(y - 1 < 0)
					return false;
				if(UI_Battle.instance.CollisionGrid[x * GameConstants._COLLISION_GRID_PRECISION + 1, y * GameConstants._COLLISION_GRID_PRECISION - 1] == 1 && UI_Battle.instance.CollisionGrid[x * GameConstants._COLLISION_GRID_PRECISION + 10, y * GameConstants._COLLISION_GRID_PRECISION - 10] == 1)
					return false;		
			}
			return true;
		}

		public UI_Battle.BuildingOnGrid GetBuilding(Vector2 position)
		{

			//Debug.Log("building count in Player:" + buildings.Count);
			int x = (int)(position.x);
			int y = (int)(position.y);
			Debug.Log("x:" +x + "  y:" + y);

			int buildingIndex;
			if(move.x > 0 && move.y < 0) // v>
			{
				buildingIndex = UI_Battle.instance.CollisionGrid[x * GameConstants._COLLISION_GRID_PRECISION, y * GameConstants._COLLISION_GRID_PRECISION - 1];
				Debug.Log("building index:" + buildingIndex);
				if(buildingIndex != 0)
					return buildingOnGrid[buildingIndex];
			}
			if (move.x == 0 && move.y < 0) // v
			{
				buildingIndex = UI_Battle.instance.CollisionGrid[x * GameConstants._COLLISION_GRID_PRECISION - 1, y * GameConstants._COLLISION_GRID_PRECISION - 1];
				Debug.Log("building index:" + buildingIndex);
				if(buildingIndex != 0)
					return buildingOnGrid[buildingIndex];			
			}
			if(move.x < 0 && move.y < 0) // <v
			{
				buildingIndex = UI_Battle.instance.CollisionGrid[x * GameConstants._COLLISION_GRID_PRECISION - 1, y * GameConstants._COLLISION_GRID_PRECISION];
				Debug.Log("building index:" + buildingIndex);

				if(buildingIndex != 0)
					return buildingOnGrid[buildingIndex];	
			}
			if(move.x < 0 && move.y == 0) // <
			{
				
				buildingIndex = UI_Battle.instance.CollisionGrid[x * GameConstants._COLLISION_GRID_PRECISION - 1, y * GameConstants._COLLISION_GRID_PRECISION + 1];
				Debug.Log("building index:" + buildingIndex);
				if(buildingIndex != 0)
					return buildingOnGrid[buildingIndex];	
			}

			if(move.x < 0 && move.y > 0) // <^
			{
				buildingIndex = UI_Battle.instance.CollisionGrid[x * GameConstants._COLLISION_GRID_PRECISION, y * GameConstants._COLLISION_GRID_PRECISION + 1];
				Debug.Log("building index:" + buildingIndex);
				if(buildingIndex != 0)
					return buildingOnGrid[buildingIndex];	
			}
			if(move.x == 0 && move.y > 0) // ^
			{
				buildingIndex = UI_Battle.instance.CollisionGrid[x * GameConstants._COLLISION_GRID_PRECISION + 1, y * GameConstants._COLLISION_GRID_PRECISION + 1];
				Debug.Log("building index:" + buildingIndex);
				if(buildingIndex != 0)
					return buildingOnGrid[buildingIndex];	
			}
			if(move.x > 0 && move.y > 0) // ^>
			{
				buildingIndex = UI_Battle.instance.CollisionGrid[x * GameConstants._COLLISION_GRID_PRECISION + 1, y * GameConstants._COLLISION_GRID_PRECISION];
				Debug.Log("building index:" + buildingIndex);

				if(buildingIndex != 0)
					return buildingOnGrid[buildingIndex];	
			}
			if(move.x > 0 && move.y == 0) // >
			{
				buildingIndex = UI_Battle.instance.CollisionGrid[x * GameConstants._COLLISION_GRID_PRECISION + 1, y * GameConstants._COLLISION_GRID_PRECISION - 1];
				Debug.Log("building index:" + buildingIndex);

				if(buildingIndex != 0)
					return buildingOnGrid[buildingIndex];		
			} 
			return null;
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