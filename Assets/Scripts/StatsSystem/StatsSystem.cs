using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Stats_System
{
    [Serializable]
    public struct StatsSystem
    {
        [SerializeField] private Stats _baseStats;
        public Stats Stats => _baseStats;
        
        public float ArmorReduceMultiplier
        {
            get
            {
                float multiplier = 1 - _baseStats.Armor / _baseStats.FullResistArmour;
                return 1 - multiplier > _baseStats.ArmorReduceCup ? 1 - _baseStats.ArmorReduceCup : multiplier;
            }
        }

        // public float GetDamageWithWeapon(Weapon weapon)
        // {
        //     float damage = _baseStats.BaseDamage + weapon.Stats.Damage;
        //     if (Random.value < (_baseStats.CriticalChance + weapon.Stats.CriticalChance))
        //         damage *= (_baseStats.CriticalMultiply + weapon.Stats.CriticalMultiplier);
        //
        //     return damage;
        // }

        public void TakeDamage(float damage)
        {
            //TODO: implement block ability;
            damage *= ArmorReduceMultiplier;
            _baseStats.Health -= damage;
        }

        public float GetDamage(float damage)
        {
            return Stats.BaseDamage;
        }

        private static float ToZero(float value)
        {
            return value < 0 ? 0 : value;
        }
    }
}