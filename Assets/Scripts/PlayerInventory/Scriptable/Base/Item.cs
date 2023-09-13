using UnityEngine;
using UnityEngine.PlayerLoop;

namespace PlayerInventory.Scriptable
{
    [CreateAssetMenu(menuName = "Items/Item", fileName = "Item")]
    public class Item : ScriptableObject
    {
        public string name;
        public string description;
        public Sprite sprite;
        public ItemType type;
        public ItemRarity Rarity = ItemRarity.Common;
        public virtual string Info()
        {
            return "";
        }
    }
}