namespace DungeonDefence {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using TMPro;

	public class UI_Bar : MonoBehaviour
	{
		public Image bar = null;
		public RectTransform rect = null;
		public TextMeshProUGUI time = null;
		[HideInInspector]public Building building = null;
		private float max = 1;
		private float min = 0;
		private float value = 0.05f;

		private void Awake()
	   {
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = Vector2.zero;
	   }
		public void SetMin(float input)
		{
			min = input;
			AdjustUI();
		}
		public void SetMax(float input)
		{
			max = input;
			AdjustUI();
		}
		public void SetValue(float input)
		{
			value = input;
			AdjustUI();
		}

		public void AdjustUI()
		{
			bar.fillAmount = value / Mathf.Abs(max - min);
			System.TimeSpan span = building.data.constructionTime - Player.instance.data.nowTime;
			time.text = span.ToString();
		}

	}

}