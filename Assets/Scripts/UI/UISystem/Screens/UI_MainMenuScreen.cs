using UnityEngine;

namespace UI
{
    public class UI_MainMenuScreen : UI_ScreenBase
    {
        [SerializeField] private GameObject settingsScreen;
        
        public override void InitScreen()
        {
            base.InitScreen();

            settingsScreen.SetActive(false);
            Cursor.visible = true;
        }

        public void _OpenSettingsScreen()
        {
            settingsScreen.SetActive(true);
        }
    }
}