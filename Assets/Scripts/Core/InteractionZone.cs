using System;
using Core;
using UnityEngine;

namespace GameCode.Core
{
    public class InteractionZone : MonoBehaviour, IInteractive
    {
        [field: SerializeField] public bool Interactable { get; private set; } = true;
        public event Action OnInteract;
        
        public void SetInteractionState(bool interactable) 
            => Interactable = interactable;

        public void Interact()
            => OnInteract?.Invoke();
    }
}