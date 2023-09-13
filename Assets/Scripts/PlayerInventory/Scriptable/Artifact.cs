using UnityEngine;

namespace PlayerInventory.Scriptable
{
    
    [CreateAssetMenu(menuName = "Items/Specials/Artifact", fileName = "New item")]
    public class Artifact : SpecialItem
    {
        public override string Info()
        {
            return base.Info();
        }
    }
}