namespace DungeonDefence
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using DevelopersHub.RealtimeNetworking.Client;

	public class Unit : MonoBehaviour
	{
		
		public Data.UnitID id = Data.UnitID.barbarian;
		private static Unit _unitInstance = null; public static Unit unitInstance {get {return _unitInstance; } set { _unitInstance = value;} }
		private static Unit _selectedInstance = null; public static Unit selectedInstance {get {return _selectedInstance; } set { _selectedInstance = value;} }
		[SerializeField] private long _databaseID = 1; public long databaseID  { get { return _databaseID; } set { _databaseID = value; }  }
		[SerializeField] public MeshRenderer _baseArea = null;


		private int _currentX = 0; public int currentX { get { return _currentX; } set { currentX = value; } }
		private int _currentY = 0; public int currentY { get { return _currentY; } set { currentY = value; } }
		private int _X = 0;
		private int _Y = 0;

		private int _originalX = 0;
		private int _originalY = 0;

		[HideInInspector]public bool waitinReplaceRepsonce = false;
		[HideInInspector]public bool waitinDeleteRepsonce = false;
		public void PlacedOnGrid(int x, int y)
		{
			_currentX = x;
			_currentY = y;
			_X = x;
			_Y = y;
			_originalX = x;
			_originalY = y;
			Vector3 position = UI_Main.instance._grid.GetCenterPosition(x,y, 1, 1);
			transform.position = position;
		}
		public void StartMovingOnGrid()
		{
			_X = _currentX;
			_Y = _currentY;
		}

		public void UpdateGridPosition(Vector3 basePosition, Vector3 currentPosition)
		{	

			Vector3 dir = UI_Main.instance._grid.transform.TransformPoint(currentPosition) - UI_Main.instance._grid.transform.TransformPoint(basePosition);
			int xDis = Mathf.RoundToInt(dir.z / UI_Main.instance._grid.cellSize);
			int yDis = Mathf.RoundToInt(-dir.x / UI_Main.instance._grid.cellSize);

			//Debug.Log("UNIT: _currentX:" + _currentX  + "  _currentY" + _currentY);
			_currentX = _X + xDis;
			_currentY = _Y + yDis;

			Vector3 position = UI_Main.instance._grid.GetCenterPosition(_currentX,_currentY, 1, 1);
			transform.position = position;

			if(_X != _currentX || _Y != _currentY)
			{
				_baseArea.gameObject.SetActive(true);
			}
			SetbaseColor();
		}

		private void SetbaseColor()
		{
			if(UI_Main.instance._grid.CanPlaceUnit(this,currentX, currentY))
			{
				UI_PlaceUnit.instance.clickConfirmButton.interactable = true;
				_baseArea.sharedMaterial.color = Color.green;
			}
			else
			{
				UI_PlaceUnit.instance.clickConfirmButton.interactable = false;
				_baseArea.sharedMaterial.color = Color.red;
			}
		}

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
			Debug.Log("Selected" + selectedInstance.id);
			_baseArea.gameObject.SetActive(true);
			SetbaseColor();
			_originalX = currentX;
			_originalY = currentY;
			UI_BuildingOptions.instance.SetStatus(true);
		}

		public void Deselected()
		{			
			_baseArea.gameObject.SetActive(false);
			Debug.Log("DEselected" + selectedInstance.id);
			CameraController.instance.isPlacingUnit = false;
			if(_originalX != currentX || _originalY != currentY)
			{
				SaveLocation();
			}
			selectedInstance = null;
		}
		public void DeleteUnit()
		{
			Debug.Log("unit deleted");
		}

		public void RemovedFromGrid()
		{
			unitInstance = null;
			UI_PlaceUnit.instance.SetStatus(false);
			CameraController.instance.isPlacingUnit = false;
			Destroy(gameObject);
		}

		public void SaveLocation(bool resetIfNot = true)
		{
			if(UI_Main.instance._grid.CanPlaceUnit(this, currentX, currentY) && (_X != currentX || _Y != currentY) &&  !waitinReplaceRepsonce)
			{
				waitinReplaceRepsonce = true;
				Packet packet = new Packet();
				packet.Write((int)Player.RequestId.REPLACEUNIT);
				packet.Write(selectedInstance.databaseID);
				packet.Write(selectedInstance.currentX);
				packet.Write(selectedInstance.currentY);
				packet.Write(UI_WarLayout.instance.isActive ? 2 : 1);

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
				else
				{
					if(_originalX == currentX && _originalY == currentY)
					{
						_baseArea.gameObject.SetActive(false);
					}
				}
			}
		}
	}

}
