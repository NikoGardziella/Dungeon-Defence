namespace DungeonDefence
{
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UI_WarLayoutBuilding : MonoBehaviour
    {

        [SerializeField] private Button _button = null;
        [SerializeField] private TextMeshProUGUI _name = null;
        private long _id = 0;

        private void Start()
        {
            _button.onClick.AddListener(Clicked);
        }
        
        public void Initialized(Data.Building building)
        {
            _id = building.databaseID;
            _name.text = building.id.ToString();
        }

        private void Clicked()
        {
            if (UI_WarLayout.instance.placingItem != null)
            {
                UI_WarLayout.instance.placingItem.SetActive(true);
                UI_WarLayout.instance.placingItem = null;
                UI_Build.instance.Cancel();
            }
            int n = -1;
            for (int i = 0; i < Player.instance.data.buildings.Count; i++)
            {
                if(Player.instance.data.buildings[i].databaseID == _id)
                {
                    n = i;
                    break;
                }
            }
            if(n >= 0)
            {
                Building prefab = UI_Main.instance.GetBuildingPrefab(Player.instance.data.buildings[n].id);
                if (prefab)
                {
                    UI_WarLayout.instance.placingID = Player.instance.data.buildings[n].databaseID;
                    Vector3 position = Vector3.zero;
                    Building building = Instantiate(prefab, position, Quaternion.identity);
                    building.PlacedOnGrid(20, 20); // todo: best avalibale pos
                    building._baseArea.gameObject.SetActive(true);
                    Building.buildInstance = building;
                    CameraController.instance.isPlacingBuilding = true;
                    UI_WarLayout.instance.placingItem = gameObject;
                    UI_WarLayout.instance.placingItem.SetActive(false);
                    UI_Build.instance.SetStatus(true);
                }
            }
        }

    }
}