using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    [Serializable]
    public class DragAndDropController
    {
        [SerializeField] private DragAndDropHolder dragAndDropHolder;

        private List<UI_Slot> _slots = new List<UI_Slot>();
        
        public event Action<UI_Slot> OnDropItem;
        public event Action<UI_Slot, UI_Slot> OnSwapItemsInInventory;

        public void RegisterSlots(IEnumerable<UI_Slot> newSlots)
        {
            foreach (var newSlot in newSlots)
                RegisterSlot(newSlot);
        }
        
        public void RegisterSlot(UI_Slot newSlot)
        {
            newSlot.OnBeginDragEvent += StartDrag;
            newSlot.OnDragEvent += Drag;
            newSlot.OnEndDragEvent += EndDrag;
            
            _slots.Add(newSlot);
        }

        private void StartDrag(UI_Slot startSlot)
        {
            dragAndDropHolder.ToggleVisible(true);
            dragAndDropHolder.SetSprite(startSlot.Item.Sprite);
            dragAndDropHolder.transform.position = Input.mousePosition;
            startSlot.ToggleItemVisible(false);
        }

        private void Drag()
        {
            dragAndDropHolder.transform.position = Input.mousePosition;
        }

        private void EndDrag(UI_Slot startSlot, UI_Slot endSlot)
        {
            if (!startSlot)
                throw new Exception("Start slot is null");
            
            if (!endSlot)
            {
                OnDropItem?.Invoke(startSlot);
            }
            else if (!endSlot.Item || startSlot.IsSuitableType(endSlot.Item))
            {
                var startItem = startSlot.Item;
                var endItem = endSlot.Item;
                
                
                startSlot.SetItem(endItem);
                endSlot.SetItem(startItem);
                OnSwapItemsInInventory?.Invoke(startSlot, endSlot);
            }

            startSlot.ToggleItemVisible(true);
            dragAndDropHolder.ToggleVisible(false);
        }
    }
}