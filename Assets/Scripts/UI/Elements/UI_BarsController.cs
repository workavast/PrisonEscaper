using Character;
using LevelGeneration.LevelsGenerators;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class UI_BarsController : MonoBehaviour
    {
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Slider manaSlider;
        
        [Inject] private LevelGeneratorBase _levelGenerator;
        
        private UI_SomeBar _healthBar;
        private UI_SomeBar _manaBar;

        private void Awake()
        {
            _levelGenerator.OnPlayerSpawned += Init;
        }

        private void Init(Player player)
        {
            player.StatsSystem.Health.OnChange += SetHealthSliderValue;
            player.StatsSystem.Mana.OnChange += SetManaSliderValue;

            _healthBar = new UI_SomeBar(this, healthSlider, player.StatsSystem.Health.FillingPercentage);
            _manaBar = new UI_SomeBar(this, manaSlider, player.StatsSystem.Mana.FillingPercentage);
        }

        private void SetHealthSliderValue() => SetBarValue(_healthBar, Player.Instance.StatsSystem.Health.FillingPercentage);
        private void SetManaSliderValue() => SetBarValue(_manaBar, Player.Instance.StatsSystem.Mana.FillingPercentage);

        private void SetBarValue(UI_SomeBar bar, float value)
        {
            if(gameObject.activeInHierarchy)
                bar.SetTargetValue(value);
            else
                bar.SetValue(value);
        }
    }
}