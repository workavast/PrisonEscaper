using GameCode.PGD;
using GameCode.UI.Elements;
using GameCycleFramework;
using LevelGeneration.LevelsGenerators;
using UI;
using UnityEngine;
using Zenject;

namespace Core
{
    public class LevelController : MonoBehaviour
    {
        [Inject] private IGameCycleSwitcher _gameCycleSwitcher;
        [Inject] private LevelGeneratorBase _levelGeneratorBase;
        [Inject] private KeyboardObserver _keyboardObserver;
        
        private void Awake()
        {
            _levelGeneratorBase.OnGenerationStart += () => _gameCycleSwitcher.SwitchState(GameCycleState.LocationGeneration);
            _levelGeneratorBase.OnGenerateFinished += CheckTrain;
        }

        private void CheckTrain()
        {
            if (PlayerGlobalData.Instance.TutorialSettings.TutorialCompleted)
            {
                _keyboardObserver.OnInventory += ToggleInventory;
                _gameCycleSwitcher.SwitchState(GameCycleState.Gameplay);
                return;
            }
            
            _gameCycleSwitcher.SwitchState(GameCycleState.Pause);
            var trainWindow = FindObjectOfType<TutorialWindow>(true);
            if (trainWindow == null)
            {
                _keyboardObserver.OnInventory += ToggleInventory;
                _gameCycleSwitcher.SwitchState(GameCycleState.Gameplay);
                return;
            }
            
            trainWindow.Open();
            trainWindow.TutorialCompleted += () =>
            {
                _keyboardObserver.OnInventory += ToggleInventory;
                _gameCycleSwitcher.SwitchState(GameCycleState.Gameplay);
            };
        }
        
        private void ToggleInventory()
        {
            var inventoryUi = UI_ScreenRepository.GetScreen<UI_Inventory>();
            if (!inventoryUi.gameObject.activeInHierarchy)
            {
                UI_Controller.SetWindow(ScreenEnum.Inventory);
                Cursor.visible = true;
            }
            else
            {
                UI_Controller.SetWindow(ScreenEnum.GameplayScreen);
                Cursor.visible = false;
            }
        }
    }
}