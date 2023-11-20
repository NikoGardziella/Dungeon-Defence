namespace DungeonDefence
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using DevelopersHub.RealtimeNetworking.Client;


    public class Building : MonoBehaviour
	{



		public Data.BuildingID id = Data.BuildingID.townhall;
		private static Building _buildInstance = null; public static Building buildInstance {get {return _buildInstance; } set { _buildInstance = value;} }
		private static Building _selectedInstance = null; public static Building selectedInstance {get {return _selectedInstance; } set { _selectedInstance = value;} }

		[HideInInspector]public Data.Building data = new Data.Building();
		[HideInInspector]public UI_Button collectButton = null;
		[HideInInspector]public bool collecting = false;
		[HideInInspector]public UI_Bar buildBar = null;
		


		[System.Serializable] public class Level
		{
			public int level = 1;
			public Sprite icon = null;
			public GameObject mesh = null;
		}

		private BuildGrid _grid = null;

		[SerializeField] private long _databaseID = 1; public long databaseID  { get { return _databaseID; } set { _databaseID = value; }  }

		[SerializeField] private int _columns = 1; public int columns { get { return _columns; } }
		[SerializeField] private int _rows = 1; public int rows { get { return _rows; } }
		[SerializeField] public MeshRenderer _baseArea = null;
		[SerializeField] public GameObject trapTrigger;
		[SerializeField] public GameObject trapProjectileStart;
		[SerializeField] private Level[] _levels = null;
		private int _currentX = 0; public int currentX { get { return _currentX; } set { currentX = value; } }
		private int _currentY = 0; public int currentY { get { return _currentY; } set { currentY = value; } }
		private int _X = 0;
		private int _Y = 0;

		private int _originalX = 0;
		private int _originalY = 0;
		private int _yRotation = 0; public int yRotation { get { return _yRotation; } set { yRotation = value; } }
		private int _size = 0; public int size { get { return _size; } set { size = value; } }

		[SerializeField] private float _BuildintTopLeft = 0.5f; public  float BuildintTopLeft { get { return _BuildintTopLeft; } }
		[SerializeField] private float _BuildintTopRight = 0.5f; public float BuildintTopRight { get { return _BuildintTopRight; } }
		[SerializeField] private float _BuildintDownLeft = 0.5f; public float BuildintDownLeft { get { return _BuildintDownLeft; } }
		[SerializeField] private float _BuildintDownRight = 0.5f; public float  BuildintDownRight { get { return _BuildintDownRight; } }

		private void Update()
		{
			AdjustUI();
		}

		private void OnDestroy()
		{
			if(buildBar)
			{
				Destroy(buildBar.gameObject);
			}
			if(collectButton)
			{
				Destroy(collectButton.gameObject);
			}
		}

		public void AdjustUI()
		{
			if(collectButton)
			{
				switch (id)
				{
					case Data.BuildingID.townhall:

					break;
					case Data.BuildingID.goldmine:
						if(data.goldStorage >= Data.minGoldCollect)
						{
							collectButton.gameObject.SetActive(!collecting && data.isConstructing == false && Player.instance.gold < Player.instance.maxGold);
						}
						else
						{
							collectButton.gameObject.SetActive(false);
						}
						break;
					case Data.BuildingID.goldstorage:
					break;
					case Data.BuildingID.elixirmine:
						if(data.elixirStorage >= Data.minElixirCollect)
						{
							collectButton.gameObject.SetActive(!collecting && data.isConstructing == false && Player.instance.elixir < Player.instance.maxElixir);
						}
						else
						{
							collectButton.gameObject.SetActive(false);
						}
					break;
						case Data.BuildingID.darkelixirmine:
						if(data.darkStorage >= Data.minDarkElixirCollect)
						{
							collectButton.gameObject.SetActive(!!collecting && data.isConstructing == false && Player.instance.darkElixir < Player.instance.maxDarkElixir);
						}
						else
						{
							collectButton.gameObject.SetActive(false);
						}
					break;					
				}

				Vector3 end = UI_Main.instance._grid.GetEndPosition(this);
				
				Vector3 planeDownLeft = CameraController.instance.planeDownLeft;
				Vector3 planeTopRight = CameraController.instance.planeTopRight;

				float w = planeTopRight.x - planeDownLeft.x;
				float h = planeTopRight.z - planeDownLeft.z;

				float endW = end.x - planeDownLeft.x;
				float endH = end.z - planeDownLeft.z;

				Vector2 screenPoint = new Vector2(endW / w * Screen.width, endH / h * Screen.height);
				collectButton.rect.anchoredPosition = screenPoint;
			}

			if(buildBar)
			{

			//	buildBar.AdjustUI();
				if(data.isConstructing)
				{
					System.TimeSpan span = data.constructionTime - Player.instance.data.nowTime;
					if(span.TotalDays > 1)
					{
						buildBar.texts[0].text = span.ToString(@"dd\:hh\:mm\:ss");
					}
					else
					{
						buildBar.texts[0].text = span.ToString(@"hh\:mm\:ss");

					}
					buildBar.bar.fillAmount = Mathf.Abs(1f - ((float)span.TotalSeconds / (float)data.buildTime));
					buildBar.gameObject.SetActive(true);
					Vector3 end = UI_Main.instance._grid.GetEndPosition(this);
				
					Vector3 planeDownLeft = CameraController.instance.planeDownLeft;
					Vector3 planeTopRight = CameraController.instance.planeTopRight;

					float w = planeTopRight.x - planeDownLeft.x;
					float h = planeTopRight.z - planeDownLeft.z;

					float endW = end.x - planeDownLeft.x;
					float endH = end.z - planeDownLeft.z;

					Vector2 screenPoint = new Vector2(endW / w * Screen.width, endH / h * Screen.height);
					buildBar.rect.anchoredPosition = screenPoint;
				}
				else
				{
					buildBar.gameObject.SetActive(false);
				}
			}
		}


		public void Collect()
		{
			collectButton.gameObject.SetActive(false);
			collecting = true;
			Packet packet = new Packet();
			packet.Write((int)Player.RequestId.COLLECT);
			packet.Write(data.databaseID);
			Sender.TCP_Send(packet); 
		}

		public void PlacedOnGrid(int x, int y)
		{
			_currentX = x;
			_currentY = y;
			_X = x;
			_Y = y;
			_originalX = x;
			_originalY = y;
			
			Vector3 position = UI_Main.instance._grid.GetCenterPosition(x,y, _rows, _columns);
			transform.position = position;
			SetbaseColor();
		}
		public void StartMovingOnGrid()
		{
			_X = _currentX;
			_Y = _currentY;
		}
		public void RemovedFromGrid()
		{
			_buildInstance = null;
			UI_Build.instance.SetStatus(false);
			CameraController.instance.isPlacingBuilding = false;
			/*
			for (int i = 0; i < UI_Main.instance._grid.buildings.Count; i++)
			{
				if(selectedInstance.databaseID == UI_Main.instance._grid.buildings[i].databaseID)
					UI_Main.instance._grid.buildings.Remove(UI_Main.instance._grid.buildings[i]);
			}
			*/
			Destroy(gameObject);
		}

		public void DeleteBuilding()
		{
			if(!waitinReplaceRepsonce)
			{
				waitinReplaceRepsonce = true;
				Packet packet = new Packet();
				packet.Write((int)Player.RequestId.DELETEBUILDING);
				packet.Write(selectedInstance.databaseID);
				packet.Write(selectedInstance.currentX);
				packet.Write(selectedInstance.currentY);
				packet.Write(UI_WarLayout.instance.isActive ? 2 : 1);
				Sender.TCP_Send(packet);
				
				
				Destroy(gameObject);
			}
		}
		

		public void MoveToNextOnGrid(int x, int y)
		{
			_currentX += x;
			_currentY += y;
			StartMovingOnGrid();
			//_originalX = currentX;
			//_originalY = currentY;
			//buildInstance._X += x;
			//buildInstance._Y  += y;
			gameObject.transform.position = UI_Main.instance._grid.GetCenterPosition(_currentX,_currentY, _rows, _columns);
			SetbaseColor();
		}

		public void IncreaseSize()
		{
			Vector3 changeVector;
			if(_yRotation == 90 || _yRotation == 270)
			{
				changeVector = new Vector3(0.35f,0,-0.35f);
				buildInstance._rows += 1;
			}
			else
			{
				changeVector = new Vector3(0.35f,0,0.35f);
				buildInstance._columns += 1;
			}
			_baseArea.transform.localScale += new Vector3(0,1,0);
			if(trapTrigger)
				trapTrigger.transform.position -= changeVector;
			if(trapProjectileStart)
				trapProjectileStart.transform.position += changeVector;
			_buildInstance._size += 1;
		}

		public void SetBuildingSize(int size)
		{
			Vector3 changeVector;
			if(size <= 0)
				size = 1;
			if(_yRotation == 90 || _yRotation == 270)
			{
				changeVector = new Vector3(0.35f * size,0,-0.35f * size);
			}
			else
			{
				changeVector = new Vector3(0.35f * size,0,0.35f * size);
			}
			_baseArea.transform.localScale += new Vector3(0,size,0);
			if(trapTrigger)
				trapTrigger.transform.position -= changeVector;
			if(trapProjectileStart)
				trapProjectileStart.transform.position += changeVector;
		}


		public void BuildingInitRotation(int yRotate)
		{
			gameObject.transform.RotateAround(transform.position, Vector3.up,yRotate);
			//gameObject.transform.Rotate(Vector3.up * yRotate,Space.Self);
			_yRotation = (int)gameObject.transform.localEulerAngles.y;
			gameObject.transform.position = UI_Main.instance._grid.GetCenterPosition(_currentX,_currentY, _rows, _columns);
			SetbaseColor();
		}

		public void RotateBuildingClockwise(int yRotate, Vector3 ButtonPosition)
		{
			int temp;
			Vector3 newPos;
			newPos = gameObject.transform.position;
			//gameObject.transform.localEulerAngles += new Vector3(0,yRotate,0);
			gameObject.transform.RotateAround(gameObject.transform.localPosition , Vector3.up, yRotate);
			//newPos = UI_Main.instance._grid.transform.InverseTransformPoint(new Vector3(gameObject.transform.position.x,0, gameObject.transform.position.z)); 
			temp = _rows;
			_rows = _columns;
			_columns = temp;
			StartMovingOnGrid();
			CameraController.instance.UpdateBuildingPosition(gameObject.transform.position);
			//UpdateGridPosition(newPos,gameObject.transform.position);
			//StartMovingOnGrid();
			//gameObject.transform.SetPositionAndRotation
			//gameObject.transform.Rotate(Vector3.up * yRotate,Space.World);y
			//transform.rotation = Quaternion.identity;
			//transform.RotateAround(transform.position + new Vector3(gameObject.transform.localScale.x / 2 , 0f, transform.localScale.z / 2), Vector3.up, yRotate);
			_yRotation = (int)gameObject.transform.localEulerAngles.y;
			Debug.Log("y_rotation:" + _yRotation + " " + gameObject.transform.localEulerAngles.y);
			//if(_yRotation == 90 || _yRotation == 270)
		//	{
				
		//}
			//_currentX = (int)newPos.x;
			//_currentY = (int)newPos.z;
			//gameObject.transform.position = UI_Main.instance._grid.GetCenterPosition(_currentX,_currentY, _rows, _columns);
			SetbaseColor();
		}

		

		public void UpdateGridPosition(Vector3 basePosition, Vector3 currentPosition)
		{			

			Vector3 dir = UI_Main.instance._grid.transform.TransformPoint(currentPosition) - UI_Main.instance._grid.transform.TransformPoint(basePosition);
			int xDis = Mathf.RoundToInt(dir.z / UI_Main.instance._grid.cellSize);
			int yDis = Mathf.RoundToInt(-dir.x / UI_Main.instance._grid.cellSize);

//			Debug.Log("_currentX:" + _currentX  + "  _currentY" + _currentY);
			_currentX = _X + xDis;
			_currentY = _Y + yDis;

			Vector3 position = UI_Main.instance._grid.GetCenterPosition(_currentX,_currentY, _rows, _columns);
			transform.position = position;

			if(_X != _currentX || _Y != _currentY)
			{
				_baseArea.gameObject.SetActive(true);
			}
			SetbaseColor();

			//Debug.Log(_originalX + " || " + _originalY);
			UI_WarLayout.instance.lastX = _currentX;
			UI_WarLayout.instance.lastY = _currentY;


		}

		private void SetbaseColor()
		{
			if(UI_Main.instance._grid.CanPlaceBuilding(this,currentX, currentY))
			{
				UI_Build.instance.clickConfirmButton.interactable = true;
				_baseArea.sharedMaterial.color = Color.green;
			}
			else
			{
				UI_Build.instance.clickConfirmButton.interactable = false;
				_baseArea.sharedMaterial.color = Color.red;
			}
		}

		[HideInInspector]public bool waitinReplaceRepsonce = false;
		[HideInInspector]public bool waitinDeleteRepsonce = false;


		public void Selected()
		{
	
			if(selectedInstance != null)
			{
				if(selectedInstance == this)
				{
					return;
				}
				else
				{
					selectedInstance.Deselected();
				}
			}
			selectedInstance = this;
			if(waitinReplaceRepsonce)
			{
				return ;
			}
			_baseArea.gameObject.SetActive(true);
			SetbaseColor();
			_originalX = currentX;
			_originalY = currentY;
			UI_BuildingOptions.instance.SetStatus(true);

			
		}

		public void Deselected()
		{
			UI_BuildingOptions.instance.SetStatus(false);
			_baseArea.gameObject.SetActive(false);
			CameraController.instance.isReplacingBuilding = false;
			if(_originalX != currentX || _originalY != currentY)
			{
				SaveLocation();
			}
			selectedInstance = null;
		}

		public void SaveLocation(bool resetIfNot = true)
		{
			if(!UI_Build.instance.wallBrush)
			{
				if(UI_Main.instance._grid.CanPlaceBuilding(this, currentX, currentY) && (_X != currentX || _Y != currentY) &&  !waitinReplaceRepsonce && UI_Main.instance._grid.IsPathToPlayerStart(this.data.warX, this.data.warY,selectedInstance.currentX, selectedInstance.currentY))
				{
					waitinReplaceRepsonce = true;
					if(Ui_Debug.instance.IsDebugging && Ui_Debug.instance.AnotherAccountID != 0)
					{
						
						Debug.Log("sending REPLACEFORANOTERHACCOUNT with id: " + Ui_Debug.instance.AnotherAccountID);
						Packet p = new Packet();
						p.Write((int)Player.RequestId.REPLACEFORANOTERHACCOUNT);
						p.Write(Ui_Debug.instance.AnotherAccountID);
						p.Write(selectedInstance.databaseID);
						p.Write(selectedInstance.currentX);
						p.Write(selectedInstance.currentY);
						p.Write(selectedInstance.yRotation);
						p.Write(UI_WarLayout.instance.isActive ? 2 : 1);
						int temp = UI_WarLayout.instance.isActive ? 2 : 1;
						Debug.Log("changin layout:" + temp + " ||  database id; " + selectedInstance.databaseID);
						Sender.TCP_Send(p);
					}
					else
					{
						Packet packet = new Packet();
						packet.Write((int)Player.RequestId.REPLACE);
						packet.Write(selectedInstance.databaseID);
						packet.Write(selectedInstance.currentX);
						packet.Write(selectedInstance.currentY);
						packet.Write(selectedInstance.yRotation);

						packet.Write(UI_WarLayout.instance.isActive ? 2 : 1);

						Sender.TCP_Send(packet);

					}
					_baseArea.gameObject.SetActive(false);
				}
				else
				{
					if(resetIfNot)
					{
						if(waitinReplaceRepsonce == false)
						{
							PlacedOnGrid(_originalX, _originalY);
						}
						_baseArea.gameObject.SetActive(false);
					}
					else
					{
						if(_originalX == currentX && _originalY == currentY)
						{
							_baseArea.gameObject.SetActive(false);
						}
					}
				}
			}
			//UI_WarLayout.instance.lastX = selectedInstance.currentX;
			//UI_WarLayout.instance.lastY = selectedInstance.currentY;
		}

		private float trapTimer = 1f;
		private float timer = 0; 


		
		
		public class DungeonTrap
		{
			public delegate void PlayerDamageCallBack(float damage);
			public PlayerDamageCallBack _playerDamageCallBack = null;

			private UI_Player _target = null;
		// private UI_Player _Player = null;
			private Vector3 _targetPosition = Vector3.zero;
			private Vector3 _start = Vector3.zero;
			public bool active = false;
			private float time = 0;
			private float timer = 5;
			public Vector3 _trapTrigger = Vector3.zero;
			private Data.Building _data;
			
			public UI_Projectile boulderPrefab = null;
			public Vector3 launchPos = Vector3.zero;
			


		
			public void Initialize(UI_Player target,  Data.Building data, Vector3 trapTrigger, PlayerDamageCallBack playerDamageCallBack)
			{

				if(trapTrigger == Vector3.zero)
					Debug.Log("no trap trigger");
				if(target == null)
					Debug.Log("no target");
				if (target != null && trapTrigger != Vector3.zero)
				{
					_trapTrigger = trapTrigger;
					_target = target;
					_targetPosition = target.transform.position;
					active = true;
					_data = data;
					_playerDamageCallBack = playerDamageCallBack;
					_data.damage = 10f; // REMOVE
					//transform.position = _target.transform.position;
					Debug.Log("trap init ok");
				}
			}

			public void UpdateTrap()
			{
			
				if(active)
				{
					/*
					_targetPosition = _target.transform.position;
					float distance = Vector3.Distance(_targetPosition, _trapTrigger);
					//Debug.Log("distance: " + distance);
                    if(distance < 1.5)
                    {
						SpringTheTrap();
                    }
					*/

				}
				else
				{
					//Debug.Log("timer" + timer + " || active: " + active);
					if(timer <= 0)
					{
						active = true;
						timer = 2f;
					}
					else
					{
						timer -= Time.deltaTime;
					}
				}

			}
			private void SpringTheTrap()
			{
				Debug.Log("calling _playerDamageCallBack from buildin");
				_playerDamageCallBack(_data.damage);
				active = false;

			}
		}
		

	}
}
