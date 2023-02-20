namespace DungeonDefence
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using TMPro;
	using UnityEngine.UI;

	public class UI_Main : MonoBehaviour
	{
		[SerializeField] public TextMeshProUGUI _goldText = null;
		[SerializeField] public TextMeshProUGUI _elixirText = null;
		[SerializeField] public TextMeshProUGUI _gemsText = null;

		[SerializeField] public Button _shopButton = null;

		private static UI_Main _instance = null; public static UI_Main instance {get {return _instance; } }


		private void Awake()
		{
			_instance = this;
		}

		private void Start()
		{
			_shopButton.onClick.AddListener(ShopButtonClicked);
		}

		private void ShopButtonClicked()
		{

		}
	}

}
