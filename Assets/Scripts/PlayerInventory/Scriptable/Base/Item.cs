using UnityEngine;

namespace PlayerInventory.Scriptable
{
    [CreateAssetMenu(menuName = "Items/Item", fileName = "Item")]
    public class Item : ScriptableObject
    {
        [field: SerializeField] public string ItemName { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public Sprite Sprite { get; private set; }
        [field: SerializeField] public ItemType Type { get; private set; }
        [field: SerializeField] public ItemRarity Rarity { get; private set; }
        
        public virtual string Info()
        {
            return "";
        }
    }
}