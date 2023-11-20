namespace DungeonDefence
{
    using System.Collections;
    using System.Collections.Generic;
    using Unity.Mathematics;
    using UnityEngine;
    using UnityEngine.UI;

    public class UI_Character : MonoBehaviour
    {

        private static UI_Character _instance = null; public static UI_Character instance {get {return _instance; } }
		private bool _active = true; public bool isActive {get { return _active; } }

        [SerializeField] public GameObject _elements = null;
        [SerializeField] private Button _closeButton = null;
        [SerializeField] public UI_Item[] _itemPrefab = null;
        [SerializeField] public UI_ItemCard[] _itemCardPrefab = null;
        public UI_Item[] _playerItemPrefab = null;

        public GameObject _contentItems = null;
        public UI_Player playerPrefab;
        public GameObject _playerParent;
        [SerializeField] public RectTransform _itemHeadWearPos = null;
        [SerializeField] public RectTransform _itemMeleeWeaponPos = null;


        public List<UI_ItemCard> _itemCards = null;
        public List<UI_Item> _items = null;


   
            private void Awake()
            {
                _instance = this;
                //_elements.SetActive(false);
            }

            private void Start()
            {
                _closeButton.onClick.AddListener(CloseShop);
                debugMakeItemCard();
            }
            public void SetStatus(bool status)
            {
                _active = status;
                if(_active == true)
                {
                    InitPlayer();
                }
                _elements.SetActive(status);
            }

            private void CloseShop()
            {
                SetStatus(false);
                
                UI_Main.instance.SetStatus(true);
		    }

            void InitPlayer()
            {
                Instantiate(playerPrefab,_playerParent.transform);
            }

            void debugMakeItemCard()
            {
                for (int i = 0; i < 10; i++)
                {
                    UI_ItemCard characterItem;
                    UI_ItemCard prefab = _itemCardPrefab[0];
                    characterItem = Instantiate(prefab, Vector3.zero, Quaternion.identity, _contentItems.transform);
                    characterItem.databaseID = i;
                    if(i % 2 == 0)
                        characterItem.itemCategory = Data.CharacterItemCategory.HeadGear;
                    else
                        characterItem.itemCategory = Data.CharacterItemCategory.MeleeWeapon;

                    _itemCards.Add(characterItem);


                }
            }

            public void DataSynced()
            {
                if (Player.instance.data.characterItems != null && Player.instance.data.characterItems.Count > 0)
                {
                    for (int i = 0; i < Player.instance.data.characterItems.Count; i++)
                    {
                            UI_ItemCard characterItem = GetCharacterItem(Player.instance.data.characterItems[i].databaseID);
                            if (characterItem != null)
                            {
                                
                            }
                            else
                            {
                                UI_ItemCard prefab = GetItemPrefab(Player.instance.data.characterItems[i].itemCategory);
                                if (prefab)
                                {
                                    characterItem = Instantiate(prefab, Vector3.zero, Quaternion.identity, _contentItems.transform);

                                    characterItem.databaseID = Player.instance.data.characterItems[i].databaseID;
                                   
                                    //Debug.Log("building id: " + building.data.id);
                                   _itemCards.Add(characterItem);
                                }
                            }
                    }
                }
            }

            public UI_ItemCard GetCharacterItem(long databaseID)
            {
                for (int i = 0; i < _itemCards.Count; i++)
                {
                    if(_itemCards[i].databaseID == databaseID)
                    {
                        return _itemCards[i];
                    }
                }
                return null;
            }

            public UI_ItemCard GetItemPrefab(Data.CharacterItemCategory itemCategory)
            {
                for (int i = 0; i < _itemCardPrefab.Length; i++)
                {
                    if(_itemCardPrefab[i].itemCategory == itemCategory)
                    {
                        return _itemCardPrefab[i];
                    }
                }
                return null;
            }

            public void RemoveItem(long databaseId)
            {
                for (int i = 0; i < _items.Count; i++)
                {
                    if(_items[i].databaseID == databaseId)
                    {
                        _items.Remove(_items[i]);
                    }
                }
            }

            public void UnequipCard(long databaseId)
            {
                for (int i = 0; i < _itemCards.Count; i++)
                {
                    _itemCards[i].isEquipped = false;
                }
            }

             private void EquipItemToPlauer(UI_Item item)
            {
                    
            }
    }
}
