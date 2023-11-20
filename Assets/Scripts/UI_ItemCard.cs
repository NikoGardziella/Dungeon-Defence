using System.Collections;
using System.Collections.Generic;
using DungeonDefence;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_ItemCard : MonoBehaviour
{
       public Data.CharacterItemCategory itemCategory = Data.CharacterItemCategory.HeadGear;
      [SerializeField] private long _databaseID = 1; public long databaseID  { get { return _databaseID; } set { _databaseID = value; }  }
      public bool isEquipped = false;
      private static UI_ItemCard _selectedInstance = null; public static UI_ItemCard selectedInstance {get {return _selectedInstance; } set { _selectedInstance = value;} }
      private static UI_ItemCard _instance = null; public static UI_ItemCard instance {get {return _instance; } }

      [SerializeField] private Button _button = null;

      private void Awake()
      {
            _instance = this;
      }
      private void Start()
      {
            _button.onClick.AddListener(Clicked);
            
      }
      private float timer = 0;
      private float secondClickTime = 1.0f;
      private bool clicked = false;
      private bool doubleClicked = false;
      

      private void Clicked()
      {
            if(doubleClicked == true)
            {
                  DoubleClicked();
            }
            clicked = true;
            selectedInstance = this;
            Debug.Log("clicked" + selectedInstance.databaseID);
            SelectCard();
      }

      public void DoubleClicked()
      {
            Debug.Log(databaseID + " was double clicked");
            if(!selectedInstance.isEquipped)
            {
                  EquidCard(selectedInstance);
            }

      }

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

      private void SelectCard()
      {

      }

      private void EquidCard(UI_ItemCard card)
      {
            UI_Item characterItem;
            UI_Item prefab = UI_Character.instance._itemPrefab[0];
            Transform parent = null;
            if(card.itemCategory == Data.CharacterItemCategory.HeadGear)
            {
                  parent = UI_Character.instance._itemHeadWearPos;
                  prefab.headGearId = Data.HeadGearId.Testhat;
            }
            else if(card.itemCategory == Data.CharacterItemCategory.MeleeWeapon)
            {
                  parent = UI_Character.instance._itemMeleeWeaponPos;
            }

            characterItem = Instantiate(prefab, parent);
            
            characterItem.databaseID = card.databaseID;
            
            characterItem.itemCategory = card.itemCategory;
            card.isEquipped = true;
            characterItem.isEquipped = card.isEquipped;
            
            UI_Character.instance._items.Add(characterItem);
      }

     



}


