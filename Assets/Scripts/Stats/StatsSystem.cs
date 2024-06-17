using System;
using SomeStorages;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace UniversalStatsSystem
{
    [Serializable]
    public class StatsSystem
    {
        [SerializeField] private Stats mainStats;
        [Space]
        [SerializeField] private AttackStats attackStats;
        [Space]
        [SerializeField] private ResistStats resistStats;
        
        [Space]
        public bool isInvincible;

        [HideInInspector] public UnityEvent OnDeath;
        [HideInInspector] public UnityEvent OnStatsChanged;
        
        public Stats MainStats => mainStats;
        public AttackStats AttackStats => attackStats;
        public ResistStats ResistStats => resistStats;
        
        public IReadOnlySomeStorage<float> Health => MainStats.Health;
        public IReadOnlySomeStorage<float> Mana => MainStats.Mana;
        
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
            OnStatsChanged = new UnityEvent();
        }
        
        public void Heal(float value)
        {
            MainStats.ChangeHealth(value);
        }

        public bool ChangeMana(float value)
        {
            if (value < 0 && MainStats.Mana.CurrentValue < -value) 
                return false;
            
            MainStats.ChangeMana(value);
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
            MainStats.ChangeHealth(-damageMagnitude);
            if (MainStats.Health.IsEmpty)
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
        }
        
        public void RemoveStats(Stats mainStats, AttackStats attackStats, ResistStats resistStats)
        {
            this.mainStats -= mainStats;
            this.attackStats -= attackStats;
            this.resistStats -= resistStats;
        }
    }

    [Serializable]
    public class Stats
    {
        [SerializeField] private FloatStorage health;
        [SerializeField] private FloatStorage mana;

        public IReadOnlySomeStorage<float> Health => health;
        public IReadOnlySomeStorage<float> Mana => mana;
        
        [Space]
        public float WalkSpeed;
        
        public Stats(float health, float mana,float walkSpeed)
        {
            this.health = new FloatStorage(health);
            this.mana = new FloatStorage(mana);
            
            WalkSpeed = walkSpeed;
        }

        public static Stats operator +(Stats a, Stats b)
        {
            a.health.SetMaxValue(a.health.MaxValue + b.health.MaxValue);
            a.mana.SetMaxValue(a.mana.MaxValue + b.mana.MaxValue);
            a.WalkSpeed += b.WalkSpeed;
            return a;
        }

        public static Stats operator -(Stats a, Stats b)
        {
            a.health.SetMaxValue(a.health.MaxValue - b.health.MaxValue);
            a.mana.SetMaxValue(a.mana.MaxValue - b.mana.MaxValue);
            a.WalkSpeed -= b.WalkSpeed;
            return a;
        }

        public void ChangeHealth(float value)
        {
            health.ChangeCurrentValue(value);
        }

        public void ChangeMana(float value)
        {
            mana.ChangeCurrentValue(value);
        }
        
        public virtual string ExtraInfo()
        {
            string result = "";
            
            if (health.MaxValue != 0)
                result += $"Max health: {health.MaxValue.ToString("+0.#;-0.#")}\n"; 
            if (mana.MaxValue != 0)
                result += $"Max mana: {mana.MaxValue.ToString("+0.#;-0.#")}\n";
            if (WalkSpeed != 0)
                result += $"Walk speed: {WalkSpeed.ToString("+0.#;-0.#")}\n";
            
            return result;
        }
    }
}