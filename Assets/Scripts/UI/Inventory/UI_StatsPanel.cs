using Character;
using LevelGeneration.LevelsGenerators;
using TMPro;
using UnityEngine;
using Zenject;

namespace UI
{
    public class UI_StatsPanel : MonoBehaviour, IIniteableUI
    {
        [SerializeField] private TMP_Text health;
        [SerializeField] private TMP_Text mana;
        [SerializeField] private TMP_Text armor;
        [SerializeField] private TMP_Text baseDamage;
        [SerializeField] private TMP_Text critChance;
        [SerializeField] private TMP_Text CritMulty;
        [SerializeField] private TMP_Text attackSpeed;

        [Inject] private readonly LevelGeneratorBase _levelGenerator;
        
        public void Init()
        {
            _levelGenerator.OnPlayerSpawned += Init;
        }
        
        private void Init(Player player)
        {
            player.StatsSystem.OnStatsChanged.AddListener(UpdateStatsPanel);
            UpdateStatsPanel();
        }

        private void UpdateStatsPanel()
        {
            var StatsSystem = Player.Instance.StatsSystem;
            health.text = $"Здоровье: {StatsSystem.Health.CurrentValue}/{StatsSystem.Health.MaxValue}";
            mana.text = $"Мана: {StatsSystem.Mana.CurrentValue}/{StatsSystem.Mana.MaxValue}";
            armor.text = $"Сопротивления:\n{StatsSystem.ResistStats.ExtraInfo()}";
            baseDamage.text = $"Урон:\n{StatsSystem.AttackStats.ExtraInfo()}";
            critChance.text = $"Шанс крита: {StatsSystem.AttackStats.criticalChance * 100}%";
            CritMulty.text = $"Множитель крита: {StatsSystem.AttackStats.criticalMultiply * 100}%";
            attackSpeed.text = $"Скорость атаки: {StatsSystem.AttackStats.attackCooldown * 100}%";
        }
    }
}