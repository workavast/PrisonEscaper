using Character;
using PlayerInventory;
using PlayerInventory.Scriptable;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class DragNDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private UIInventorySlot _beginSlot, _endSlot;
        private UIInventory _uiInventory;
        private Item _beginSlotItem;
        private Image _imageHolder;

        private void Start()
        {
            _uiInventory = UIInventory.Instance;
            _imageHolder = _uiInventory.ImageHolder;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!_uiInventory || !_imageHolder)
                Start();


            GameObject target = eventData.pointerCurrentRaycast.gameObject;
            if (target)
            {
                UIInventorySlot slot = target.transform.parent.GetComponent<UIInventorySlot>();
                if (slot) // in slot
                {
                    _beginSlotItem = slot.Item;
                    if (!_beginSlotItem)
                        return;
                            
                    _beginSlot = slot;
                    _imageHolder.enabled = true;
                    _imageHolder.sprite = _beginSlotItem.sprite;
                    _imageHolder.rectTransform.position = Input.mousePosition;
                }
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            _imageHolder.rectTransform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_beginSlotItem)
                return;
            
            GameObject target = eventData.pointerCurrentRaycast.gameObject;
            if (target)
            {
                UIInventorySlot targetSlot = target.transform.parent.GetComponent<UIInventorySlot>();
                
                if (targetSlot && _beginSlot != targetSlot && targetSlot.IsSuitableType(_beginSlotItem)) // in slot
                {
                    Item targetSlotItem = targetSlot.Item;

                    if (!targetSlotItem || _beginSlot.IsSuitableType(targetSlotItem)) 
                    {
                        var endSlotItem = targetSlot.Item;
                        if (endSlotItem)
                            _beginSlot.Item = endSlotItem;
                        
                        targetSlot.Item = _beginSlotItem;
                        _beginSlot.Item = targetSlotItem;
                        _uiInventory.SwapItemsInInventory(_beginSlot, targetSlot);
                    }
                    else
                    {
                        _beginSlot.Item = _beginSlotItem;
                        targetSlot.Item = targetSlotItem;
                    }
                }
                else // Not in slot
                {
                    _beginSlot.Item = _beginSlotItem;
                }
            }
            else // Not in ui (TODO: may be drop feature later)
            {
                _beginSlot.Item = _beginSlotItem;
            }

            _imageHolder.enabled = false;
            _beginSlotItem = null;
        }
    }
}