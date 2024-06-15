using GameCode.PGD.Audio;
using UnityEngine;
using UnityEngine.Audio;

namespace GameCode.audio
{
    public class AudioVolumeChanger
    {
        private const string MasterParam = "MasterVolume";
        private const string EffectsParam = "EffectsVolume";
        private const string OstParam = "MusicVolume";
        private readonly AudioMixer _mixer;
        private readonly VolumeSettings _volumeSettings;

        public AudioVolumeChanger(AudioMixer mixer, VolumeSettings volumeSettings)
        {
            _mixer = mixer;
            _volumeSettings = volumeSettings;
        }

        public float MasterVolume => _volumeSettings.Master;
        public float OstVolume => _volumeSettings.OstVolume;
        public float EffectsVolume => _volumeSettings.EffectsVolume;

        public void StartInit()
        {         
            SetVolume(MasterParam, MasterVolume);
            SetVolume(EffectsParam, EffectsVolume);
            SetVolume(OstParam, OstVolume);
        }

        public void SetMasterVolume(float newVolume)
        {
            _volumeSettings.ChangeMasterVolume(newVolume);
            SetVolume(MasterParam, MasterVolume);
        }
        
        public void SetEffectsVolume(float newVolume)
        {
            _volumeSettings.ChangeEffectsVolume(newVolume);
            SetVolume(EffectsParam, EffectsVolume);
        }

        public void SetOstVolume(float newVolume)
        {
            _volumeSettings.ChangeOstVolume(newVolume);
            SetVolume(OstParam, OstVolume);
        }

        private void SetVolume(string paramName, float newVolume)
            => _mixer.SetFloat($"{paramName}", Mathf.Lerp(-80, 0, Mathf.Sqrt(Mathf.Sqrt(newVolume))));
    }
}