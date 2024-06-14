using GameCode.UI;
using LevelGeneration.LevelsGenerators;
using UnityEngine;
using Zenject;

namespace GameCode.Core.SceneLoading
{
    public class GameplaySceneLoader : MonoBehaviour
    {
        [Inject] private readonly LevelGeneratorBase _levelGeneratorBase;
        
        private void Awake()
        {
            _levelGeneratorBase.OnGenerateFinished += LoadingEnded;
        }

        private void LoadingEnded()
        {
            _levelGeneratorBase.OnGenerateFinished -= LoadingEnded;
            LoadingScreen.Instance.EndLoading();
        }
    }
}