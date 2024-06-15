using System;

namespace GameCode.PGD.Fps
{
    [Serializable]
    public class FpsSettingsSave
    {
        public int FpsCap = 60;

        public FpsSettingsSave()
        {
            FpsCap = 60;
        }
        
        public FpsSettingsSave(FpsSettings settings)
        {
            FpsCap = settings.FpsCap;
        }
    }
}