using System;
using Character;
using PlayerInventory.Scriptable;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace UniversalStatsSystem
{
    [Serializable]
    public class StatsSystem
    {
        [SerializeField] private Stats mainStats;
        public Stats MainStats => mainStats;
        
        [Space]
        [SerializeField] private AttackStats attackStats;
        public AttackStats AttackStats => attackStats;
        
        [Space]
        [SerializeField] private ResistStats resistStats;
        public ResistStats ResistStats => resistStats;

        [Space]
        public bool isInvincible;

        [HideInInspector] public UnityEvent OnDeath;
        [HideInInspector] public UnityEvent OnStatsChanged;
        
        public event Action OnHealthStatsChange;
        public event Action OnManaStatsChange;

        [HideInInspector]
        public enum DamageType
        {
            None,
            Physical,
            Fire,
            Water,
            Air,
            Earth,
            Electricity,
            Poison
        }

        public void Init()
        {
            MainStats.Health = MainStats.MaxHealth;
            MainStats.Mana = MainStats.MaxMana;
            OnStatsChanged = new UnityEvent();
        }
        
        public void Heal(float value)
        {
            MainStats.Health = Mathf.Min(MainStats.Health + value, MainStats.MaxHealth);
            OnHealthStatsChange?.Invoke();
        }

        public bool SetMana(float value)
        {
            if ((MainStats.Mana + value) < 0)
            {
                return false;
            }
            MainStats.Mana = Math.Min(MainStats.Mana + value, MainStats.MaxMana);
            OnManaStatsChange?.Invoke();
            return true;
        }

        private float CalculateTypedDamage(float damageValue, DamageType damageType)
        {
            float finalDamage = damageValue, damageReduction = 0;

            switch (damageType)
            {
                case DamageType.Physical:
                    damageReduction = ResistStats.physicalResistance;
                    break;
                case DamageType.Fire:
                    damageReduction = ResistStats.fireResistance;
                    break;
                case DamageType.Water:
                    damageReduction = ResistStats.waterResistance;
                    break;
                case DamageType.Air:
                    damageReduction = ResistStats.airResistance;
                    break;
                case DamageType.Earth:
                    damageReduction = ResistStats.earthResistance;
                    break;
                case DamageType.Electricity:
                    damageReduction = ResistStats.electricityResistance;
                    break;
                case DamageType.Poison:
                    damageReduction = ResistStats.poisonResistance;
                    break;
            }

            float effectiveResistance = Mathf.Clamp(damageReduction, 0, resistStats.fullResistAmount);
            float resistMultiplier = 1 - Mathf.Min(effectiveResistance / resistStats.fullResistAmount, resistStats.resistReduceCup);

            finalDamage *= resistMultiplier;
            return finalDamage;
        }

        private void TakeDamageCont(float damageMagnitude)
        {
            if (isInvincible)
                return;
            MainStats.Health -= damageMagnitude;
            OnHealthStatsChange?.Invoke();
            if (MainStats.Health < 0)
                OnDeath.Invoke();

            Debug.Log($"[Stats system]: total taken damage: {damageMagnitude}");
        }

        public void TakeDamage(float damageValue, DamageType damageType)
        {
            TakeDamageCont(CalculateTypedDamage(damageValue, damageType));
        }

        public void TakeDamage(float damageValue)
        {
            TakeDamageCont(damageValue);
        }

        public void TakeDamage(AttackStats attackStats)
        {
            TakeDamageCont(attackStats * ResistStats);
        }

        public AttackStats GetDamage()
        {
            AttackStats criticalAttackStats =
                AttackStats * (Random.value <= AttackStats.criticalChance ? AttackStats.criticalMultiply : 1);
            return criticalAttackStats;
        }

        public void ApplyStats(Stats mainStats, AttackStats attackStats, ResistStats resistStats)
        {
            this.mainStats += mainStats;
            this.attackStats += attackStats;
            this.resistStats += resistStats;
            OnHealthStatsChange?.Invoke();
            OnManaStatsChange?.Invoke();
        }
        
        public void RemoveStats(Stats mainStats, AttackStats attackStats, ResistStats resistStats)
        {
            this.mainStats -= mainStats;
            this.attackStats -= attackStats;
            this.resistStats -= resistStats;
            OnHealthStatsChange?.Invoke();
            OnManaStatsChange?.Invoke();
        }
    }

    [Serializable]
    public class Stats
    {
        [HideInInspector] public float Health;
        public float MaxHealth;
        [HideInInspector] public float Mana;
        public float MaxMana;
        
        [Space]
        public float WalkSpeed;
        
        public Stats(float health, float mana,float walkSpeed)
        {
            MaxHealth = Health = health;
            MaxMana = Mana = mana;
            WalkSpeed = walkSpeed;
        }

        public static Stats operator +(Stats a, Stats b)
        {
            a.MaxHealth += b.MaxHealth;
            if (a.Health > a.MaxHealth)
                a.Health = a.MaxHealth;
            
            a.MaxMana += b.MaxMana;
            if (a.Mana > a.MaxMana)
                a.Mana = a.MaxMana;
            
            a.WalkSpeed += b.WalkSpeed;
            return a;
        }

        public static Stats operator -(Stats a, Stats b)
        {
            a.MaxHealth -= b.MaxHealth;
            if (a.Health > a.MaxHealth)
                a.Health = a.MaxHealth;
            
            a.MaxMana -= b.MaxMana;
            if (a.Mana > a.MaxMana)
                a.Mana = a.MaxMana;
            
            a.WalkSpeed -= b.WalkSpeed;
            return a;
        }

        public virtual string ExtraInfo()
        {
            string result = "";
            
            if (MaxHealth != 0)
                result += $"Max health: {MaxHealth.ToString("+0.#;-0.#")}\n"; 
            if (MaxMana != 0)
                result += $"Max mana: {MaxMana.ToString("+0.#;-0.#")}\n";
            if (WalkSpeed != 0)
                result += $"Walk speed: {WalkSpeed.ToString("+0.#;-0.#")}\n";
            
            return result;
        }
    }
}