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
        public TutorialSettingsSave tutorialSettingsSave;

        public PlayerGlobalDataSave()
        {
            volumeSettingsSave = new();
            fpsSettingsSave = new();
            tutorialSettingsSave = new();
        }
        
        public PlayerGlobalDataSave(PlayerGlobalData playerGlobalData)
        {
            volumeSettingsSave = new VolumeSettingsSave(playerGlobalData.VolumeSettings);
            fpsSettingsSave = new FpsSettingsSave(playerGlobalData.FpsSettings);
            tutorialSettingsSave = new TutorialSettingsSave(playerGlobalData.TutorialSettings);
        }
        
        public PlayerGlobalDataSave(VolumeSettings volumeSettings, FpsSettings fpsSettings, TutorialSettings tutorialSettings)
        {
            volumeSettingsSave = new VolumeSettingsSave(volumeSettings);
            fpsSettingsSave = new FpsSettingsSave(fpsSettings);
            tutorialSettingsSave = new TutorialSettingsSave(tutorialSettings);
        }
    }
}