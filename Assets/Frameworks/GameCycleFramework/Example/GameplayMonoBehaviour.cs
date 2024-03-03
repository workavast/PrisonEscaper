using UnityEngine;

namespace GameCycleFramework.Example
{
    public class GameplayMonoBehaviour : MonoBehaviour, IGameCycleUpdate, IGameCycleFixedUpdate, IGameCycleEnter, IGameCycleExit
    {
        [SerializeField] protected GameCycleController gameCycleController;
        public GameCycleState GameCycleState => GameCycleState.Gameplay;
        
        protected void Awake()
        {
            gameCycleController.AddListener(GameCycleState, this as IGameCycleUpdate);
            gameCycleController.AddListener(GameCycleState, this as IGameCycleFixedUpdate);
            gameCycleController.AddListener(GameCycleState, this as IGameCycleEnter);
            gameCycleController.AddListener(GameCycleState, this as IGameCycleExit);
        }
                
        public void GameCycleUpdate()
        {
            Debug.Log("GameCycleUpdate");
        }

        public void GameCycleFixedUpdate()
        {
            Debug.Log("GameCycleFixedUpdate");
        }
        
        public void GameCycleEnter()
        {
            Debug.Log("GameCycleEnter");
        }

        public void GameCycleExit()
        {
            Debug.Log("GameCycleExit");
        }
        
        private void OnDestroy()
        {
            gameCycleController.RemoveListener(GameCycleState, this as IGameCycleUpdate);
            gameCycleController.RemoveListener(GameCycleState, this as IGameCycleFixedUpdate);
            gameCycleController.AddListener(GameCycleState, this as IGameCycleEnter);
            gameCycleController.AddListener(GameCycleState, this as IGameCycleExit);
        }
    }
}