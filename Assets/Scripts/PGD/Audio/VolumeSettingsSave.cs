using System;

namespace GameCode.PGD.Audio
{
    [Serializable]
    public sealed class VolumeSettingsSave
    {
        public float OstVolume = 0.5f;
        public float EffectsVolume = 0.5f;

        public VolumeSettingsSave()
        {
            OstVolume = 0.5f;
            EffectsVolume = 0.5f;
        }
        
        public VolumeSettingsSave(VolumeSettings settings)
        {
            OstVolume = settings.OstVolume;
            EffectsVolume = settings.EffectsVolume;
        }
    }
}