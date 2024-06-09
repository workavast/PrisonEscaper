using System.Collections.Generic;
using PlayerInventory.Scriptable;
using UnityEngine;

namespace PlayerInventory
{
    public class InventoryController : MonoBehaviour
    {
        [SerializeField] private int inventorySize;
        [SerializeField] private Item[] items;
        [SerializeField] private List<SpecialItem> specialSlots;

        private void Awake()
        {
            Inventory.Init(inventorySize);
            Inventory.OnBagChanged.AddListener(UpdateInventoryItems);
        }

        private void UpdateInventoryItems(Item[] inventoryItems)
        {
            items = inventoryItems;

            specialSlots = new List<SpecialItem>(Inventory.SpecialSlots.Values);
        }
    }
}