using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniversalStatsSystem
{
    [System.Serializable]
    public struct ResistStats
    {
        [SerializeField] [Range(0, 1)] private float resistReduceCup; // 0.8 = 80% 
        [SerializeField] private float fullResistAmount; // 1000
        
        [Space] 
        public float physicalResistance;
        public float fireResistance;
        public float waterResistance;
        public float airResistance;
        public float earthResistance;
        public float electricityResistance;
        public float poisonResistance;
        
        public string ExtraInfo()
        {
            string result = "";
            
            if (physicalResistance != 0)
                result += $"Physical resist: {physicalResistance.ToString("+0.#;-0.#")}\n"; 
            if (fireResistance != 0)
                result += $"Fire resist: {fireResistance.ToString("+0.#;-0.#")}\n";
            if (waterResistance != 0)
                result += $"Water resist: {waterResistance.ToString("+0.#;-0.#")}\n";
            if (airResistance != 0)
                result += $"Air resist: {airResistance.ToString("+0.#;-0.#")}\n"; 
            if (earthResistance != 0)
                result += $"Earth resist: {earthResistance.ToString("+0.#;-0.#")}\n";
            if (electricityResistance != 0)
                result += $"Electricity resist: {electricityResistance.ToString("+0.#;-0.#")}\n";
            if (poisonResistance != 0)
                result += $"Poison resist: {poisonResistance.ToString("+0.#;-0.#")}\n";
            
            return result;
        }
        
        public ResistStats(float resistance)
        {
            resistReduceCup = 0.8f;
            fullResistAmount = 1000f;

            physicalResistance = resistance;
            fireResistance = resistance;
            waterResistance = resistance;
            airResistance = resistance;
            earthResistance = resistance;
            electricityResistance = resistance;
            poisonResistance = resistance;
        }

        public static ResistStats operator +(ResistStats resistStatsA, ResistStats resistStatsB)
        {
            resistStatsA.physicalResistance += resistStatsB.physicalResistance;
            resistStatsA.fireResistance += resistStatsB.fireResistance;
            resistStatsA.waterResistance += resistStatsB.waterResistance;
            resistStatsA.airResistance += resistStatsB.airResistance;
            resistStatsA.earthResistance += resistStatsB.earthResistance;
            resistStatsA.electricityResistance += resistStatsB.electricityResistance;
            resistStatsA.poisonResistance += resistStatsB.poisonResistance;

            return resistStatsA;
        }

        public static ResistStats operator -(ResistStats resistStatsA, ResistStats resistStatsB)
        {
            resistStatsA.physicalResistance -= resistStatsB.physicalResistance;
            resistStatsA.fireResistance -= resistStatsB.fireResistance;
            resistStatsA.waterResistance -= resistStatsB.waterResistance;
            resistStatsA.airResistance -= resistStatsB.airResistance;
            resistStatsA.earthResistance -= resistStatsB.earthResistance;
            resistStatsA.electricityResistance -= resistStatsB.electricityResistance;
            resistStatsA.poisonResistance -= resistStatsB.poisonResistance;

            return resistStatsA;
        }

        public static ResistStats operator *(ResistStats resistStats, float scale)
        {
            resistStats.physicalResistance *= scale;
            resistStats.fireResistance *= scale;
            resistStats.waterResistance *= scale;
            resistStats.airResistance *= scale;
            resistStats.earthResistance *= scale;
            resistStats.electricityResistance *= scale;
            resistStats.poisonResistance *= scale;

            return resistStats;
        }

        public static float operator *(AttackStats attackStats, ResistStats resistStats)
        {
            float multiplier = 1;
            
            multiplier = 1 - resistStats.physicalResistance / resistStats.fullResistAmount;
            multiplier = ((1 - multiplier) > resistStats.resistReduceCup) ? (1 - resistStats.resistReduceCup) : multiplier;
            float physicalDamage = Mathf.Clamp(attackStats.physicalDamage, 0f, Mathf.Infinity) * multiplier;

            multiplier = 1 - resistStats.fireResistance / resistStats.fullResistAmount;
            multiplier = ((1 - multiplier) > resistStats.resistReduceCup) ? (1 - resistStats.resistReduceCup) : multiplier;
            float fireDamage = Mathf.Clamp(attackStats.fireDamage, 0f, Mathf.Infinity) * multiplier;

            multiplier = 1 - resistStats.waterResistance / resistStats.fullResistAmount;
            multiplier = ((1 - multiplier) > resistStats.resistReduceCup) ? (1 - resistStats.resistReduceCup) : multiplier;
            float waterDamage = Mathf.Clamp(attackStats.waterDamage, 0f, Mathf.Infinity) * multiplier;

            multiplier = 1 - resistStats.airResistance / resistStats.fullResistAmount;
            multiplier = ((1 - multiplier) > resistStats.resistReduceCup) ? (1 - resistStats.resistReduceCup) : multiplier;
            float airDamage = Mathf.Clamp(attackStats.airDamage, 0f, Mathf.Infinity) * multiplier;

            multiplier = 1 - resistStats.earthResistance / resistStats.fullResistAmount;
            multiplier = ((1 - multiplier) > resistStats.resistReduceCup) ? (1 - resistStats.resistReduceCup) : multiplier;
            float earthDamage = Mathf.Clamp(attackStats.earthDamage, 0f, Mathf.Infinity) * multiplier;

            multiplier = 1 - resistStats.electricityResistance / resistStats.fullResistAmount;
            multiplier = ((1 - multiplier) > resistStats.resistReduceCup) ? (1 - resistStats.resistReduceCup) : multiplier;
            float electricityDamage = Mathf.Clamp(attackStats.electricityDamage, 0f, Mathf.Infinity) * multiplier;

            multiplier = 1 - resistStats.poisonResistance / resistStats.fullResistAmount;
            multiplier = ((1 - multiplier) > resistStats.resistReduceCup) ? (1 - resistStats.resistReduceCup) : multiplier;
            float poisonDamage = Mathf.Clamp(attackStats.poisonDamage, 0f, Mathf.Infinity) * multiplier;

            return physicalDamage + fireDamage + waterDamage + airDamage + earthDamage + electricityDamage +
                   poisonDamage;
        }
    }
}