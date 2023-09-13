using Stats_System;
using UnityEngine;

namespace Ability_system
{
    [CreateAssetMenu(menuName = "Items/Abilities/TestShieldAbility", fileName = "TestShieldAbility")]
    public class TestShieldActiveSkill : BaseAbility, IActiveAbility
    {
        // с шансом 50 процентов не получить урон
        
        [SerializeField] private float cooldown;
        
        public void Use()
        {
            
        }
    }
}