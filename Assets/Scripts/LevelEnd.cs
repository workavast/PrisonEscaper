using System;
using GameCode.Core.SceneLoading;
using UnityEngine;
using Zenject;

public class LevelEnd : MonoBehaviour
{
    [SerializeField] private LocationID nextLocation;

    [Inject] private readonly SceneLoader _sceneLoader;

    public void SwitchLevel()
    {
        Time.timeScale = 0;

        switch (nextLocation)
        {
            case LocationID.None:
                UI.UI_Controller.SetWindow(UI.ScreenEnum.GameplayMenuScreen);
                break;
            case LocationID.Caves:
                _sceneLoader.LoadScene(1);
                break;
            case LocationID.Canalization:
                _sceneLoader.LoadScene(2);
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
