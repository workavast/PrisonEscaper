using System;
using System.Collections.Generic;
using EnumValuesLibrary;
using PoolSystem;
using UnityEngine;
using Zenject;

namespace Core
{
    public class PoolFactoryBase<TEnum, TBase, TConfig> : MonoBehaviour
        where TEnum : Enum
        where TBase : MonoBehaviour, PoolSystem.IPoolable<TBase, TEnum>
        where TConfig : PrefabsConfigBase<TEnum, TBase>
    {
        [Inject] private TConfig _prefabsConfig;
        [Inject] private DiContainer _container;

        private Pool<TBase, TEnum> _pool;
        private readonly Dictionary<TEnum, Transform> _parents = new();

        public event Action<TBase> OnCreate; 
        
        private void Awake()
        {
            _pool = new Pool<TBase, TEnum>(InstantiateProjectile);
            
            var types = EnumValuesTool.GetValues<TEnum>();
            foreach (var type in types)
            {
                var parent = new GameObject()
                {
                    name = type.ToString(),
                    transform = { parent = this.gameObject.transform }
                };
                _parents.Add(type, parent.transform);
            }
        }

        public TBase Create(TEnum id) 
            => Create(id, Vector3.zero, Quaternion.identity);

        public TBase Create(TEnum id, Vector3 position)
            => Create(id, position, Quaternion.identity);
        
        public TBase Create(TEnum id, Vector3 position, Quaternion rotation)
        {
            _pool.ExtractElement(id, out var @base);
            @base.transform.position = position;
            @base.transform.rotation = rotation;
            OnCreate?.Invoke(@base);
            
            return @base;
        }
    
        private TBase InstantiateProjectile(TEnum id)
        {
            if (!_prefabsConfig.Data.ContainsKey(id))
                throw new IndexOutOfRangeException($"Type {id} dont present in {_prefabsConfig}");
                    
            var newObject = _container.InstantiatePrefab(_prefabsConfig.Data[id], _parents[id]);
            
            if (!newObject.TryGetComponent<TBase>(out var @base))
                throw new NullReferenceException($"Prefab with Key {id} dont have script {nameof(TBase)}");
            
            return @base;
        }
    }
}