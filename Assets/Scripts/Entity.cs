using System;
using System.Collections;
using Ability_system;
using New_Folder.Healthbar;
using Stats_system;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public abstract class Entity : MonoBehaviour
{
    [Header("Entity")] 
    [SerializeField] protected AbilitySystem _abilitySystem;
    
    [SerializeField] private HealthBar _healthBar;
    [SerializeField] protected Animator _animator;
    
    private float _deathCooldown = 0.5f;
    public bool IsAlive { get; private set; }

    public UnityEvent DeathEvent;

    // public string _name;

    protected virtual void Awake()
    {
        IsAlive = true;
        _healthBar?.SetMaxHealth(_abilitySystem.Stats.MaxHealth);
        _healthBar?.SetHealth(_abilitySystem.Stats.Health);
        DeathEvent = new UnityEvent();
    }

    public void TakeDamage(float damage)
    {
        if (IsAlive && damage >= 0)
        {
            _animator.SetTrigger("Hurt");

            _abilitySystem.TakeDamage(damage);
            
            Debug.Log($"{name} recieved {damage} damage! Health: {_abilitySystem.Stats.Health}");

            _healthBar?.SetHealth(_abilitySystem.Stats.Health);

            if (_abilitySystem.Stats.Health <= 0)
            {
                Die();
            }
        }
    }

    public void AddHealth(float addedHealth)
    {
        _abilitySystem.Stats.Health += addedHealth;
        
        if (_abilitySystem.Stats.Health > _abilitySystem.Stats.MaxHealth)
            _abilitySystem.Stats.Health = _abilitySystem.Stats.MaxHealth;
        
        _healthBar?.SetHealth(_abilitySystem.Stats.Health);
    }

    private void Die()
    {
        IsAlive = false;
        DeathEvent.Invoke();
        _animator.SetTrigger("Dead");
        StartCoroutine(DestroyAfterTime());
    }

    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(_deathCooldown);
        Destroy(gameObject);
    }
}