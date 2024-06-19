using System;

namespace GameCode.PGD.Fps
{
    [Serializable]
    public class TutorialSettingsSave
    {
        public bool TutorialCompleted = false;

        public TutorialSettingsSave()
        {
            TutorialCompleted = false;
        }
        
        public TutorialSettingsSave(TutorialSettings settings)
        {
            TutorialCompleted = settings.TutorialCompleted;
        }
    }
}