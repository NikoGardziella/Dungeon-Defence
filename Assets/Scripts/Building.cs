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
		[SerializeField] private Level[] _levels = null;
		private int _currentX = 0; public int currentX { get { return _currentX; } }
		private int _currentY = 0; public int currentY { get { return _currentY; } }
		private int _X = 0;
		private int _Y = 0;

		private int _originalX = 0;
		private int _originalY = 0;

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
							collectButton.gameObject.SetActive(!collecting && data.isConstructing == false);
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
							collectButton.gameObject.SetActive(!collecting && data.isConstructing == false);
						}
						else
						{
							collectButton.gameObject.SetActive(false);
						}
					break;
						case Data.BuildingID.darkelixirmine:
						if(data.darkStorage >= Data.minDarkElixirCollect)
						{
							collectButton.gameObject.SetActive(!collecting);
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
			Destroy(gameObject);
		}
		public void UpdateGridPosition(Vector3 basePosition, Vector3 currentPosition)
		{
			Vector3 dir = UI_Main.instance._grid.transform.TransformPoint(currentPosition) - UI_Main.instance._grid.transform.TransformPoint(basePosition);
			int xDis = Mathf.RoundToInt(dir.z / UI_Main.instance._grid.cellSize);
			int yDis = Mathf.RoundToInt(-dir.x / UI_Main.instance._grid.cellSize);

			_currentX = _X + xDis;
			_currentY = _Y + yDis;

			Vector3 position = UI_Main.instance._grid.GetCenterPosition(_currentX,_currentY, _rows, _columns);
			transform.position = position;

			if(_X != _currentX || _Y != _currentY)
			{
				_baseArea.gameObject.SetActive(true);
			}
			SetbaseColor();
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

		public void Selected()
		{
			Debug.Log("selected");
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
			if(waitinReplaceRepsonce)
			{
				return ;
			}
			_originalX = currentX;
			_originalY = currentY;
			selectedInstance = this;
			UI_BuildingOptions.instance.SetStatus(true);
		}

		public void Deselected()
		{
			UI_BuildingOptions.instance.SetStatus(false);
			
			CameraController.instance.isReplacingBuilding = false;
			if(_originalX != currentX || _originalY != currentY)
			{
				SaveLocation();
			}
			selectedInstance = null;
		}

		public void SaveLocation(bool resetIfNot = true)
		{
			if(UI_Main.instance._grid.CanPlaceBuilding(this, currentX, currentY) && (_X != currentX || _Y != currentY) &&  !waitinReplaceRepsonce)
			{
				waitinReplaceRepsonce = true;
				Packet packet = new Packet();
				packet.Write((int)Player.RequestId.REPLACE);
				packet.Write(selectedInstance.databaseID);
				packet.Write(selectedInstance.currentX);
				packet.Write(selectedInstance.currentY);
				Sender.TCP_Send(packet);
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
			}
		}


	}
}
