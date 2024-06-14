namespace GameCode.Scenes.PGD
{
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