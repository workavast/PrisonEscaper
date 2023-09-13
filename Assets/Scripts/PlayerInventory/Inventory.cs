using System;
using System.Collections.Generic;
using Character;
using PlayerInventory.Scriptable;
using UnityEngine;
using UnityEngine.Events;

namespace PlayerInventory
{
    [Serializable]
    public class Inventory 
    {
        // [SerializeField] private int _bagCapacity;
        [field: SerializeField] public int Size { get; private set; }
        [SerializeField] private Item[] _bagItems;


        public UnityEvent<Item[]> onBagChanged;
        
        public Dictionary<SlotType, SpecialItem> SpecialSlots;

        public void Init()
        {
            _bagItems = new Item[Size];
            onBagChanged = new UnityEvent<Item[]>();
            SpecialSlots = new Dictionary<SlotType, SpecialItem>()
            {
                { SlotType.Weapon , null},
                { SlotType.Shield , null},
                { SlotType.Amulet , null},
                { SlotType.Lring , null},
                { SlotType.Rring , null},
                { SlotType.Artifact , null},
                { SlotType.HotBarSlot1 , null},
                { SlotType.HotBarSlot2 , null},
                { SlotType.HotBarSlot3 , null},
                { SlotType.HotBarSlot4 , null},
            };
        }
        
        
        public void AddItem(Item item)
        {
            for (int i = 0; i < _bagItems.Length; i++)
            {
                if (_bagItems[i] is null)
                {
                    _bagItems[i] = item;
                    break;
                }
            }
            onBagChanged.Invoke(_bagItems);
        }

        public Item DeleteItem(int indexInBag)
        {
            Item item = _bagItems[indexInBag];
            _bagItems[indexInBag] = null;
            return item;
        }

        public bool HasBagEmptySlot()
        {
            bool res = false;
            foreach (var item in _bagItems)
            {
                if (!item)
                {
                    res = true;
                    break;
                }
            }

            return res;
        }

        public void UseItem(Item item)
        {
            if (item is IUsable)
            {
                var usableItem = item as IUsable;
                usableItem.UseEffect();
            }
        }

        public void ApplySpecial(int indexFrom, SlotType slotType)
        {
            if (SpecialSlots[slotType] != null)
            {
                Player.Instance.RemoveItemStats(SpecialSlots[slotType]!.MainStats, SpecialSlots[slotType]!.AttackStats, SpecialSlots[slotType]!.ResistStats);
            }
                
            (_bagItems[indexFrom], SpecialSlots[slotType]) = (SpecialSlots[slotType], _bagItems[indexFrom] as SpecialItem);
            Player.Instance.ApplyItemStats(SpecialSlots[slotType]!.MainStats, SpecialSlots[slotType]!.AttackStats, SpecialSlots[slotType]!.ResistStats);
        }

        public void RemoveSpecial(int indexTo, SlotType slotType)
        {
            if (SpecialSlots[slotType] != null)
            {
                Player.Instance.RemoveItemStats(SpecialSlots[slotType]!.MainStats, SpecialSlots[slotType]!.AttackStats, SpecialSlots[slotType]!.ResistStats);
            }
            
            (_bagItems[indexTo], SpecialSlots[slotType]) = (SpecialSlots[slotType], _bagItems[indexTo] as SpecialItem);
            
            if(SpecialSlots[slotType] != null)
                Player.Instance.ApplyItemStats(SpecialSlots[slotType]!.MainStats, SpecialSlots[slotType]!.AttackStats, SpecialSlots[slotType]!.ResistStats);
        }

        public void SwapBagItems(int from, int to)
        {
            (_bagItems[from], _bagItems[to]) = ( _bagItems[to], _bagItems[from]);
        }


        public void SwapSpecials(SlotType from, SlotType to)
        {
            (SpecialSlots[from], SpecialSlots[to]) = (SpecialSlots[to], SpecialSlots[from]);
        }
    }
}