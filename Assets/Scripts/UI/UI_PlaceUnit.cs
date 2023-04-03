namespace DungeonDefence
{

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using DevelopersHub.RealtimeNetworking.Client;

	public class UI_PlaceUnit : MonoBehaviour
	{
		[SerializeField] public GameObject _elements = null;

		public RectTransform buttonConfirm = null;
		public RectTransform buttonCancel = null;


		[HideInInspector] public Button clickConfirmButton = null;
		[HideInInspector] public Button clickCancelButton = null;

		private static UI_PlaceUnit _instance = null; public static UI_PlaceUnit instance {get {return _instance; } }
		int moveX = 0;
		int moveY = 0;
		
		private void Awake()
		{
			_instance = this;
			_elements.SetActive(false);
			clickConfirmButton = buttonConfirm.gameObject.GetComponent<Button>();

			clickCancelButton = buttonCancel.gameObject.GetComponent<Button>();



		}

		private void Start()
		{
			buttonConfirm.gameObject.GetComponent<Button>().onClick.AddListener(Confirm);
			buttonCancel.gameObject.GetComponent<Button>().onClick.AddListener(Cancel);


			buttonConfirm.anchorMin = Vector3.zero;
			buttonConfirm.anchorMax = Vector3.zero;
			buttonCancel.anchorMin = Vector3.zero;
			buttonCancel.anchorMax = Vector3.zero;
		

		}

		private void Update()
		{
			if(Unit.unitInstance != null && CameraController.instance.isPlacingUnit)
			{
				Vector3 end = UI_Main.instance._grid.GetEndPosition(Unit.unitInstance.currentX,Unit.unitInstance.currentY, 1, 1);
				
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
			}
		}
		public void SetStatus(bool status)
		{
			_elements.SetActive(status);
		}
			
		private void Confirm()
		{
			if(Unit.unitInstance != null && UI_Main.instance._grid.CanPlaceUnit(Unit.unitInstance, Unit.unitInstance.currentX, Unit.unitInstance.currentY))
			{
				Packet packet = new Packet();
				packet.Write((int)Player.RequestId.PLACEDUNGEONUNIT);
				packet.Write(SystemInfo.deviceUniqueIdentifier);
				packet.Write(Unit.unitInstance.id.ToString());
				packet.Write(Unit.unitInstance.currentX);
				packet.Write(Unit.unitInstance.currentY);

				if(UI_WarLayout.instance.isActive)
				{
					packet.Write(3);
				}
				else
				{
					packet.Write(1);
				}
				//packet.Write(UI_WarLayout.instance.isActive ? 2 : 1);
				packet.Write(UI_WarLayout.instance.placingID);
				if(UI_WarLayout.instance.isActive && UI_WarLayout.instance.placingItem != null)
				{
					Destroy(UI_WarLayout.instance.placingItem);
					UI_WarLayout.instance.placingItem = null;
				}

				Sender.TCP_Send(packet);
		
				
				Cancel();

			}
		}

	
		public void Cancel()
		{
			if(Unit.unitInstance != null)
			{
				CameraController.instance.isPlacingUnit = false;
				Unit.unitInstance.RemovedFromGrid();
				if(UI_WarLayout.instance.isActive && UI_WarLayout.instance.placingItem != null)
				{
					UI_WarLayout.instance.placingItem.SetActive(true);
					UI_WarLayout.instance.placingItem = null;
				}
			}
		}
	}

}
