using System;
using GameCode.Scenes.PGD;
using SourceCode.Core.GlobalData.Volume;
using SourceCode.SavingAndLoading;
using UnityEngine;

namespace SourceCode.Core.GlobalData
{
    public class PlayerGlobalData
    {
        private static PlayerGlobalData _instance;
        public static PlayerGlobalData Instance => _instance ??= new PlayerGlobalData();

        public readonly VolumeSettings VolumeSettings = new();
        public readonly FpsSettings FpsSettings = new();
        
        private readonly ISaveAndLoader _saveAndLoader;
        
        public event Action OnInit;

        private PlayerGlobalData()
        {
            Debug.Log("-||- PlayerGlobalData initializing");
            
            _saveAndLoader = new DesktopSaveAndLoader();
            LoadData();
            SubsAfterFirstLoad();
        }
        
        public void ResetSaves() 
            => _saveAndLoader.ResetSave();
        
        private void LoadData()
        {
            var save = _saveAndLoader.Load(out var isFirstSession);
            
            VolumeSettings.LoadData(save.volumeSettingsSave);
            FpsSettings.LoadData(save.fpsSettingsSave);
        }
        
        private void SaveData() 
            => _saveAndLoader.Save(this);

        private void SubsAfterFirstLoad()
        {
            Debug.Log("-||- SubsAfterFirstLoad");
            
            ISettings[] settings =
            {
                VolumeSettings, 
                FpsSettings
            };
            foreach (var setting in settings)
                setting.OnChange += SaveData;
            
            OnInit?.Invoke();
        }
    }
}
