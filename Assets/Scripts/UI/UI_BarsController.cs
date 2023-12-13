﻿using Character;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UI_BarsController : MonoBehaviour
    {
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Slider manaSlider;
        
        private UI_SomeBar _healthBar;
        private UI_SomeBar _manaBar;
        
        private void Start()
        {
            Player.Instance.StatsSystem.Health.OnChange += SetHealthSliderValue;
            Player.Instance.StatsSystem.Mana.OnChange += SetManaSliderValue;

            _healthBar = new UI_SomeBar(this, healthSlider, Player.Instance.StatsSystem.Health.FillingPercentage);
            _manaBar = new UI_SomeBar(this, manaSlider, Player.Instance.StatsSystem.Mana.FillingPercentage);
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