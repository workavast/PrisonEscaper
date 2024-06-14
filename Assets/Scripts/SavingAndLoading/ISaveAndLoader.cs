using SourceCode.Core.GlobalData;

namespace SourceCode.SavingAndLoading
{
    public interface ISaveAndLoader
    {
        public PlayerGlobalDataSave Load(out bool isFirstSession);

        public void Save(PlayerGlobalData playerGlobalData);

        public void ResetSave();
    }
}