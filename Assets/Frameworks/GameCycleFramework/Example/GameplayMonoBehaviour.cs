using UnityEngine;
using Zenject;

namespace GameCycleFramework.Example
{
    public class GameplayMonoBehaviour : MonoBehaviour, IGameCycleUpdate, IGameCycleFixedUpdate, IGameCycleEnter, IGameCycleExit
    {
        [Inject] private readonly IGameCycleController _gameCycleController;
        
        public GameCycleState GameCycleState => GameCycleState.Gameplay;
        
        protected void Awake()
        {
            _gameCycleController.AddListener(GameCycleState, this as IGameCycleUpdate);
            _gameCycleController.AddListener(GameCycleState, this as IGameCycleFixedUpdate);
            _gameCycleController.AddListener(GameCycleState, this as IGameCycleEnter);
            _gameCycleController.AddListener(GameCycleState, this as IGameCycleExit);
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
            _gameCycleController?.RemoveListener(GameCycleState, this as IGameCycleUpdate);
            _gameCycleController?.RemoveListener(GameCycleState, this as IGameCycleFixedUpdate);
            _gameCycleController?.RemoveListener(GameCycleState, this as IGameCycleEnter);
            _gameCycleController?.RemoveListener(GameCycleState, this as IGameCycleExit);
        }
    }
}