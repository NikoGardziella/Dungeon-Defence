namespace DungeonDefence
{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using DevelopersHub.RealtimeNetworking.Client;
    using Pathfinding;

    public class Ui_Debug : MonoBehaviour
    {



        private static Ui_Debug _instance = null; public static Ui_Debug instance {get {return _instance; } }
        [SerializeField] public GameObject _elements = null;
        [SerializeField] private Button _closeButton = null;
        public Button OpenDebugButton = null;
        public bool IsDebugging = false;
        public int AnotherAccountID;
        [SerializeField] private TMP_InputField _inputId = null;
        [SerializeField] private Button _buttonConfirmId = null;
        [SerializeField] private TMP_InputField _inputDeviceId = null;
        [SerializeField] private Button _buttonConfirmDeviceId = null;

		public List<GameObject> debugUnitsOnGrid = new List<GameObject>();
		public GameObject debugTilePrefab;

        
        private void Start()
        {
            _closeButton.onClick.AddListener(Close);
            _buttonConfirmId.onClick.AddListener(LoginToAnotherAccount);
            _buttonConfirmDeviceId.onClick.AddListener(SyncAnotherDevice);
            
            OpenDebugButton.onClick.AddListener(OpenDebugger);
        }

        private void Awake()
        {
            _instance = this;
            _elements.SetActive(false);
        }


        public void OpenDebugger()
        {
            _elements.SetActive(true);
            _buttonConfirmId.interactable = true;
            _inputId.interactable = true;
            IsDebugging = true;
            
        }

        public void Close()
        {
            IsDebugging = false;
            _elements.SetActive(false);
            UI_Main.instance._grid.Clear();
            UI_Main.instance._grid.ClearUnits();
            UI_Main.instance._grid.ClearDungeonUnits();
            ClearDebugTiles();
            Player.instance.SendSyncRequest();

        }

        public void ClearDebugTiles()
        {
            foreach (var tile in debugUnitsOnGrid)
            {
                Destroy(tile.gameObject);
            }
            debugUnitsOnGrid.Clear();
            for (int i = 0; i < debugUnitsOnGrid.Count; i++)
            {
              //  debugUnitsOnGrid.Remove(debugUnitsOnGrid[i]);
               // Destroy(debugUnitsOnGrid[i].gameObject);
            }
            //debugUnitsOnGrid.Clear();
        }

        private void LoginToAnotherAccount()
        {
            int.TryParse(_inputId.text, out int id);
            if (id != 0)
            {

                _buttonConfirmId.interactable = false;
                _inputId.interactable = false;
                
                
                
            }
        }

        private void SyncAnotherDevice()
        {
            int.TryParse(_inputDeviceId.text, out int id);
            if(id != 0)
            {
                UI_Main.instance._grid.Clear();
                UI_Main.instance._grid.ClearUnits();
                UI_Main.instance._grid.ClearDungeonUnits();
                AnotherAccountID = id;
                IsDebugging = true;
                Player.instance.SendSyncRequest();
                Debug.Log("another account is: " + AnotherAccountID);
            }
           
        }

        public void DrawBlockedTiles(int x, int y)
        {
            if(Ui_Debug.instance.IsDebugging)
            {
                Vector3 tempPos;
                GameObject debugTile;				
                tempPos = UI_Main.instance._grid.transform.TransformPoint(new Vector3(x,0, y)); 
                debugTile = Instantiate(debugTilePrefab, tempPos, Quaternion.identity);
                debugUnitsOnGrid.Add(debugTile);
            }
        }

        public void DrawPath(List<Pathfinding.Cell> points)
        {
            if(Ui_Debug.instance.IsDebugging)
            {
                //BattleUnit prefab = UI_Main.instance.GetBattleUnitPrefab(Data.UnitID.barbarian);
                //BattleUnit unit = new BattleUnit();
                //unit = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                for (int i = 0; i < points.Count; i++)
                {
                    Vector3 tempPos;
                    GameObject debugTile;				
                    tempPos = UI_Main.instance._grid.transform.TransformPoint(new Vector3(points[i].Location.X,0, points[i].Location.Y)); 
                    debugTile = Instantiate(debugTilePrefab, tempPos, Quaternion.identity);
                    debugUnitsOnGrid.Add(debugTile);
                }
                
            }
        }

        public class DebugUnit
		{
			public Data.Unit unit = null;
			public double pathTime = 0;
			public double pathTraveledTime = 0;
        }
    }

}