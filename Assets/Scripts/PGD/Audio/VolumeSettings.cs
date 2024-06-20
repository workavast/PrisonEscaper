using System;
using UnityEngine;

namespace GameCode.PGD.Audio
{
    public sealed class VolumeSettings : ISettings
    {
        public float Master { get; private set; }
        public float OstVolume { get; private set; }
        public float EffectsVolume { get; private set; }

        private float _prevMasterVolume;
        private float _prevOstVolume;
        private float _prevEffectsVolume;
        
        public event Action OnChange;

        public VolumeSettings()
        {
            Master = 1;
            OstVolume = 0.5f;
            EffectsVolume = 0.5f;
        }
    
        public void ChangeMasterVolume(float newVolume)
        {
            Master = newVolume;
        }
        
        public void ChangeOstVolume(float newVolume)
        {
            OstVolume = newVolume;
        }
    
        public void ChangeEffectsVolume(float newVolume)
        {
            EffectsVolume = newVolume;
        }

        public void Apply()
        {
            const float tolerance = 0.001f;
            if (Math.Abs(_prevMasterVolume - Master) > tolerance ||
                Math.Abs(_prevOstVolume - OstVolume) > tolerance ||
                Math.Abs(_prevEffectsVolume - EffectsVolume) > tolerance)
            {
                Debug.Log("Apply");
                _prevMasterVolume = Master;
                _prevOstVolume = OstVolume;
                _prevEffectsVolume = EffectsVolume;

                OnChange?.Invoke();
            }
        }
        
        public void LoadData(VolumeSettingsSave volumeSettings)
        {
            OstVolume = volumeSettings.OstVolume;
            EffectsVolume = volumeSettings.EffectsVolume;
        }
    }
}