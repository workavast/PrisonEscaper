using System;
using Projectiles;
using UnityEngine;

namespace GameCode
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleEffector : MonoBehaviour
    {
        [SerializeField] private ProjectileBase projectileBase;
        
        private void Awake()
        {
            Hide();
            projectileBase.ReturnElementEvent += (_) => Hide();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }
        
        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}