using System;
using GameCode.Scenes.PGD;
using SourceCode.Core.GlobalData.Volume;

namespace SourceCode.Core.GlobalData
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