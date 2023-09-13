using System;
using Character;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UIStatsPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text health;
        [SerializeField] private TMP_Text mana;
        [SerializeField] private TMP_Text armor;
        [SerializeField] private TMP_Text baseDamage;
        [SerializeField] private TMP_Text critChance;
        [SerializeField] private TMP_Text CritMulty;
        [SerializeField] private TMP_Text attackSpeed;


        private void Start()
        {
            Player.Instance.StatsSystem.OnStatsChanged.AddListener(UpdateStatsPanel);
            UpdateStatsPanel();
        }

        private void UpdateStatsPanel()
        {
            var StatsSystem = Player.Instance.StatsSystem;
            health.text = $"Здоровье: {StatsSystem.MainStats.Health}/{StatsSystem.MainStats.MaxHealth}";
            mana.text = $"Мана: {StatsSystem.MainStats.Mana}/{StatsSystem.MainStats.MaxMana}";
            armor.text = $"Магнитуда сопротивлений: {StatsSystem.ResistStats.ExtraInfo()}";
            baseDamage.text = $"Физический урон: {StatsSystem.AttackStats.ExtraInfo()}";
            critChance.text = $"Шанс крита: {StatsSystem.AttackStats.criticalChance * 100}%";
            CritMulty.text = $"Множитель крита: {StatsSystem.AttackStats.criticalMultiply * 100}%";
            attackSpeed.text = $"Скорость атаки: {StatsSystem.AttackStats.attackCooldown * 100}%";
        }
    }
}