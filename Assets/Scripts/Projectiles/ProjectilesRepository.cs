using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Projectiles
{
    public class ProjectilesRepository : MonoBehaviour
    {
        [Inject] private ProjectileFactory _projectilesFactory;

        public IReadOnlyList<ProjectileBase> Projectiles => _projectiles;
    
        private readonly List<ProjectileBase> _projectiles = new();

        private void Awake()
        {
            _projectilesFactory.OnCreate += Add;
        }
    
        private void Add(ProjectileBase projectile)
        {
            if (_projectiles.Contains(projectile))
                throw new Exception("Duplicate");
        
            projectile.ReturnElementEvent += Remove;
            projectile.DestroyElementEvent += Remove;
            _projectiles.Add(projectile);
        }
    
        private void Remove(ProjectileBase projectile)
        {
            projectile.ReturnElementEvent -= Remove;
            projectile.DestroyElementEvent += Remove;
            _projectiles.Remove(projectile);
        }
    }
}