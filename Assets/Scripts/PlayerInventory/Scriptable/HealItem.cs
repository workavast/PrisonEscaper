using Character;
using UnityEngine;

namespace PlayerInventory.Scriptable
{
    [CreateAssetMenu(menuName = "Items/Specials/Heal", fileName = "New item")]
    public class HealItem_ : SpecialItem, IUsable
    {
        [SerializeField] private float _healAmount;

        public override string Info()
        {
            return base.Info() + $"Heal: {_healAmount}HP\n";
        }

        public void UseEffect()
        {
            Player.Instance.Heal(_healAmount);
        }
    }
}