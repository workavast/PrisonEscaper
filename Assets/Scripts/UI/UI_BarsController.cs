using Character;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UI_BarsController : MonoBehaviour
    {
        [SerializeField] private Slider _HealthSlider;
        [SerializeField] private Slider _ManaSlider;

        private void Start()
        {
            Player.Instance.StatsSystem.OnHealthStatsChange += SetHealthSliderValue;
            Player.Instance.StatsSystem.OnManaStatsChange += SetManaSliderValue;

            SetHealthSliderValue();
            SetManaSliderValue();
        }
        
        private void SetHealthSliderValue()
        {
            _HealthSlider.value = Player.Instance.StatsSystem.MainStats.Health / Player.Instance.StatsSystem.MainStats.MaxHealth;
        }
        
        private void SetManaSliderValue()
        {
            _ManaSlider.value = Player.Instance.StatsSystem.MainStats.Mana / Player.Instance.StatsSystem.MainStats.MaxMana;
        }
    }
}