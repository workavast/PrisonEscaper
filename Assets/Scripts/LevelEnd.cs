using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEnd : MonoBehaviour
{
    [SerializeField] private LocationID nextLocation;

    public void SwitchLevel()
    {
        Time.timeScale = 0;

        switch (nextLocation)
        {
            case LocationID.None:
                UI.UI_Controller.SetWindow(UI.ScreenEnum.GameplayMenuScreen);
                break;
            case LocationID.Caves:
                SceneManager.LoadScene("Caves");
                break;
            case LocationID.Canalization:
                SceneManager.LoadScene("Canalization");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnDestroy()
    {
        Time.timeScale = 1;
    }
}
