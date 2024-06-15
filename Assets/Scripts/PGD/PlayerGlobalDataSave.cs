using System;
using GameCode.PGD.Audio;
using GameCode.PGD.Fps;

namespace GameCode.PGD
{
    [Serializable]
    public class PlayerGlobalDataSave
    {
        public VolumeSettingsSave volumeSettingsSave;
        public FpsSettingsSave fpsSettingsSave;

        public PlayerGlobalDataSave()
        {
            volumeSettingsSave = new();
            fpsSettingsSave = new();
        }
        
        public PlayerGlobalDataSave(PlayerGlobalData playerGlobalData)
        {
            volumeSettingsSave = new VolumeSettingsSave(playerGlobalData.VolumeSettings);
            fpsSettingsSave = new FpsSettingsSave(playerGlobalData.FpsSettings);
        }
        
        public PlayerGlobalDataSave(VolumeSettings volumeSettings, FpsSettings fpsSettings)
        {
            volumeSettingsSave = new VolumeSettingsSave(volumeSettings);
            fpsSettingsSave = new FpsSettingsSave(fpsSettings);
        }
    }
}