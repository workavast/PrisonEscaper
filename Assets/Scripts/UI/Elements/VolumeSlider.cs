using System;
using GameCode.audio;
using GameCode.UI.Elements;
using UnityEngine;
using Zenject;

namespace SourceCode.Ui.Elements
{
    [RequireComponent(typeof(ExtendedSlider))]
    public class VolumeSlider : MonoBehaviour
    {
        [SerializeField] private VolumeType volumeType;
        
        [Inject] private readonly AudioVolumeChanger _audioVolumeChanger;
        
        private ExtendedSlider _slider;
        
        private event Action<float> OnValueChange;
        
        private void Awake()
        {
            _slider = GetComponent<ExtendedSlider>();
            _slider.onValueChanged.AddListener(ChangeValue);
            _slider.OnPointerUpEvent += _audioVolumeChanger.Apply;
        }

        private void Start()
        {
            switch (volumeType)
            {
                case VolumeType.Master:
                    _slider.value = _audioVolumeChanger.MasterVolume;
                    OnValueChange += SetMasterVolume;
                    break;
                case VolumeType.Music:
                    _slider.value = _audioVolumeChanger.OstVolume;
                    OnValueChange += SetOstVolume;
                    break;
                case VolumeType.Effects:
                    _slider.value = _audioVolumeChanger.EffectsVolume;
                    OnValueChange += SetEffectsVolume;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ChangeValue(float newValue) => OnValueChange?.Invoke(newValue);
        
        private void SetMasterVolume(float newVolume) => _audioVolumeChanger.SetMasterVolume(newVolume);

        private void SetEffectsVolume(float newVolume) => _audioVolumeChanger.SetEffectsVolume(newVolume);
        
        private void SetOstVolume(float newVolume) => _audioVolumeChanger.SetOstVolume(newVolume);

        private void OnDestroy() => _slider.onValueChanged.RemoveListener(ChangeValue);

        private void Apply() => _audioVolumeChanger.Apply();
        
        private enum VolumeType
        {
            Master = 0,
            Music = 10,
            Effects = 20
        }
    }
}