using UnityEngine;

namespace GameCycleFramework.Example
{
    public class GameCycleStateChanger : MonoBehaviour
    {
        [SerializeField] private GameCycleController gameCycleController;
        
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.G))
                gameCycleController.SwitchState(GameCycleState.Gameplay);
            
            if(Input.GetKeyDown(KeyCode.P))
                gameCycleController.SwitchState(GameCycleState.Pause);
        }
    }
}