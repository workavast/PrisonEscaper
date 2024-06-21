using System;
using UnityEngine;

namespace GameCode.Core
{
    public class TriggerZone2D : MonoBehaviour
    {
        public event Action<Collider2D> OnTriggerEnter2DEvent;
        public event Action<Collider2D> OnTriggerExit2DEvent;
        
        private void OnTriggerEnter2D(Collider2D other) 
            => OnTriggerEnter2DEvent?.Invoke(other);

        private void OnTriggerExit2D(Collider2D other) 
            => OnTriggerExit2DEvent?.Invoke(other);
    }
}