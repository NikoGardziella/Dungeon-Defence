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

		public RectTransform buttonConfirm = null;
		public RectTransform buttonCancel = null;
		[HideInInspector] public Button clickConfirmButton = null;

		private static UI_Build _instance = null; public static UI_Build instance {get {return _instance; } }
		
		private void Awake()
		{
			_instance = this;
			_elements.SetActive(false);
			clickConfirmButton = buttonConfirm.gameObject.GetComponent<Button>();
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
			if(Building.buildInstance != null && CameraController.instance.isPlacingBuilding)
			{
				Vector3 end = UI_Main.instance._grid.GetEndPosition(Building.buildInstance);
				
				Vector3 planeDownLeft = CameraController.instance.CameraScreenPositionToPlanePosition(Vector2.zero);
				Vector3 planeTopRight = CameraController.instance.CameraScreenPositionToPlanePosition(new Vector2(Screen.width, Screen.height));

				float w = planeTopRight.x - planeDownLeft.x;
				float h = planeTopRight.z - planeDownLeft.z;

				float endW = end.x - planeDownLeft.x;
				float endH = end.z - planeDownLeft.z;

				Vector2 screenPoint = new Vector2(endW / w * Screen.width, endH / h * Screen.height);

				Vector2 confirmPoint = screenPoint;
				confirmPoint.x += (buttonConfirm.rect.width + 10f);
				buttonConfirm.anchoredPosition = confirmPoint;

				Vector2 cancelPoint = screenPoint;
				confirmPoint.x -= (buttonCancel.rect.width + 10f);
				buttonCancel.anchoredPosition = cancelPoint;
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
				Packet packet = new Packet();
				packet.Write((int)Player.RequestId.BUILD);
				packet.Write(SystemInfo.deviceUniqueIdentifier);
				packet.Write(Building.buildInstance.id);
				packet.Write(Building.buildInstance.currentX);
				packet.Write(Building.buildInstance.currentY);
				Sender.TCP_Send(packet);
				Cancel();
			}
		}
		public void Cancel()
		{
			if(Building.buildInstance != null)
			{
				CameraController.instance.isPlacingBuilding = false;
				Building.buildInstance.RemovedFromGrid();
			}
		}
	}
}