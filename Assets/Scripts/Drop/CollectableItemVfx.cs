using UnityEngine;

namespace GameCode.Drop
{
    [RequireComponent(typeof(Animator))]
    public class CollectableItemVfx : MonoBehaviour
    {
        private Animator _animator;
        
        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void ActivatePickUpAnimation() 
            => _animator.Play("Picked");
    }
}