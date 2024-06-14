using System;

namespace SourceCode.Core.GlobalData
{
    public interface ISettings
    {
        public event Action OnChange;
    }
}