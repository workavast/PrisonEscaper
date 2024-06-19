using UnityEngine;

namespace UI
{
    public class UI_MainMenuScreen : UI_ScreenBase
    {
        [SerializeField] private GameObject settingsScreen;
        [SerializeField] private GameObject creditsScreen;
        
        public override void InitScreen()
        {
            base.InitScreen();

            settingsScreen.SetActive(false);
            creditsScreen.SetActive(false);
            Cursor.visible = true;
        }

        public void _OpenSettingsScreen()
        {
            settingsScreen.SetActive(true);
            creditsScreen.SetActive(false);
        }
        
        public void _OpenCreditsScreenScreen()
        {
            settingsScreen.SetActive(false);
            creditsScreen.SetActive(true);
        }
    }
}