using GameCode.Scenes.PGD;
using UnityEngine;

namespace GameCode.Scenes.Audio
{
    public class FpsCapChanger
    {
        private readonly FpsSettings _fpsSettings;
        
        public FpsCapChanger(FpsSettings fpsSettings)
        {
            _fpsSettings = fpsSettings;
            ApplyFpsCap(_fpsSettings.FpsCap);
        }

        public void SetFpsCap(int newFpsCap)
        {
            ApplyFpsCap(newFpsCap);
            _fpsSettings.SetFpsCap(newFpsCap);
        }

        private static void ApplyFpsCap(int fpsCap) 
            => Application.targetFrameRate = fpsCap;
    }
}