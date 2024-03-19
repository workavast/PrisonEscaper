using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Enemies
{
    public class EnemiesRepository : MonoBehaviour
    { 
        [Inject] private EnemiesFactory _enemiesFactory;

        public IReadOnlyList<EnemyBase> Enemies => _enemies;
    
        private readonly List<EnemyBase> _enemies = new();

        private void Awake()
        {
            _enemiesFactory.OnEnemyCreate += AddEnemy;
        }
    
        private void AddEnemy(EnemyBase enemyBase)
        {
            if (_enemies.Contains(enemyBase))
                throw new Exception("Duplicate");
        
            enemyBase.OnEndDie += Remove;
            _enemies.Add(enemyBase);
        }
    
        private void Remove(EnemyBase enemy)
        {
            enemy.OnEndDie -= Remove;
            _enemies.Remove(enemy);
        }
    }
}