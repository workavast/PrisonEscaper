using System;
using System.Collections.Generic;
using EnumValuesLibrary;
using UnityEngine;
using Zenject;

namespace Enemies
{
    public class EnemiesFactory : MonoBehaviour
    {
        [Inject] private readonly EnemyPrefabsConfig _prefabsConfig;
        [Inject] private readonly DiContainer _container;
        
        private readonly Dictionary<EnemyID, Transform> _parents = new();

        public event Action<EnemyBase> OnEnemyCreate;
    
        private void Awake()
        {
            var enemyIds = EnumValuesTool.GetValues<EnemyID>();
            foreach (var enemyID in enemyIds)
            {
                var parent = new GameObject()
                {
                    name = enemyID.ToString(),
                    transform = { parent = this.gameObject.transform }
                };
                _parents.Add(enemyID, parent.transform);
            }
        }

        public EnemyBase Create(EnemyID id) 
            => Create(id, Vector3.zero);
    
        public EnemyBase Create(EnemyID id, Vector3 position)
        {
            if (!_prefabsConfig.Data.ContainsKey(id))
                throw new IndexOutOfRangeException($"Type {id} dont present in {_prefabsConfig}");
        
            if (!_parents.ContainsKey(id))
                throw new IndexOutOfRangeException($"Type {id} dont present in {_parents}");

            var enemy = _container.InstantiatePrefab(_prefabsConfig.Data[id], position, Quaternion.identity, _parents[id]);

            if (!enemy.TryGetComponent(out EnemyBase enemyBase))
                throw new NullReferenceException($"Prefab with Key {id} dont have script {nameof(EnemyBase)}");
            OnEnemyCreate?.Invoke(enemyBase);
            return enemyBase;
        }
    }
}