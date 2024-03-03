using System.Collections.Generic;
using Character;
using PlayerInventory;
using PlayerInventory.Scriptable;
using SerializableDictionaryExtension;
using UnityEngine;
using Zenject;

namespace UI
{
    public class UI_Inventory : UI_ScreenBase
    {
        [SerializeField] private GameObject bagSlotsParent;
        [SerializeField] private GameObject slotPrefab;
        [SerializeField] private GameObject collectablePrefab;
        [SerializeField] private DragAndDropController dragAndDropController;
        [SerializeField] private UI_ItemInfoPanel infoPanel;
        [SerializeField] private SerializableDictionary<SlotType, UI_Slot> specialSlots;

        [Inject] private VolumeProfileController _volumeProfileController;
        [Inject] private KeyboardObserver _keyboardObserver;

        private int _backpackSlotsCount;
        private List<UI_Slot> _backpackCells;
        
        public override void InitScreen()
        {
            base.InitScreen();
            //TODO: move inventory opening/closing in other class (maybe in UI_Controller??, not sure)
            _keyboardObserver.OnInventory += ToggleActive;
        }

        private void Awake()
        {
            dragAndDropController.OnDropItem += DropItem;
            dragAndDropController.OnSwapItemsInInventory += SwapItemsInPlayerInventory;
        }

        private void Start()
        {
            _backpackSlotsCount = Inventory.Size;
            _backpackCells = new List<UI_Slot>(new UI_Slot[_backpackSlotsCount]);
            Inventory.OnBagChanged.AddListener(UpdateBackpack);

            InitBag();

            UpdateBackpack(Inventory.Items);
            UpdateSpecialSlots();
        }

        private void InitBag()
        {
            foreach (var slot in specialSlots.Values)
            {
                slot.OnPointerEnterEvent += infoPanel.Show;
                slot.OnPointerExitEvent += infoPanel.Hide;
            }
            
            for (int i = 0; i < _backpackSlotsCount; i++)
            {
                if (!_backpackCells[i])
                {
                    var slot = Instantiate(slotPrefab, bagSlotsParent.transform).GetComponent<UI_Slot>();
                    slot.OnPointerEnterEvent += infoPanel.Show;
                    slot.OnPointerExitEvent += infoPanel.Hide;
                    _backpackCells[i] = slot;
                }
            }
            
            dragAndDropController.RegisterSlots(specialSlots.Values);
            dragAndDropController.RegisterSlots(_backpackCells);
        }

        //TODO: move inventory opening/closing in other class (maybe in UI_Controller??, not sure)
        private void ToggleActive()
        {
            if (!gameObject.activeInHierarchy)
            {
                UI.UI_Controller.SetWindow(ScreenEnum.Inventory);
                Cursor.visible = true;
            }
            else
            {
                UI.UI_Controller.SetWindow(ScreenEnum.GameplayScreen);
                Cursor.visible = false;
            }
        }
        
        private void UpdateBackpack(Item[] items)
        {
            for (int i = 0; i < items.Length; i++)
                _backpackCells[i].SetItem(items[i]);
        }

        private void UpdateSpecialSlots()
        {
            var inventoryData = Inventory.SpecialSlots;
            foreach (var slot in specialSlots)
                if (inventoryData.ContainsKey(slot.Key))
                    slot.Value.SetItem(inventoryData[slot.Key]);
        }
        
        private void SwapItemsInPlayerInventory(UI_Slot from, UI_Slot to)
        {
            if (!(to.IsSpecialSlot || from.IsSpecialSlot))//swap between inventory slots
                Inventory.SwapBagItems(_backpackCells.IndexOf(from), _backpackCells.IndexOf(to));
            else if (from.IsSpecialSlot && to.IsSpecialSlot)//swap between special slots
                Inventory.SwapSpecials(from.SlotType, to.SlotType);
            else if (to.IsSpecialSlot)//swap from inventory to special slot
                Inventory.ApplySpecial(_backpackCells.IndexOf(from), to.SlotType);   
            else//swap from special slot to inventory
                Inventory.RemoveSpecial(_backpackCells.IndexOf(to), from.SlotType);
        }

        private void DropItem(UI_Slot slot)
        {
            CollectableItem collectableItem =
                Instantiate(collectablePrefab, Player.Instance.transform.position + Vector3.up, Quaternion.identity)
                    .GetComponentInChildren<CollectableItem>();

            collectableItem.Item = slot.Item;
            slot.SetItem(null);
            
            if (slot.IsSpecialSlot) 
                Inventory.RemoveSpecialItem(slot.SlotType);
            else 
                Inventory.RemoveItem(_backpackCells.IndexOf(slot));
        }
        
        private void OnEnable()
        {
            Cursor.visible = true;
            _volumeProfileController.ToggleDepthOfField(true);
        }

        private void OnDisable()
        {
            Cursor.visible = false;
            infoPanel.Hide();
            _volumeProfileController.ToggleDepthOfField(false);
        }
    }
}