using UnityEngine;
using UniversalStatsSystem;

namespace PlayerInventory.Scriptable
{
    public abstract class SpecialItem : Item
    {
        [SerializeField] private Stats mainStats;
        public Stats MainStats => mainStats;
        
        [Space]
        [SerializeField] private AttackStats attackStats;
        public AttackStats AttackStats => attackStats;
        
        [Space]
        [SerializeField] private ResistStats resistStats;
        public ResistStats ResistStats => resistStats;

        public override string Info()
        {
            return base.Info() + MainStats.ExtraInfo() + attackStats.ExtraInfo() + resistStats.ExtraInfo();
        }
    }
}