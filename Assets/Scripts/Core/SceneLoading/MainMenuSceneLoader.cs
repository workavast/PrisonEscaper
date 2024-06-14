using GameCode.UI;
using UnityEngine;

namespace GameCode.Core.SceneLoading
{
    public class MainMenuSceneLoader : MonoBehaviour
    {
        private void Start()
        {
            LoadingScreen.Instance.EndLoading();
        }
    }
}