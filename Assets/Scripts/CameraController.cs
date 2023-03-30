namespace DungeonDefence
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using DevelopersHub.RealtimeNetworking.Client;


	public class CameraController : MonoBehaviour
	{

		public UI_Player ui_Player;

		private static CameraController _instance = null; public static CameraController instance {get {return _instance; } }

		[SerializeField] private Camera _camera = null;
		[SerializeField] private float _moveSpeed = 50.0f;
		[SerializeField] private float _moveSmooth = 1.0f;
		[SerializeField] private float _zoomSmooth = 5.0f;
		[SerializeField] private float _zoomSpeed = 5f;


		[SerializeField] private float _scrollZoomSpeed = 20f;
		

		public Control _inputs = null;
		private bool _zooming = false;
		private bool _moving = false;
		private Vector3 _center = Vector3.zero;
		private float _right = 10.0f;
		private float _left = 10.0f;
		private float _up = 10.0f;
		private float _down = 10.0f;
		private float _angle = 45.0f;
		private float _zoom = 5f;

		private Vector2 _zoomPositionOnSCreen = Vector2.zero;
		private Vector3 _zoomPositionInWorld = Vector3.zero;
		private float _zoomBaseValue = 0.0f;
		private float _zoomBaseDistance = 0.0f;
		private float _maxZoom = 10.0f;
		private float _minZoom = 1.0f;
		private Vector3 _moveRootPosition = Vector3.zero;
		private Vector2 _moveBaseDirection = Vector2.zero;
		private Vector2 _moveBasePosition = Vector2.zero;


		private Transform _root = null;
		private Transform _pivot = null;
		private Transform _target = null;

		private bool _building = false; public bool isPlacingBuilding{ get { return _building; } set { _building = value; } }
		private bool _placingUnit = false; public bool isPlacingUnit{ get { return _placingUnit; } set { _placingUnit = value; } }
		public Vector3 _unitBasePosition = Vector3.zero;


		public Vector3 _buildBasePosition = Vector3.zero;
		private bool _movingBuilding = false;
		private bool _movingUnit= false;

		private bool _replacing = false; public bool isReplacingBuilding{ get { return _replacing; } set { _replacing = value; } }
		private Vector3 _ReplaceBasePosition = Vector3.zero;
		private bool _replacingBuilding = false;

		private float normalZoom = 10f;

		public Vector3 planeDownLeft = Vector3.zero;
		public Vector3 planeTopRight = Vector3.zero;
		[SerializeField] public GameObject normalLayout = null;
		[SerializeField] public GameObject dungeonLayout = null;
		private void Awake()
		{
			_instance = this;
			_inputs = new Control();
			_root = new GameObject("CameraHelper").transform;
			_pivot = new GameObject("CameraPivot").transform;
			_target = new GameObject("GameraTarget").transform;
			_camera.orthographic = true;
			_camera.nearClipPlane = 0;
		}

		private void Start()
		{
			Initialize(Vector3.zero, 40.0f, 40.0f, 40.0f, 40.0f, 45.0f, normalZoom, 5.0f , 20.0f);
		}

		public void Initialize(Vector3 center, float right, float left, float up, float down, float angle, float zoom, float minZoom, float maxZoom)
		{
			_center = center;
			_right = right;
			_left = left;
			_down = down;
			_up = up;
			_moving = false;
			_angle = angle;
			_pivot.SetParent(_root);
			_target.SetParent(_pivot);
			_zoom = zoom;
			_maxZoom = maxZoom;
			_minZoom = minZoom;

			_camera.orthographicSize = _zoom;


			_root.position = center;
			_root.localEulerAngles = Vector3.zero;

			_pivot.localPosition = Vector3.zero;
			_pivot.localEulerAngles = new Vector3(_angle, 0,0);

			_target.localPosition = new Vector3(0,0, - 100);
			_target.localEulerAngles = Vector3.zero;
		}


		private void OnEnable()
		{
			_inputs.Enable();
			_inputs.Main.Move.started += _ => MoveStarted();
			_inputs.Main.Move.canceled += _ => MoveCanceled();
			_inputs.Main.TouchZoom.started += _ => ZoomStarted();
			_inputs.Main.TouchZoom.canceled += _ => ZoomCanceled();
			_inputs.Main.PointerClick.performed += _ => ScreenClicked();

		}
		private void OnDisable()
		{
			_inputs.Main.Move.started -= _ => MoveStarted();
			_inputs.Main.Move.canceled -= _ => MoveCanceled();
			_inputs.Main.TouchZoom.started -= _ => ZoomStarted();
			_inputs.Main.TouchZoom.canceled -= _ => ZoomCanceled();
			_inputs.Main.PointerClick.performed -= _ => ScreenClicked();
			_inputs.Disable();
		}

		private void ScreenClicked()
		{
			Vector2 position = _inputs.Main.PointerPosition.ReadValue<Vector2>();
			PointerEventData data = new PointerEventData(EventSystem.current);
			data.position = position;
			List<RaycastResult> results = new List<RaycastResult>();
			EventSystem.current.RaycastAll(data, results);

			if(UI_Main.instance.isActive || UI_WarLayout.instance.isActive)
			{
				if(results.Count <= 0)
				{
					Debug.Log("no results. unit count;" + UI_Main.instance._grid.units.Count);
					bool found = false;
					Vector3 planePosition = CameraScreenPositionToPlanePosition(position);

					for (int i = 0; i < UI_Main.instance._grid.units.Count; i++)
					{
						if(UI_Main.instance._grid.IsWorldPositionIsOnPlane(planePosition, UI_Main.instance._grid.units[i].currentX, UI_Main.instance._grid.units[i].currentY, 1, 1))
						{
							Debug.Log("found");
							found = true;
							UI_Main.instance._grid.units[i].Selected();
							break;
						}
					}
					for (int i = 0; i < UI_Main.instance._grid.buildings.Count; i++)
					{
						if(UI_Main.instance._grid.IsWorldPositionIsOnPlane(planePosition, UI_Main.instance._grid.buildings[i].currentX, UI_Main.instance._grid.buildings[i].currentY, UI_Main.instance._grid.buildings[i].rows, UI_Main.instance._grid.buildings[i].columns))
						{
							found = true;
							UI_Main.instance._grid.buildings[i].Selected();
							break;
						}
					}
					if(!found)
					{
						if(Building.selectedInstance != null)
						{
							Building.selectedInstance.Deselected();
						}
						if(Unit.selectedInstance != null)
						{
							Unit.selectedInstance.Deselected();
						}
					}
				}
				else
				{
					if(Building.selectedInstance != null)
					{		
						for (int i = 0; i < results.Count; i++)
						{
							if(results[i].gameObject == UI_BuildingOptions.instance.infoButton.gameObject)
							{
								Debug.Log("show info of: " + Building.selectedInstance.id);
							}
							else if(results[i].gameObject == UI_BuildingOptions.instance.upgradeButton.gameObject)
							{
								Packet packet = new Packet();
								packet.Write((int)Player.RequestId.PREUPGRADE);
								packet.Write(Building.selectedInstance.data.databaseID);
								Sender.TCP_Send(packet);
							}
							else if(results[i].gameObject == UI_BuildingOptions.instance.instantButton.gameObject)
							{
								Packet packet = new Packet();
								packet.Write((int)Player.RequestId.INSTANTBUILD);
								packet.Write(Building.selectedInstance.data.databaseID);
								Sender.TCP_Send(packet);
							}
							else if(results[i].gameObject == UI_BuildingOptions.instance.trainButton.gameObject)
							{
								UI_Train.instance.SetStatus(true);
							}
							else if(results[i].gameObject == UI_BuildingOptions.instance.clanButton.gameObject)
							{
								UI_Clan.instance.Open();
							}
							else if(results[i].gameObject == UI_BuildingOptions.instance.spellButton.gameObject)
							{
								UI_Spell.instance.SetStatus(true);
							}
							else if(results[i].gameObject == UI_BuildingOptions.instance.deleteBuildingButton.gameObject)
							{
								Debug.Log(Building.selectedInstance.data.id);
								Building.selectedInstance.DeleteBuilding();
							//	Building.buildInstance.RemovedFromGrid();
							}
							else if(results[i].gameObject == UI_BuildingOptions.instance.editDungeonButton.gameObject)
							{
								dungeonLayout.SetActive(true);
								normalLayout.SetActive(false);
								UI_WarLayout.instance.SetStatus(true);
							}
						}
						
						Building.selectedInstance.Deselected();
					}

					if(Unit.selectedInstance != null)
					{
						for (int i = 0; i < results.Count; i++)
						{
							if(results[i].gameObject == UI_BuildingOptions.instance.deleteBuildingButton.gameObject)
							{								
								Unit.selectedInstance.DeleteUnit();							
							}
						}
						Unit.selectedInstance.Deselected();
					}
				}
			}
			else if(UI_Battle.instance.isActive)
			{
				if(results.Count <= 0 && (UI_Battle.instance.selectedUnit >= 0 ||UI_Battle.instance.selectedSpell >= 0))
				{
					Vector3 planePosition = CameraScreenPositionToPlanePosition(position);
					planePosition = UI_Main.instance._grid.transform.InverseTransformPoint(planePosition);
					if(planePosition.x >= (0 - Data.battleGridOffset) && planePosition.x < (Data.gridSize + Data.battleGridOffset) && planePosition.z >= (0 - Data.battleGridOffset)  && planePosition.z < (Data.gridSize + Data.battleGridOffset))
					{
						UI_Battle.instance.PlaceUnit(Mathf.FloorToInt(planePosition.x), Mathf.FloorToInt(planePosition.z));
					}
				}
			}
			else if(UI_WarLayout.instance.isActive)
			{
				if(results.Count <= 0)
				{
					bool found = false;
					Vector3 planePosition = CameraScreenPositionToPlanePosition(position);
					for (int i = 0; i < UI_Main.instance._grid.buildings.Count; i++)
					{
						if(UI_Main.instance._grid.IsWorldPositionIsOnPlane(planePosition, UI_Main.instance._grid.buildings[i].currentX, UI_Main.instance._grid.buildings[i].currentY, UI_Main.instance._grid.buildings[i].rows, UI_Main.instance._grid.buildings[i].columns))
						{
							found = true;
							UI_Main.instance._grid.buildings[i].Selected();
							break;
						}
					}
					for (int i = 0; i < UI_Main.instance._grid.units.Count; i++)
					{
						if(UI_Main.instance._grid.IsWorldPositionIsOnPlane(planePosition, UI_Main.instance._grid.units[i].currentX, UI_Main.instance._grid.units[i].currentY, 1, 1))
						{
							found = true;
							UI_Main.instance._grid.units[i].Selected();
							break;
						}
					}

					if(!found)
					{
						if(Building.selectedInstance != null)
						{
							Building.selectedInstance.Deselected();
						}
						if(Unit.selectedInstance != null)
						{
							Unit.selectedInstance.Deselected();
						}
					}
				}
				else
				{
					if(Building.selectedInstance != null)
					{
						Building.selectedInstance.Deselected();
					}
					if(Unit.selectedInstance != null)
					{
						Unit.selectedInstance.Deselected();
					}
				}
			}
		}

		public bool IsSreenPointOverUI(Vector2 position)
		{
			PointerEventData data = new PointerEventData(EventSystem.current);
			data.position = position;
			List<RaycastResult> results = new List<RaycastResult>();
			EventSystem.current.RaycastAll(data, results);
			return results.Count > 0;
		}

		private void MoveStarted()
		{
			if((UI_Main.instance.isActive || UI_Battle.instance.isActive || UI_WarLayout.instance.isActive) && UI_Chat.instance.isActive == false && UI_Settings.instance.isActive == false)
			{
				if(_building)
				{
					_buildBasePosition = CameraScreenPositionToPlanePosition(_inputs.Main.PointerPosition.ReadValue<Vector2>());
					if(UI_Main.instance._grid.IsWorldPositionIsOnPlane(_buildBasePosition, Building.buildInstance.currentX,Building.buildInstance.currentY,Building.buildInstance.rows, Building.buildInstance.columns))
					{
						Building.buildInstance.StartMovingOnGrid();
						_movingBuilding = true;
					}
				}
				else if(_placingUnit)
				{
					_unitBasePosition = CameraScreenPositionToPlanePosition(_inputs.Main.PointerPosition.ReadValue<Vector2>());
					if(UI_Main.instance._grid.IsWorldPositionIsOnPlane(_unitBasePosition, Unit.unitInstance.currentX,Unit.unitInstance.currentY, 1, 1))
					{		
						Unit.unitInstance.StartMovingOnGrid();
						_movingUnit = true;
					}
				}

				if(Building.selectedInstance != null)
				{
					_ReplaceBasePosition = CameraScreenPositionToPlanePosition(_inputs.Main.PointerPosition.ReadValue<Vector2>());
					if(UI_Main.instance._grid.IsWorldPositionIsOnPlane(_ReplaceBasePosition, Building.selectedInstance.currentX, Building.selectedInstance.currentY, Building.selectedInstance.rows, Building.selectedInstance.columns))
					{
						if(!_replacing)
						{
							_replacing = true;
						}
						Building.selectedInstance.StartMovingOnGrid();
						_replacingBuilding = true;
					}
				}
				if(_movingBuilding == false && _replacingBuilding == false && _placingUnit == false) 
				{
					Debug.Log("everything false");
					_moveRootPosition = _root.position;
					_moveBasePosition = _inputs.Main.PointerPosition.ReadValue<Vector2>();
					Vector3 dl = CameraScreenPositionToWorldPosition(Vector2.zero);
					Vector3 tr = CameraScreenPositionToWorldPosition(new Vector2(Screen.width, Screen.height));
					_moveBaseDirection = new Vector2((tr.x - dl.x) / Screen.width, (tr.y - dl.y) / Screen.height);
					_moving = true;
				}
			}
		}
		private void MoveCanceled()
		{
			_moving = false;
			_movingBuilding = false;
			_movingUnit = false;
			if(_replacingBuilding)
			{
				_replacingBuilding = false;
				if(Building.selectedInstance)
				{
					Building.selectedInstance.SaveLocation(false);
				}
			}
		}

		private void ZoomStarted()
		{
			if((UI_Main.instance.isActive || UI_Battle.instance.isActive || UI_WarLayout.instance.isActive)  && UI_Chat.instance.isActive == false && UI_Settings.instance.isActive == false)
			{
				_moveRootPosition = _root.position;
				Vector2 touch0 = _inputs.Main.TouchPosition0.ReadValue<Vector2>();
				Vector2 touch1 = _inputs.Main.TouchPosition1.ReadValue<Vector2>();
				_zoomPositionOnSCreen = Vector2.Lerp(touch0, touch1, 0.5f);
				_zoomPositionInWorld = CameraScreenPositionToPlanePosition(_zoomPositionOnSCreen);
				_zoomBaseValue = _zoom;

				touch0.x /= Screen.width;
				touch1.x /= Screen.width;
				touch0.y /= Screen.height;
				touch1.y /= Screen.height;

				_zoomBaseDistance = Vector2.Distance(touch0, touch1);
				_zooming = true;
				_moving = false;
			}
		}
		private void ZoomCanceled()
		{
			_zooming = false;
		}

		private void Update()
		{
			if(Input.touchSupported == false)
			{
				float mouseScroll = _inputs.Main.MouseScroll.ReadValue<float>();
				if(mouseScroll > 0)
				{
					_zoom -= _scrollZoomSpeed * Time.deltaTime;
				}
				else if (mouseScroll < 0)
				{
					_zoom += _scrollZoomSpeed * Time.deltaTime;
				}
			}
			if(_zooming)
			{
				Vector2 touch0 = _inputs.Main.TouchPosition0.ReadValue<Vector2>();
				Vector2 touch1 = _inputs.Main.TouchPosition1.ReadValue<Vector2>();
	
				touch0.x /= Screen.width;
				touch1.x /= Screen.width;
				touch0.y /= Screen.height;
				touch1.y /= Screen.height;

				float currentDistance = Vector2.Distance(touch0, touch1);
				float deltaDistance = currentDistance - _zoomBaseDistance;
				_zoom = _zoomBaseValue - (deltaDistance * _zoomSpeed);

				Vector3 zoomCenter = CameraScreenPositionToPlanePosition(_zoomPositionOnSCreen);
				_root.position = _moveRootPosition + (_zoomPositionInWorld - zoomCenter);
			}
			else if(dungeonLayout.gameObject.activeInHierarchy == true && Player.inBattle)
			{
				_target.position =  ui_Player.transform.position;
				_target.localPosition = new Vector3(ui_Player.transform.position.x,ui_Player.transform.position.z * 0.70f, - 100);
				_zoom = 5f;

				//_pivot.localPosition = Vector3.zero;
				//_pivot.localEulerAngles = new Vector3(_angle, ui_Player.move.x ,ui_Player.transform.rotation.y);
			}
			else if(_moving)
			{
				/* if(_zoom != normalZoom)
				{
					_target.localPosition = new Vector3(0,0, - 100);
					_target.localEulerAngles = Vector3.zero;	
					_zoom = normalZoom;
				} */
				Vector2 delta = _inputs.Main.PointerPosition.ReadValue<Vector2>() - _moveBasePosition;
				
				_root.position = _moveRootPosition - new Vector3(delta.x * _moveBaseDirection.x, 0 , delta.y * _moveBaseDirection.y * 2f);
			}
			AdjustBounds();
			if(_camera.orthographicSize != _zoom)
			{
				_camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _zoom, _zoomSmooth * Time.deltaTime);
			}
			if(_camera.transform.position != _target.position)
			{
				Vector3 velocity = Vector3.zero;
				_camera.transform.position =  Vector3.SmoothDamp(_camera.transform.position, _target.position,ref velocity, _moveSmooth * Time.deltaTime);
			}
			if(_camera.transform.rotation != _target.rotation)
			{
				_camera.transform.rotation = _target.rotation;
			}

			if(_building && _movingBuilding)
			{
				Vector3 pos = CameraScreenPositionToPlanePosition(_inputs.Main.PointerPosition.ReadValue<Vector2>());
				Building.buildInstance.UpdateGridPosition(_buildBasePosition, pos);
			}
			if(_replacing && _replacingBuilding)
			{
				Vector3 pos = CameraScreenPositionToPlanePosition(_inputs.Main.PointerPosition.ReadValue<Vector2>());
				Building.selectedInstance.UpdateGridPosition(_ReplaceBasePosition, pos);
			}
			//Debug.Log(_placingUnit + " " + _movingUnit);
			if(_placingUnit && _movingUnit)
			{
				Vector3 pos = CameraScreenPositionToPlanePosition(_inputs.Main.PointerPosition.ReadValue<Vector2>());
				Unit.unitInstance.UpdateGridPosition(_unitBasePosition, pos);
			}

			planeDownLeft = CameraScreenPositionToPlanePosition(Vector2.zero);
			planeTopRight = CameraScreenPositionToPlanePosition(new Vector2(Screen.width, Screen.height));

		}

		public Vector3 CameraScreenPositionToWorldPosition(Vector2 position)
		{
			float h = _camera.orthographicSize * 2f;
			float w = _camera.aspect * h;

			Vector3 anchor = _camera.transform.position - (_camera.transform.right.normalized * w / 2f) - (_camera.transform.up.normalized * h / 2.0f);
			return anchor + (_camera.transform.right.normalized * position.x / Screen.width * w) + (_camera.transform.up.normalized * position.y / Screen.height * h);
		}

		public Vector3 CameraScreenPositionToPlanePosition(Vector2 position)
		{
			Vector3 point = CameraScreenPositionToWorldPosition(position);
			float h = point.y - _root.position.y;
			float x = h / Mathf.Sin(_angle * Mathf.Deg2Rad);
			return point + _camera.transform.forward.normalized * x;
		}

		private void AdjustBounds()
		{
			if(_zoom < _minZoom)
				_zoom = _minZoom;
			if(_zoom > _maxZoom)
				_zoom = _maxZoom;
			float h = PlaneOrthographicSize();
			float w = h * _camera.aspect;
			if(h > (_up + _down) / 2.0f)
			{
				float n = (_up +_down)  / 2.0f;
				_zoom = n * Mathf.Sin(_angle * Mathf.Deg2Rad);
			}
			if(w > (_right + _left / 2.0f))
			{
				float n = (_right +_left)  / 2.0f;
				_zoom = n * Mathf.Sin(_angle * Mathf.Deg2Rad) / _camera.aspect;
			}

			h = PlaneOrthographicSize();
			w = _zoom * _camera.aspect;
			Vector3 topRight = _root.position + _root.right.normalized * w + _root.forward.normalized * h;
			Vector3 topLeft = _root.position - _root.right.normalized * w + _root.forward.normalized * h;
			Vector3 downRight = _root.position + _root.right.normalized * w - _root.forward.normalized * h;
			Vector3 downLeft = _root.position - _root.right.normalized * w - _root.forward.normalized * h;

			if(topRight.x > _center.x + _right)
			{
				_root.position += Vector3.left * Mathf.Abs(topRight.x - (_center.x + _right));
			}
			if(topLeft.x < _center.x - _left)
			{
				_root.position += Vector3.right * Mathf.Abs((_center.x - _left) - topLeft.x);
			}

			if(topRight.z > _center.z + _up)
			{
				_root.position += Vector3.back * Mathf.Abs(topRight.z - (_center.z + _up));
			}
			if(downLeft.z < _center.z - _down)
			{
				_root.position += Vector3.forward * Mathf.Abs((_center.z - _down) - downLeft.z);
			}
		}

		private float PlaneOrthographicSize()
		{
			float h = _zoom * 2.0f;
			return (h / Mathf.Sin(_angle * Mathf.Deg2Rad) / 2.0f);
		}

	
	}
}