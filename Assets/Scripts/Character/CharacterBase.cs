using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalStatsSystem;

public abstract class CharacterBase : MonoBehaviour, IDamageable, IStatusEffectable
{
    [field: SerializeField] public StatsSystem StatsSystem { get; private set; }
    public float Health => StatsSystem.MainStats.Health;
    public bool Invincible { get => StatsSystem.isInvincible; protected set => StatsSystem.isInvincible = value; }

    
    protected StatusEffectSystem StatusEffectSystem = new StatusEffectSystem();
    public MonoBehaviour StatusEffectCoroutine => this;
    public Vector2 Position => transform.position;
    
    private void Awake()
    {
        OnAwake();
    }

    private void Start()
    {
        OnStart();
    }

    private void Update()
    {
        OnUpdate();
    }

    private void FixedUpdate()
    {
        OnFixedUpdate();
    }
    
    protected virtual void OnAwake()
    {
        StatusEffectSystem.OnAwake(this);
    }

    protected virtual void OnStart ()
    {
        
    }

    protected virtual void OnUpdate()
    {
        
    }
		
    protected virtual void OnFixedUpdate()
    {

    }

    public virtual void TakeDamage(AttackStats attackStats)
    {
        AddStatusEffect(attackStats);
    }

    public virtual void AddStatusEffect(AttackStats attackStats)
    {
        StatusEffectSystem.AddStatusEffects(attackStats);
    }
}
