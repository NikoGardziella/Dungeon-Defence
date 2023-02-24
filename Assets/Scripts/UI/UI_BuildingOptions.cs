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
		public Button infoButton = null;
		public Button upgradeButton = null;

		private void Awake()
		{
			_instance = this;
			_elements.SetActive(false);
		}

		public void SetStatus(bool status)
		{
			if(status && Building.selectedInstance != null)
			{
				infoPanel.gameObject.SetActive(Building.selectedInstance.data.isConstructing == false);
				upgradeButton.gameObject.SetActive(Building.selectedInstance.data.isConstructing == false);

			}
			_elements.SetActive(status);
		}
	}
}