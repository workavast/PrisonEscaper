using System;
using GameCode.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameCode.Core.SceneLoading
{
    public class SceneLoader : MonoBehaviour
    {
        // public const int GAME_BOOTSTRAP_SCENE_INDEX = 0;
        public const int LOAD_SCENE_INDEX = 3;
        
        private static int _targetSceneIndex = -1;

        public event Action LoadingStarted;
        
        public void Start()
        {
            var activeSceneIndex = SceneManager.GetActiveScene().buildIndex;

            // if (activeSceneIndex != GAME_BOOTSTRAP_SCENE_INDEX && _targetSceneIndex <= -1)
            // {
                // LoadingScreen.Instance.EndLoading();
                // return;
            // }
            
            if (_targetSceneIndex <= -1)
                return;

            if (activeSceneIndex == LOAD_SCENE_INDEX)
                LoadTargetScene();
            
            // if(activeSceneIndex == _targetSceneIndex)
                // LoadingScreen.Instance.EndLoading();
        }
        
        public void LoadScene(int index)
        {
            _targetSceneIndex = index;
            
            LoadingStarted?.Invoke();
            
            LoadingScreen.Instance.StartLoading();
            SceneManager.LoadSceneAsync("Scenes/LoadScene");
        }

        private void LoadTargetScene()
        {
            SceneManager.LoadSceneAsync(_targetSceneIndex);
        }
    }
}