namespace SourceCode.SavingAndLoading.FileSavingAndLoading
{
    public interface IFileSaveAndLoader
    {
        public void Save(object data);

        public T Load<T>() 
            where T : new();

        public bool SaveExist();
    }
}