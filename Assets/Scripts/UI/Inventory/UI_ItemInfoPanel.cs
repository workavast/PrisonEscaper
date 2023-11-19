using PlayerInventory.Scriptable;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UI_ItemInfoPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text description;
        [SerializeField] private TMP_Text itemInfo;

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void Show(UI_Slot slot)
        {
            title.text = slot.Item.ItemName;
            description.text = slot.Item.Description;
            itemInfo.text = slot.Item.Info();
            gameObject.SetActive(true);
        }
        
        public void Show(Item item)
        {
            title.text = item.ItemName;
            description.text = item.Description;
            itemInfo.text = item.Info();
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}