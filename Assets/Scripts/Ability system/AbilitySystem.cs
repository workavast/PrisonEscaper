using UnityEngine;
using System;
using System.Collections.Generic;
using PlayerInventory.Scriptable;
using Stats_System;


namespace Ability_system 
{
    [Serializable]
    public class AbilitySystem 
    {
        [SerializeField] private StatsSystem _statsSystem;
        private List<BaseAbility> _currentAbilities;
        public Stats Stats => _statsSystem.Stats;


        public AbilitySystem()
        {
            _statsSystem = new StatsSystem();
        }
        
        
        
        public float CalculateDamage(float baseDamage)
        {
            //TODO: operate with baseDamage
            return baseDamage;
        }

        public float GetDamage()
        {
            //TODO: calculate damage
            return _statsSystem.Stats.BaseDamage;
        }

        public float TakeDamage(float damage)
        {
            //TODO: calculate damage using all abilities;
            return _statsSystem.GetDamage(damage);
        }

        public void SetWeapon(Item weapon)
        {
            if (weapon is not Weapon)
                return;
            
        }

        private void ApplyAbilities()
        {
            foreach (BaseAbility ability in _currentAbilities)
            {
                if (ability is IActiveAbility)
                {
                    
                }
            }
        }
    }
}