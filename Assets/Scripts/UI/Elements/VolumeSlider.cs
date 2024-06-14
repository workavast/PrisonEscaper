using System;
using SourceCode.Audio;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SourceCode.Ui.Elements
{
    [RequireComponent(typeof(Slider))]
    public class VolumeSlider : MonoBehaviour
    {
        [SerializeField] private VolumeType volumeType;

        [Inject] private readonly AudioVolumeChanger _audioManager;
        
        private Slider _slider;
        
        private event Action<float> OnValueChange;
        
        private void Awake()
        {
            _slider = GetComponent<Slider>();
            _slider.onValueChanged.AddListener(ChangeValue);
        }

        private void Start()
        {
            switch (volumeType)
            {
                case VolumeType.Master:
                    _slider.value = _audioManager.MasterVolume;
                    OnValueChange += SetMasterVolume;
                    break;
                case VolumeType.Music:
                    _slider.value = _audioManager.OstVolume;
                    OnValueChange += SetOstVolume;
                    break;
                case VolumeType.Effects:
                    _slider.value = _audioManager.EffectsVolume;
                    OnValueChange += SetEffectsVolume;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ChangeValue(float newValue) => OnValueChange?.Invoke(newValue);
        
        private void SetMasterVolume(float newVolume) => _audioManager.SetMasterVolume(newVolume);

        private void SetEffectsVolume(float newVolume) => _audioManager.SetEffectsVolume(newVolume);
        
        private void SetOstVolume(float newVolume) => _audioManager.SetOstVolume(newVolume);

        private void OnDestroy() => _slider.onValueChanged.RemoveListener(ChangeValue);

        private enum VolumeType
        {
            Master = 0,
            Music = 10,
            Effects = 20
        }
    }
}