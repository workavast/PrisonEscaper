using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BlinkingLamp : MonoBehaviour
{
    private Animator _animator;
    
    public event Action<BlinkingLamp> OnDestroyEvent;

    void Start()
    {
        if (GlobalLampBlinks.Instance == null) return;

        _animator = GetComponent<Animator>();
        GlobalLampBlinks.Instance.AddLamp(this);
    }
    
    private void OnDestroy()
    {
        OnDestroyEvent?.Invoke(this);
    }

    public void Blink()
    {
        _animator.SetTrigger("Blink");
    }
}
