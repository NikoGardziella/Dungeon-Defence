namespace DungeonDefence
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;


	public class CameraController : MonoBehaviour
	{

		[SerializeField] private Camera _camera = null;
		[SerializeField] private float _moveSpeed = 50.0f;
		[SerializeField] private float _moveSmooth = 5.0f;
		[SerializeField] private float _zoomSmooth = 5.0f;
		[SerializeField] private float _zoomSpeed = 5f;

		private Control _inputs = null;
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

		private Transform _root = null;
		private Transform _pivot = null;
		private Transform _target = null;

		private void Awake()
		{
			_inputs = new Control();
			_root = new GameObject("CameraHelper").transform;
			_pivot = new GameObject("CameraPivot").transform;
			_target = new GameObject("GameraTarget").transform;
			_camera.orthographic = true;
			_camera.nearClipPlane = 0;
		}

		private void Start()
		{
			Initialize(Vector3.zero, 10.0f, 10.0f, 10.0f, 10.0f, 45.0f, 5.0f, 10.0f , 3.0f);
		}

		public void Initialize(Vector3 center, float right, float left, float up, float down, float angle, float zoom, float maxZoom, float minZoom)
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

			_target.localPosition = new Vector3(0,0, - 10);
			_target.localEulerAngles = Vector3.zero;
		}


		private void OnEnable()
		{
			_inputs.Enable();
			_inputs.Main.Move.started += _ => MoveStarted();
			_inputs.Main.Move.canceled += _ => MoveCanceled();
			_inputs.Main.TouchZoom.started += _ => ZoomStarted();
			_inputs.Main.TouchZoom.canceled += _ => ZoomCanceled();
		}
		private void OnDisable()
		{

			_inputs.Main.Move.started -= _ => MoveStarted();
			_inputs.Main.Move.canceled -= _ => MoveCanceled();
			_inputs.Main.TouchZoom.started -= _ => ZoomStarted();
			_inputs.Main.TouchZoom.canceled -= _ => ZoomCanceled();
			_inputs.Disable();
		}

		private void MoveStarted()
		{
			_moving = true;
		}
		private void MoveCanceled()
		{
			_moving = false;
		}

		private void ZoomStarted()
		{
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
					_zoom -= 3f * Time.deltaTime;
				}
				else if (mouseScroll < 0)
				{
					_zoom += 3f * Time.deltaTime;

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
				_root.position += (_zoomPositionInWorld - zoomCenter);
			}
			else if(_moving)
			{
				Vector2 move = _inputs.Main.MoveDelta.ReadValue<Vector2>();
				if(move != Vector2.zero)
				{
					move.x /= Screen.width;
					move.y /= Screen.height;
					_root.position -= _root.right.normalized * move.x * _moveSpeed;
					_root.position -= _root.forward.normalized * move.y * _moveSpeed;

				}
			}
			AdjustBounds();
			if(_camera.orthographicSize != _zoom)
			{
				_camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _zoom, _zoomSmooth * Time.deltaTime);
			}
			if(_camera.transform.position != _target.position)
			{
				_camera.transform.position =  Vector3.Lerp(_camera.transform.position, _target.position, _moveSmooth * Time.deltaTime);
			}
			if(_camera.transform.rotation != _target.rotation)
			{
				_camera.transform.rotation = _target.rotation;
			}
		}

		private Vector3 CameraScreenPositionToWorldPosition(Vector2 position)
		{
			float h = _camera.orthographicSize * 2f;
			float w = _camera.aspect * h;

			Vector3 anchor = _camera.transform.position - (_camera.transform.right.normalized * w / 2f) - (_camera.transform.up.normalized * h / 2.0f);
			return anchor + (_camera.transform.right.normalized * position.x / Screen.width * w) + (_camera.transform.up.normalized * position.y / Screen.height * h);
		}

		private Vector3 CameraScreenPositionToPlanePosition(Vector2 position)
		{
			Vector3 point = CameraScreenPositionToWorldPosition(position);
			float h = point.y - _root.position.y;
			float x = h / Mathf.Sin(_angle * Mathf.Deg2Rad); // or Sign?
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
				_zoom = (_up +_down)  / 2.0f;
			if(w > (_right + _left / 2.0f))
				_zoom = (_right +_left)  / 2.0f / _camera.aspect;

			h = PlaneOrthographicSize();
			w = h * _camera.aspect;
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