using System;
using UnityEngine;

public class KeyboardObserver : MonoBehaviour
{
    public event Action<float> OnMove;
    public event Action OnInventory;
    public event Action OnFirstAttack;
    public event Action OnSecondAttack;
    public event Action OnInteract;
    public event Action OnJump;
    public event Action OnDash;
    public event Action<int> OnAbilityUse;

    public event Action OnPlatformDrop;

    private void Update()
    {
        OnMove?.Invoke(Input.GetAxis("Horizontal"));
            
        if(Input.GetKeyDown(KeyCode.S))
            OnPlatformDrop?.Invoke();
            
        if (Input.GetMouseButton(0))
            OnFirstAttack?.Invoke();

        if (Input.GetMouseButtonDown(1))
            OnSecondAttack?.Invoke();
            
        if (Input.GetKeyDown(KeyCode.F))
            OnInteract?.Invoke();
        
        if (Input.GetKeyDown(KeyCode.Space))
            OnJump?.Invoke();

        if (Input.GetKeyDown(KeyCode.LeftShift))
            OnDash?.Invoke();
            
        if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Escape))
            OnInventory?.Invoke();
            
        for (int i = 1; i <= 9; i++)
        {
            KeyCode key = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Alpha" + i);
            if (Input.GetKeyDown(key))
            {
                OnAbilityUse?.Invoke(i);
                break;
            }
        }
    }
}