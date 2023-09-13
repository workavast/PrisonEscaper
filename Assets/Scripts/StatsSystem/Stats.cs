using System;
using UnityEngine;

namespace Stats_System
{
    [Serializable]
    public class Stats
    {
        [HideInInspector] public float Health;
        public float MaxHealth;
        public float BaseDamage;
        public float Armor;
        public float CriticalChance;
        public float CriticalMultiply;
        public float AttackSpeed;
        public float WalkSpeed;
        public float ArmorReduceCup;
        public float FullResistArmour;

        public Stats()
        {
            Health = MaxHealth;
        }

        // public Stats(float health, float baseDamage, float walkSpeed, float armor = 0, float criticalChance = 0,
        //     float criticalMultiply = 0, float attackSpeed = 1f)
        // {
        //     MaxHealth = Health = health;
        //     BaseDamage = baseDamage;
        //     Armor = armor;
        //     CriticalChance = criticalChance;
        //     CriticalMultiply = criticalMultiply;
        //     AttackSpeed = attackSpeed;
        //     WalkSpeed = walkSpeed;
        // }
    }
}