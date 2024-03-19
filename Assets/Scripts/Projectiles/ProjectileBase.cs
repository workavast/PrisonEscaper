using System;
using PoolSystem;
using UnityEngine;

namespace Projectiles
{
    public abstract class ProjectileBase : MonoBehaviour, IPoolable<ProjectileBase, ProjectileId>
    {
        [SerializeField] private ProjectileId projectileId;
        
        public ProjectileId PoolId => projectileId;
        
        public event Action<ProjectileBase> ReturnElementEvent;
        public event Action<ProjectileBase> DestroyElementEvent;
        
        public abstract void HandleFixedUpdate(float fixedDeltaTime);

        public virtual void OnElementExtractFromPool()
            => gameObject.SetActive(true);

        public virtual void OnElementReturnInPool()
            => gameObject.SetActive(false);

        protected void ReturnInPool() 
            => ReturnElementEvent?.Invoke(this);

        private void OnDestroy()
            => DestroyElementEvent?.Invoke(this);
    }
}