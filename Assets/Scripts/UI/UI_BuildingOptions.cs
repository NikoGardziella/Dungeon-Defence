namespace DungeonDefence
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;

	public class UI_BuildingOptions : MonoBehaviour
	{
		[SerializeField] public GameObject _elements = null;
		private static UI_BuildingOptions _instance = null; public static UI_BuildingOptions instance {get {return _instance; } }
		
		public RectTransform infoPanel = null;
		public RectTransform upgradePanel = null;
		public RectTransform instantPanel = null;
		public RectTransform clanPanel = null;

		public RectTransform trainPanel = null;

		public Button infoButton = null;
		public Button upgradeButton = null;
		public Button instantButton = null;
		public Button trainButton = null;
		public Button clanButton = null;



		private void Awake()
		{
			_instance = this;
			_elements.SetActive(false);
		}

		public void SetStatus(bool status)
		{
			if(status && Building.selectedInstance != null)
			{
				infoPanel.gameObject.SetActive(UI_Main.instance.isActive);
				upgradePanel.gameObject.SetActive(Building.selectedInstance.data.isConstructing == false && UI_Main.instance.isActive);
				instantPanel.gameObject.SetActive(Building.selectedInstance.data.isConstructing == true && UI_Main.instance.isActive);
				trainPanel.gameObject.SetActive((Building.selectedInstance.data.id == Data.BuildingID.armycamp || Building.selectedInstance.data.id == Data.BuildingID.barracks) && UI_Main.instance.isActive);
				clanPanel.gameObject.SetActive(Building.selectedInstance.data.id == Data.BuildingID.clancastle && UI_Main.instance.isActive);

			}
			_elements.SetActive(status);
		}
	}
}