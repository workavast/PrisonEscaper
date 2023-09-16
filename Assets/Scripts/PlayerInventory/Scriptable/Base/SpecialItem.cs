using UnityEngine;
using UniversalStatsSystem;

namespace PlayerInventory.Scriptable
{
    public abstract class SpecialItem : Item
    {
        [field: Space]
        [field: SerializeField] public Stats MainStats { get; private set; }
        
        [field: Space]
        [field: SerializeField] public AttackStats AttackStats { get; private set; }
        
        [field: Space]
        [field: SerializeField] public ResistStats ResistStats { get; private set; }

        public override string Info()
        {
            return base.Info() + MainStats.ExtraInfo() + AttackStats.ExtraInfo() + ResistStats.ExtraInfo();
        }
    }
}