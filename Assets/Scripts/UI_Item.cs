using System.Collections;
using System.Collections.Generic;
using DungeonDefence;
using UnityEngine;
using UnityEngine.UI;

public class UI_Item : MonoBehaviour
{

      [SerializeField] public List<UI_Item> equippedItems = null;

      [SerializeField] public Data.CharacterItemCategory itemCategory = Data.CharacterItemCategory.HeadGear;
      [SerializeField] private long _databaseID = 1; public long databaseID  { get { return _databaseID; } set { _databaseID = value; }  }
	private static UI_Item _selectedInstance = null; public static UI_Item selectedInstance {get {return _selectedInstance; } set { _selectedInstance = value;} }
      private static UI_Item _instance = null; public static UI_Item instance {get {return _instance; } }

      public bool isEquipped = false;
      [SerializeField] private Button _button = null;
      private float timer = 0;
      private float secondClickTime = 1.0f;
      private bool clicked = false;
      private bool doubleClicked = false;

      public Data.HeadGearId headGearId = Data.HeadGearId.None;
      public Data.MeleeWepaonId meleeWepaonId = Data.MeleeWepaonId.None;

      private void Update()
      {
            if(clicked == true)
            {
                  //Debug.Log("clicked");

                  doubleClicked = true;
                  timer += Time.deltaTime;
                  
                  if(timer > secondClickTime)
                  {
                        doubleClicked = false;
                        clicked = false;
                        timer = 0;
                  }
            }
           // clicked = true;
      }
      private void Awake()
      {
            _instance = this;
      }
      private void Start()
      {
            _button.onClick.AddListener(Clicked);
            
      }

      private void Clicked()
      {
            if(doubleClicked == true)
            {
                  DoubleClicked();
            }
            clicked = true;
            selectedInstance = this;
            Debug.Log("clicked item" + selectedInstance.databaseID);
      }

      public void DoubleClicked()
      {
            Debug.Log(databaseID + " was double clicked");
            if(selectedInstance.isEquipped)
            {
                  RemoveEquidpedCard(selectedInstance);
            }

      }

      private void RemoveEquidpedCard(UI_Item item)
      {
            selectedInstance = null;
            UI_Character.instance.RemoveItem(item.databaseID);
            UI_Character.instance.UnequipCard(item._databaseID);
            Destroy(gameObject);
      }
}
