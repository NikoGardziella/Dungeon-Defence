
namespace DungeonDefence
{

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using TMPro;
	using DevelopersHub.RealtimeNetworking.Client;

	public class UI_Unit : MonoBehaviour
	{

		[SerializeField] private Data.UnitID _id = Data.UnitID.barbarian; public Data.UnitID id { get { return _id; } }
		[SerializeField] private Button _button = null;
		[SerializeField] private TextMeshProUGUI _haveUnitsText = null;
		[SerializeField] private TextMeshProUGUI _reqResourceText = null;
		private int count = 0; public int haveCount { get { return count; } set { count = value; _haveUnitsText.text = count.ToString(); } }
		private bool canTrain = false;

		private void Start()
		{
			_button.onClick.AddListener(Clicked);
		}


		public void Initialize(Data.ServerUnit unit)
		{
			int barracksLevel = 0;
			int darkBarracksLevel = 0;
			for (int i = 0; i < Player.instance.data.buildings.Count; i++)
			{
				if(Player.instance.data.buildings[i].id == Data.BuildingID.barracks)
				{
					barracksLevel = Player.instance.data.buildings[i].level;
				}
				else if(Player.instance.data.buildings[i].id == Data.BuildingID.darkbarracks)
				{
					darkBarracksLevel = Player.instance.data.buildings[i].level;
				}
				if(barracksLevel > 0 && darkBarracksLevel > 0)
					break;
			}

			canTrain = Data.IsUnitUnlocked(_id, barracksLevel, darkBarracksLevel);
			_button.interactable = canTrain;

			if (unit.requiredGold > 0)
			{
				_reqResourceText.text = "Gold: " + unit.requiredGold.ToString();
			}
			else if (unit.requiredElixir > 0)
			{
				_reqResourceText.text = "Elixir: " + unit.requiredElixir.ToString();
			}
			else if (unit.requiredGems > 0)
			{
				_reqResourceText.text = "Gems: " + unit.requiredGems.ToString();
			}
			else if (unit.requiredDarkElixir > 0)
			{
				_reqResourceText.text = "Dark: " + unit.requiredDarkElixir.ToString();
			}
			else
			{
				_reqResourceText.text = "Free";
			}
		}
		private void Clicked()
		{
			Packet packet = new Packet();
			packet.Write((int)Player.RequestId.TRAIN);
			packet.Write(_id.ToString());
			Sender.TCP_Send(packet);			
		}


		 public void Sync()
		{
			count = 0;
			for (int i = 0; i < Player.instance.data.units.Count; i++)
			{
				if (Player.instance.data.units[i].id == _id && Player.instance.data.units[i].ready)
				{
					count++;
				}
			}
			haveCount = count;
		}
	}

}