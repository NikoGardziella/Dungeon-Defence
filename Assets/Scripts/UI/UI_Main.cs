namespace DungeonDefence
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using TMPro;
	using UnityEngine.UI;

	public class UI_Main : MonoBehaviour
	{
		[SerializeField] public GameObject _elements = null;

		[SerializeField] public TextMeshProUGUI _goldText = null;
		[SerializeField] public TextMeshProUGUI _elixirText = null;
		[SerializeField] public TextMeshProUGUI _gemsText = null;

		[SerializeField] public Button _shopButton = null;

		[SerializeField] public BuildGrid _grid = null;
		[SerializeField] public Building[] _buildingPrefabs = null;
		private static UI_Main _instance = null; public static UI_Main instance {get {return _instance; } }
		private bool _active = true; public bool isActive {get { return _active; } }

		private void Awake()
		{
			_instance = this;
			_elements.SetActive(true);

		}

		private void Start()
		{
			_shopButton.onClick.AddListener(ShopButtonClicked);
		}

		private void ShopButtonClicked()
		{
			_active = false;
			UI_Shop.instance.SetStatus(true);
			_elements.SetActive(false);
			SetStatus(false);
		}

		public void SetStatus(bool status)
		{
			_active = status;
			_elements.SetActive(status);
		}
	}

}
