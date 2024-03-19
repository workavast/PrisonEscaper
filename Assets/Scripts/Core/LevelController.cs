using GameCycleFramework;
using LevelGeneration.LevelsGenerators;
using UnityEngine;
using Zenject;

namespace Core
{
    public class LevelController : MonoBehaviour
    {
        [Inject] private IGameCycleSwitcher _gameCycleSwitcher;
        [Inject] private LevelGeneratorBase _levelGeneratorBase;
        
        private void Awake()
        {
            _levelGeneratorBase.OnGenereationStart += () => _gameCycleSwitcher.SwitchState(GameCycleState.LocationGeneration);
            _levelGeneratorBase.OnGenerateFinished += () => _gameCycleSwitcher.SwitchState(GameCycleState.Gameplay);
        }
    }
}