using System;
using UnityEngine;

namespace PlayerInventory.Scriptable
{
    [CreateAssetMenu(menuName = "Items/Specials/Weapon", fileName = "New item")]
    public class Weapon : SpecialItem
    {
        public override string Info()
        {
            return base.Info();
        }
    }
}