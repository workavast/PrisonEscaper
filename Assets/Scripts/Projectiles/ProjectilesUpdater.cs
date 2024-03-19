using System.Linq;
using GameCycleFramework;
using UnityEngine;
using Zenject;

namespace Projectiles
{
    public class ProjectilesUpdater : MonoBehaviour, IGameCycleFixedUpdate
    {
        [Inject] private readonly IGameCycleController _gameCycleController;
        [Inject] private readonly ProjectilesRepository _repository;

        private void Awake()
        {
            _gameCycleController.AddListener(GameCycleState.Gameplay, this as IGameCycleFixedUpdate);
        }
        
        public void GameCycleFixedUpdate()
        {
            float fixedDeltaTime = Time.fixedDeltaTime;
            var projectiles = _repository.Projectiles.ToList();
            foreach (var projectile in projectiles)
                projectile.HandleFixedUpdate(fixedDeltaTime);
        }

        private void OnDestroy()
        {
            _gameCycleController?.RemoveListener(GameCycleState.Gameplay, this as IGameCycleFixedUpdate);
        }
    }
}