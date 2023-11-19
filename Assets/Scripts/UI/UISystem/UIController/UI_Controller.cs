using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class UI_Controller : MonoBehaviour
    {
        private static GameObject UI_Activ;
        private static GameObject UI_PrevActiv;
        private static GameObject buffer;

        void Start()
        {
            if (UI_ScreenRepository.GetScreen<UI_MainMenuScreen>().isActiveAndEnabled)
                UI_Activ = UI_ScreenRepository.GetScreen<UI_MainMenuScreen>().gameObject;
            else if (UI_ScreenRepository.GetScreen<UI_Gameplay>().isActiveAndEnabled)
                UI_Activ = UI_ScreenRepository.GetScreen<UI_Gameplay>().gameObject;
            else if (UI_ScreenRepository.GetScreen<UI_GameplayMenu>().isActiveAndEnabled)
                UI_Activ = UI_ScreenRepository.GetScreen<UI_GameplayMenu>().gameObject;
            else if (UI_ScreenRepository.GetScreen<UI_Inventory>().isActiveAndEnabled)
                UI_Activ = UI_ScreenRepository.GetScreen<UI_Inventory>().gameObject;
            else
            {
                Debug.LogError("Undefined screen or null");
                UI_Activ = null;
            }

            UI_PrevActiv = UI_Activ;
        }

        public static void SetWindow(ScreenEnum screen)
        {
            buffer = UI_Activ;
            UI_Activ.SetActive(false);

            switch (screen)
            {
                case ScreenEnum.Previous:
                    UI_Activ = UI_PrevActiv;
                    break;
                case ScreenEnum.MainMenu:
                    UI_Activ = UI_ScreenRepository.GetScreen<UI_MainMenuScreen>().gameObject;
                    break;
                case ScreenEnum.GameplayScreen:
                    UI_Activ = UI_ScreenRepository.GetScreen<UI_Gameplay>().gameObject;
                    break;
                case ScreenEnum.GameplayMenuScreen:
                    UI_Activ = UI_ScreenRepository.GetScreen<UI_GameplayMenu>().gameObject;
                    break;
                case ScreenEnum.Inventory:
                    UI_Activ = UI_ScreenRepository.GetScreen<UI_Inventory>().gameObject;
                    break;
                default:
                    Debug.LogError("Error: invalid string parameter in SetWindow(Screen screen)");
                    break;
            }

            UI_PrevActiv = buffer;
            UI_Activ.SetActive(true);
        }

        public static void LoadScene(int sceneBuildIndex)
        {
            if (sceneBuildIndex == -1)
            {
                int currentSceneNum = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadScene(currentSceneNum, LoadSceneMode.Single);
            }
            else
            {
                SceneManager.LoadScene(sceneBuildIndex);
            }
        }

        public static void Quit()
        {
            Application.Quit();
        }
    }
}