using System;
using System.Collections.Generic;
using Character;
using Core;
using PlayerInventory.Scriptable;
using UnityEngine.Events;

namespace PlayerInventory
{
    public static class Inventory
    {
        public static int Size { get; private set; }
        
        private static Item[] _bagItems;
        public static Item[] Items => _bagItems;

        public static UnityEvent<Item[]> OnBagChanged;
        
        public static Dictionary<SlotType, SpecialItem> SpecialSlots;

        public static void Init(int startSize)
        {
            Size = startSize;
            _bagItems = new Item[Size];

            OnBagChanged = new UnityEvent<Item[]>();
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

        public static void AddItem(Item item)
        {
            for (int i = 0; i < _bagItems.Length; i++)
            {
                if (_bagItems[i] is null)
                {
                    _bagItems[i] = item;
                    break;
                }
            }
            
            OnBagChanged.Invoke(_bagItems);
        }

        public static Item RemoveItem(int indexInBag)
        {
            Item item = _bagItems[indexInBag];
            _bagItems[indexInBag] = null;
            return item;
        }

        public static Item RemoveSpecialItem(SlotType slotType)
        {
            Item item;

#if UNITY_EDITOR
            if(!SpecialSlots.ContainsKey(slotType)) throw new Exception("Special slot is undefined");
            if(SpecialSlots[slotType] is null) throw new Exception("Special item in the slot is null");
#endif
            
            item = SpecialSlots[slotType];
            Player.Instance.RemoveItemStats(SpecialSlots[slotType]!.MainStats, SpecialSlots[slotType]!.AttackStats, SpecialSlots[slotType]!.ResistStats);
            SpecialSlots[slotType] = null;
            
            return item;
        }
        
        public static bool HasBagEmptySlot()
        {
            foreach (var item in _bagItems)
                if (!item)
                    return true;

            return false;
        }

        public static void UseItem(Item item)
        {
            if (item is IUsable)
            {
                var usableItem = item as IUsable;
                usableItem.UseEffect();
            }
        }

        public static void ApplySpecial(int indexFrom, SlotType slotType)
        {
            if (SpecialSlots[slotType] != null)
            {
                Player.Instance.RemoveItemStats(SpecialSlots[slotType]!.MainStats, SpecialSlots[slotType]!.AttackStats, SpecialSlots[slotType]!.ResistStats);
            }
                
            (_bagItems[indexFrom], SpecialSlots[slotType]) = (SpecialSlots[slotType], _bagItems[indexFrom] as SpecialItem);
            Player.Instance.ApplyItemStats(SpecialSlots[slotType]!.MainStats, SpecialSlots[slotType]!.AttackStats, SpecialSlots[slotType]!.ResistStats);
        }

        public static void RemoveSpecial(int indexTo, SlotType slotType)
        {
            if (SpecialSlots[slotType] != null)
            {
                Player.Instance.RemoveItemStats(SpecialSlots[slotType]!.MainStats, SpecialSlots[slotType]!.AttackStats, SpecialSlots[slotType]!.ResistStats);
            }
            
            (_bagItems[indexTo], SpecialSlots[slotType]) = (SpecialSlots[slotType], _bagItems[indexTo] as SpecialItem);
            
            if(SpecialSlots[slotType] != null)
                Player.Instance.ApplyItemStats(SpecialSlots[slotType]!.MainStats, SpecialSlots[slotType]!.AttackStats, SpecialSlots[slotType]!.ResistStats);
        }

        public static void SwapBagItems(int from, int to)
        {
            (_bagItems[from], _bagItems[to]) = ( _bagItems[to], _bagItems[from]);
        }


        public static void SwapSpecials(SlotType from, SlotType to)
        {
            (SpecialSlots[from], SpecialSlots[to]) = (SpecialSlots[to], SpecialSlots[from]);
        }
    }
}