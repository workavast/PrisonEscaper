using System;
using GameCode.UI.Elements;
using PlayerInventory;
using PlayerInventory.Scriptable;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class UI_Slot : MonoBehaviour, 
        IPointerEnterHandler, IPointerExitHandler, 
        IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private UI_Rarity uiRarity;
        [SerializeField] private GameObject litFill;
        [SerializeField] private Image itemIcon;
        [SerializeField] private Image placeholder;
        
        [field: SerializeField] public ItemType ItemType { get; private set; } = ItemType.Item;
        [field: SerializeField] public SlotType SlotType { get; private set; } = SlotType.Item;
        
        public bool IsSpecialSlot => SlotType != SlotType.Item;
        public Item Item { get; private set; }
        
        private UI_Inventory _uiInventory;

        public event Action<UI_Slot> OnBeginDragEvent;
        public event Action OnDragEvent;
        public event Action<UI_Slot, UI_Slot> OnEndDragEvent;
        public event Action<UI_Slot> OnPointerEnterEvent;
        public event Action OnPointerExitEvent;
        
        private void Awake()
        {
            if(placeholder.sprite)
                placeholder.enabled = !Item;
        }

        public bool IsSuitableType(Item item)
        {
            bool bySlotType = SlotType == SlotType.Item || item.Type == ItemType;
            bool byItemType = (ItemType == ItemType.Item || (item is SpecialItem));  // logical expression: (have to be special) -> (is really special)  
            return bySlotType && byItemType;
        }

        public void SetItem(Item newItem)
        {
            Item = newItem;

            if (placeholder.sprite)
                placeholder.enabled = (newItem is null);

            itemIcon.enabled = (newItem is not null);
            itemIcon.sprite = newItem ? Item.Sprite : null;

            if (newItem != null)
                uiRarity.SetRarity(newItem.Rarity);
            else
                uiRarity.Hide();
        }

        public void ToggleItemVisible(bool showItem)
        {
            if(showItem)
                itemIcon.color = new Color(1, 1, 1, 1);
            else
                itemIcon.color = new Color(1, 1, 1, 0);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (Item)
                OnPointerEnterEvent?.Invoke(this);
            
            litFill.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnPointerExitEvent?.Invoke();
            litFill.SetActive(false);
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!Item) return;
            
            OnBeginDragEvent?.Invoke(this);
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            OnDragEvent?.Invoke();
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            GameObject target = eventData.pointerCurrentRaycast.gameObject;
            if (!target)
            {
                OnEndDragEvent?.Invoke(this, null);
                return;
            }
            
            UI_Slot targetSlot = target.transform.GetComponent<UI_Slot>();
            if (!targetSlot || this == targetSlot || !targetSlot.IsSuitableType(Item))
            {
                OnEndDragEvent?.Invoke(this, this);
                return;
            }
            
            OnEndDragEvent?.Invoke(this, targetSlot);
        }
    }
}