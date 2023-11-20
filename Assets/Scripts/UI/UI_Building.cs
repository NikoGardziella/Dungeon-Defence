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
		public  GameObject buildingCard;
		

		private static UI_Building _instance = null; public static UI_Building instance {get {return _instance; } }

		private void Start()
		{
			_button.onClick.AddListener(Clicked);
		}

		
		private void Clicked()
		{
			if(Building.buildInstance != null)
            {
                Building.buildInstance.RemovedFromGrid();
            }
			if(Unit.unitInstance != null)
			{
				Unit.unitInstance.RemovedFromGrid();
			}
			//buildingCard.gameObject.transform.localScale += new Vector3(1,1,0);
			Building prefab = UI_Main.instance.GetBuildingPrefab(_id);
			if(prefab)
			{

				UI_Shop.instance.SetStatus(false);
				UI_Main.instance.SetStatus(true);

				Vector3 position = Vector3.zero;
				Building building = Instantiate(prefab, position, Quaternion.identity);
				building.PlacedOnGrid(UI_WarLayout.instance.lastX, UI_WarLayout.instance.lastY);
				building._baseArea.gameObject.SetActive(true);
				Building.buildInstance = building;
				CameraController.instance.isPlacingBuilding = true;
				UI_Build.instance.SetStatus(true);
			}
			
		}

		

		
	}
}


