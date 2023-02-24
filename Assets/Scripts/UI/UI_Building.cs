namespace DungeonDefence
{

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using DevelopersHub.RealtimeNetworking.Client;


	public class UI_Building : MonoBehaviour
	{
		[SerializeField] private Data.BuildingID _id = Data.BuildingID.townhall;
		[SerializeField] private Button _button = null;

		private void Start()
		{
			_button.onClick.AddListener(Clicked);
		}

		private void Clicked()
		{

			Building prefab = UI_Main.instance.GetBuildingPrefab(_id);
			if(prefab)
			{

				UI_Shop.instance.SetStatus(false);
				UI_Main.instance.SetStatus(true);

				Vector3 position = Vector3.zero;
				Building building = Instantiate(prefab, position, Quaternion.identity);
				building.PlacedOnGrid(20, 20);
				building._baseArea.gameObject.SetActive(true);
				Building.buildInstance = building;
				CameraController.instance.isPlacingBuilding = true;
				UI_Build.instance.SetStatus(true);
			}
			
		}
	}

}
