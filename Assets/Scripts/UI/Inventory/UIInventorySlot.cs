using System;
using PlayerInventory;
using PlayerInventory.Scriptable;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    
    public class UIInventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image _itemIcon;
        [SerializeField] private Image _placeholder;
        [SerializeField] private Sprite _placeholderImage;
        
        private Item _item;
        private UIInventory _uiInventory;

        public ItemType itemType = ItemType.Item;
        public SlotType slotType = SlotType.Item;
        private void Start()
        {
            _placeholder.sprite = _placeholderImage;
            if (_placeholderImage)
                _placeholder.enabled = true;

            _uiInventory = UIInventory.Instance;
        }

        public Item Item
        {
            get
            {
                Item res = _item;
                Item = null;
                return res;
            }

            set
            {
                _item = value;
                
                if (_placeholder.sprite)
                    _placeholder.enabled = (value is null);

                _itemIcon.enabled = (value is not null);
                _itemIcon.sprite = value ? _item.sprite : null;
            }
        }

        public bool IsSuitableType(Item item)
        {
            bool bySlotType = slotType == SlotType.Item || item.type == itemType;
            bool byItemType = (itemType == ItemType.Item || (item is SpecialItem));  // logical expression: (have to be special) -> (is really special)  
            return bySlotType && byItemType;
        }

        public bool IsSpecialSlot => slotType != SlotType.Item;


        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_item)
                _uiInventory.InfoPanel.Show(_item);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_uiInventory.InfoPanel.enabled) return;
        
            _uiInventory.InfoPanel.Hide();
        }
    }
}