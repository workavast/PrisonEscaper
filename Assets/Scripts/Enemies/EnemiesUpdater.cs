using System.Linq;
using GameCycleFramework;
using UnityEngine;
using Zenject;

namespace Enemies
{
    public class EnemiesUpdater : MonoBehaviour, IGameCycleUpdate, IGameCycleFixedUpdate
    {
        [Inject] private readonly IGameCycleController _gameCycleController;
        [Inject] private readonly EnemiesRepository _enemiesRepository;

        private void Awake()
        {
            _gameCycleController.AddListener(GameCycleState.Gameplay, this as IGameCycleUpdate);
            _gameCycleController.AddListener(GameCycleState.Gameplay, this as IGameCycleFixedUpdate);
        }

        public void GameCycleUpdate()
        {
            float deltaTime = Time.deltaTime;
            var enemies = _enemiesRepository.Enemies.ToList();
            foreach (var enemy in enemies)
                enemy.HandleUpdate(deltaTime);
        }
        
        public void GameCycleFixedUpdate()
        {
            float fixedDeltaTime = Time.fixedDeltaTime;
            var enemies = _enemiesRepository.Enemies.ToList();
            foreach (var enemy in enemies)
                enemy.HandleFixedUpdate(fixedDeltaTime);
        }

        private void OnDestroy()
        {
            _gameCycleController?.RemoveListener(GameCycleState.Gameplay, this as IGameCycleUpdate);
            _gameCycleController?.RemoveListener(GameCycleState.Gameplay, this as IGameCycleFixedUpdate);
        }
    }
}