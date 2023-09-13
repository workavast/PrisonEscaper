using System;

namespace Stats_system
{
    [Serializable]
    public struct WeaponStats
    {
        public float Damage;
        public float Cooldown;
        public float CriticalChance;
        public float CriticalMultiplier;

        public WeaponStats(float damage, float cooldown, float criticalChance, float criticalMultiplier)
        {
            Damage = damage;
            Cooldown = cooldown;
            CriticalChance = criticalChance;
            CriticalMultiplier = criticalMultiplier;
        }
    }
}