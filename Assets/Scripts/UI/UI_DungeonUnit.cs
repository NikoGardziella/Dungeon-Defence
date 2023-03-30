namespace DungeonDefence
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using TMPro;
	using DevelopersHub.RealtimeNetworking.Client;

	public class UI_DungeonUnit : MonoBehaviour
	{
		[SerializeField] private Data.UnitID _id = Data.UnitID.barbarian; public Data.UnitID id { get { return _id; } }
		[SerializeField] private Button _button = null;
		public  GameObject unitCard;

		private void Start()
		{
			_button.onClick.AddListener(Clicked);
		}

		private void Clicked()
		{
			Unit prefab = UI_Main.instance.GetUnitPrefab(_id);
			if(prefab)
			{
				//UI_Main.instance.SetStatus(true);

				Vector3 position = Vector3.zero;
				Unit unit = Instantiate(prefab, position, Quaternion.identity);
				unit.PlacedOnGrid(20, 20);
				unit._baseArea.gameObject.SetActive(true);
				Unit.unitInstance = unit;
				CameraController.instance.isPlacingUnit = true;
				UI_PlaceUnit.instance.SetStatus(true);
			}
		}

	}
}