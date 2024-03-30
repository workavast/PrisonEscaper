using System;
using UnityEngine;

namespace GameCode.Light
{
    [RequireComponent(typeof(Animator))]
    public class BlinkingLamp : MonoBehaviour
    {
        private static readonly int AnimatorBlinkString = Animator.StringToHash("Blink");
        private Animator _animator;

        public event Action<BlinkingLamp> OnDestroyEvent;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            if (GlobalLampBlinks.Instance == null) 
                return;

            GlobalLampBlinks.Instance.AddLamp(this);
        }
    
        public void Blink()
        {
            _animator.SetTrigger(AnimatorBlinkString);
        }
        
        private void OnDestroy()
        {
            OnDestroyEvent?.Invoke(this);
        }
    }
}
