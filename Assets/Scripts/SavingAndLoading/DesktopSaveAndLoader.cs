using SourceCode.Core.GlobalData;
using SourceCode.SavingAndLoading.FileSavingAndLoading;

namespace SourceCode.SavingAndLoading
{
    public class DesktopSaveAndLoader : ISaveAndLoader
    {
        private readonly IFileSaveAndLoader _fileSaveAndLoader = new JsonSaveAndLoader();
        
        public PlayerGlobalDataSave Load(out bool isFirstSession)
        {
            isFirstSession = !_fileSaveAndLoader.SaveExist();
            return _fileSaveAndLoader.Load<PlayerGlobalDataSave>();
        }

        public void Save(PlayerGlobalData playerGlobalData) 
            => _fileSaveAndLoader.Save(new PlayerGlobalDataSave(playerGlobalData));

        public void ResetSave() 
            => _fileSaveAndLoader.Save(new PlayerGlobalDataSave());
    }
}