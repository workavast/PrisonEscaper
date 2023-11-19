using System.Collections.Generic;
using System.Linq;
using PlayerInventory.Scriptable;
using UnityEngine;

namespace PlayerInventory
{
    public class InventoryController : MonoBehaviour
    {
        private static InventoryController _instance;

        [SerializeField] private int inventorySize;
        [SerializeField] private Item[] items;
        [SerializeField] private List<SpecialItem> specialSlots;

        private void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(this);

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