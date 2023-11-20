namespace DungeonDefence
{

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using DevelopersHub.RealtimeNetworking.Client;

	public class UI_Build : MonoBehaviour
	{
		[SerializeField] public GameObject _elements = null;

		[SerializeField] public bool wallBrush = false;
		[SerializeField] public bool wallBrushRemove = false;


		public RectTransform buttonConfirm = null;
		public RectTransform buttonCancel = null;
		public RectTransform buttonMoveRight = null;

		public RectTransform buttonMoveUp = null;

		public RectTransform buttonMoveLeft = null;

		public RectTransform buttonMoveDown = null;
		public RectTransform buttonRotateClockwise = null;
		public RectTransform buttonIncreaseSize = null;


		[HideInInspector] public Button clickConfirmButton = null;
		[HideInInspector] public Button clickCancelButton = null;
		[HideInInspector] public Button clickMoveRightButton = null;

		[HideInInspector] public Button clickMoveLeftButton = null;
		[HideInInspector] public Button clickMoveUpButton = null;
		[HideInInspector] public Button clickMoveDownButton = null;
		[HideInInspector] public Button clickRotateClockwiseButton = null;
		[HideInInspector] public Button clickIncreaseSizeButton = null;


		private static UI_Build _instance = null; public static UI_Build instance {get {return _instance; } }
		private int moveX = 1;
		private int moveY = 0;

		
		private void Awake()
		{
			_instance = this;
			_elements.SetActive(false);
			clickConfirmButton = buttonConfirm.gameObject.GetComponent<Button>();
			clickMoveRightButton = buttonMoveRight.gameObject.GetComponent<Button>();
			clickCancelButton = buttonCancel.gameObject.GetComponent<Button>();
			clickMoveLeftButton = buttonMoveLeft.gameObject.GetComponent<Button>();
			clickMoveUpButton = buttonMoveUp.gameObject.GetComponent<Button>();
			clickMoveDownButton = buttonMoveDown.gameObject.GetComponent<Button>();
			clickRotateClockwiseButton = buttonRotateClockwise.gameObject.GetComponent<Button>();
			clickIncreaseSizeButton = buttonIncreaseSize.gameObject.GetComponent<Button>();



		}

		private void Start()
		{
			buttonConfirm.gameObject.GetComponent<Button>().onClick.AddListener(Confirm);
			buttonCancel.gameObject.GetComponent<Button>().onClick.AddListener(Cancel);
			buttonMoveRight.gameObject.GetComponent<Button>().onClick.AddListener(MoveRight);
			buttonMoveLeft.gameObject.GetComponent<Button>().onClick.AddListener(MoveLeft);
			buttonMoveUp.gameObject.GetComponent<Button>().onClick.AddListener(MoveUp);
			buttonMoveDown.gameObject.GetComponent<Button>().onClick.AddListener(MoveDown);
			buttonRotateClockwise.gameObject.GetComponent<Button>().onClick.AddListener(RotateClockwise);
			buttonIncreaseSize.gameObject.GetComponent<Button>().onClick.AddListener(IncreaseSize);


			buttonConfirm.anchorMin = Vector3.zero;
			buttonConfirm.anchorMax = Vector3.zero;
			buttonCancel.anchorMin = Vector3.zero;
			buttonCancel.anchorMax = Vector3.zero;
			buttonMoveRight.anchorMin = Vector3.zero;
			buttonMoveRight.anchorMax = Vector3.zero;
			buttonMoveLeft.anchorMin = Vector3.zero;
			buttonMoveLeft.anchorMax = Vector3.zero;
			buttonMoveUp.anchorMin = Vector3.zero;
			buttonMoveUp.anchorMin = Vector3.zero;
			buttonMoveDown.anchorMin = Vector3.zero;
			buttonMoveDown.anchorMin = Vector3.zero;
			buttonRotateClockwise.anchorMin = Vector3.zero;
			buttonIncreaseSize.anchorMin = Vector3.zero;


		}

		private void Update()
		{
			if(Building.buildInstance != null && CameraController.instance.isPlacingBuilding)
			{
				Vector3 end = UI_Main.instance._grid.GetEndPosition(Building.buildInstance);
				
				Vector3 planeDownLeft = CameraController.instance.planeDownLeft;
				Vector3 planeTopRight = CameraController.instance.planeTopRight;

				float w = planeTopRight.x - planeDownLeft.x;
				float h = planeTopRight.z - planeDownLeft.z;

				float endW = end.x - planeDownLeft.x;
				float endH = end.z - planeDownLeft.z;

				Vector2 screenPoint = new Vector2(endW / w * Screen.width, endH / h * Screen.height);

				Vector2 confirmPoint = screenPoint;
				confirmPoint.x += (buttonConfirm.rect.width + 0f);
				confirmPoint.y += (buttonConfirm.rect.width + 15f);
				buttonConfirm.anchoredPosition = confirmPoint;

				Vector2 cancelPoint = screenPoint;
				cancelPoint.x -= (buttonCancel.rect.width + 0f);
				cancelPoint.y += (buttonCancel.rect.width + 15f);
				buttonCancel.anchoredPosition = cancelPoint;

				Vector2 rightPoint = screenPoint;
				rightPoint.x += (buttonMoveRight.rect.width - 20f);
				rightPoint.y += (buttonMoveRight.rect.width - 30f);
				buttonMoveRight.anchoredPosition = rightPoint;

				Vector2 LeftPoint = screenPoint;
				LeftPoint.x += (buttonMoveLeft.rect.width - 90f);
				LeftPoint.y += (buttonMoveLeft.rect.width - 90f);
				buttonMoveLeft.anchoredPosition = LeftPoint;

				Vector2 upPoint = screenPoint;
				upPoint.x += (buttonMoveUp.rect.width - 80f);
				upPoint.y += (buttonMoveUp.rect.width - 35f);
				buttonMoveUp.anchoredPosition = upPoint;

				Vector2 downPoint = screenPoint;
				downPoint.x += (buttonMoveDown.rect.width - 10f);
				downPoint.y += (buttonMoveDown.rect.width - 90f);
				buttonMoveDown.anchoredPosition = downPoint;

				Vector2 RotatePoint = screenPoint;
				RotatePoint.x += (buttonRotateClockwise.rect.width - 90f);
				RotatePoint.y += (buttonRotateClockwise.rect.width - 100f);
				buttonRotateClockwise.anchoredPosition = RotatePoint;

				Vector2 IncreasePoint = screenPoint;
				IncreasePoint.x += (buttonIncreaseSize.rect.width - 20f);
				IncreasePoint.y += (buttonIncreaseSize.rect.width - 120f);
				buttonIncreaseSize.anchoredPosition = IncreasePoint;


			}

		}

		public void SetStatus(bool status)
		{
			_elements.SetActive(status);
		}
		
		private void Confirm()
		{
			if(Building.buildInstance != null && UI_Main.instance._grid.CanPlaceBuilding(Building.buildInstance, Building.buildInstance.currentX,Building.buildInstance.currentY))
			{
				if(UI_Main.instance._grid.IsPathToPlayerStart(0, 0,Building.buildInstance.currentX, Building.buildInstance.currentY))
				{
					Packet packet = new Packet();
					packet.Write((int)Player.RequestId.BUILD);
					packet.Write(SystemInfo.deviceUniqueIdentifier);
					packet.Write(Building.buildInstance.id.ToString());
					packet.Write(Building.buildInstance.currentX);
					packet.Write(Building.buildInstance.currentY);
					packet.Write(Building.buildInstance.yRotation);
					packet.Write(Building.buildInstance.size);

				/*
					if(UI_WarLayout.instance.isActive)
					{
						packet.Write(3);
					}
					else
					{
						packet.Write(1);
					}
					*/
					packet.Write(UI_WarLayout.instance.isActive ? 2 : 1);
					packet.Write(UI_WarLayout.instance.placingID);
					if(UI_WarLayout.instance.isActive && UI_WarLayout.instance.placingItem != null)
					{
						Destroy(UI_WarLayout.instance.placingItem);
						UI_WarLayout.instance.placingItem = null;
					}

					Sender.TCP_Send(packet);
					if(Building.buildInstance.id == Data.BuildingID.dungeonwall)
					{
						Building.buildInstance.MoveToNextOnGrid(moveX,moveY);
						Building.buildInstance.PlacedOnGrid(Building.buildInstance.currentX,Building.buildInstance.currentY);
						
					}
					else
					{
						Cancel();
					}				
				}
			}
		}

		private void IncreaseSize()
		{
			if(Building.buildInstance.id == Data.BuildingID.dungeonbouldertrap)
			{
				Building.buildInstance.IncreaseSize();
			}
		}
		private void RotateClockwise()
		{
			Building.buildInstance.RotateBuildingClockwise(90, buttonRotateClockwise.transform.position);
		}

		private void MoveRight()
		{
			moveX = 1;
			moveY = 0;
			Building.buildInstance.MoveToNextOnGrid(moveX,moveY);

		}
		private void MoveUp()
		{
			moveX = 0;
			moveY = 1;
			Building.buildInstance.MoveToNextOnGrid(moveX,moveY);
		}
		private void MoveLeft()
		{
			moveX = -1;
			moveY = 0;
			Building.buildInstance.MoveToNextOnGrid(moveX,moveY);
		}
		private void MoveDown()
		{
			moveX = 0;
			moveY = -1;
			Building.buildInstance.MoveToNextOnGrid(moveX,moveY);
		}
		public void Cancel()
		{
			if(Building.buildInstance != null)
			{
				CameraController.instance.isPlacingBuilding = false;
				Building.buildInstance.RemovedFromGrid();
				if(UI_WarLayout.instance.isActive && UI_WarLayout.instance.placingItem != null)
				{
					UI_WarLayout.instance.placingItem.SetActive(true);
					UI_WarLayout.instance.placingItem = null;
				}
			}
		}

		
	}
}