using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using UniversalStatsSystem;


[Serializable]
public class StatusEffectSystem
{
    [Serializable]
    protected class StatusEffect
    {
        public bool Active;
        public float Damage;
        public float Duration;
        public float CurrentTime;

        public StatusEffect(bool active, float damage, float duration)
        {
            Active = active;
            Damage = damage;
            Duration = duration;
            CurrentTime = 0;
        }
    }

    private IStatusEffectable _statusEffectable;

    private StatusEffect _fireStatus = new StatusEffect(false, 0f,0f);
    private Coroutine _fireCoroutine;
    
    private StatusEffect _frozenStatus = new StatusEffect(false, 0f,0f);
    private Coroutine _frozenCoroutine;

    private StatusEffect _electricityStatus = new StatusEffect(false, 0f,0f);
    private Coroutine _electricityCoroutine;

    private StatusEffect _poisonStatus = new StatusEffect(false, 0f,0f);
    private Coroutine _poisonCoroutine;
    
    public bool FireStatusActive => _fireStatus.Active;
    public bool FrozenStatusActive => _frozenStatus.Active;
    public bool ElectricityStatusActive => _fireStatus.Active;
    public bool PoisonStatusActive => _fireStatus.Active;

    public event Action OnFireStatusStart; 
    public event Action OnFrozenStatusStart; 
    public event Action OnElectricityStatusStart; 
    public event Action OnPoisonStatusStart;

    public event Action OnFireStatusEnd; 
    public event Action OnFrozenStatusEnd; 
    public event Action OnElectricityStatusEnd; 
    public event Action OnPoisonStatusEnd;
    
    public void OnAwake(IStatusEffectable statusEffectable)
    {
        _statusEffectable = statusEffectable;
    }

    public void AddStatusEffects(AttackStats attackStats)
    {
        StatusEffects statusEffects = attackStats.statusEffects;
        
        if (Random.value < statusEffects.FireEffectChance)
        {
            if (_fireStatus.Active)
            {
                _fireStatus = new StatusEffect(true, attackStats.fireDamage / 10, statusEffects.FireEffectDuration);
            }
            else
            {
                _fireStatus = new StatusEffect(false, attackStats.fireDamage/10, statusEffects.FireEffectDuration);
                _fireCoroutine = _statusEffectable.StatusEffectCoroutine.StartCoroutine(FireEffectTick());
            }
        }
        
        if (Random.value < statusEffects.FrozenEffectChance)
        {
            if (_frozenStatus.Active)
            {
                _frozenStatus = new StatusEffect(true,0, statusEffects.FrozenEffectDuration);
            }
            else
            {
                _frozenStatus = new StatusEffect(false,0, statusEffects.FrozenEffectDuration);
                _frozenCoroutine = _statusEffectable.StatusEffectCoroutine.StartCoroutine(FrozenEffectTick());
            }
        }

        if (Random.value < statusEffects.ElectricityEffectChance)
        {
            if (_electricityStatus.Active)
            {
                _electricityStatus = new StatusEffect( true, attackStats.electricityDamage/10, statusEffects.ElectricityEffectDuration);
            }
            else
            {
                _electricityStatus = new StatusEffect( false, attackStats.electricityDamage/10, statusEffects.ElectricityEffectDuration);
                _electricityCoroutine = _statusEffectable.StatusEffectCoroutine.StartCoroutine(ElectricityEffectTick());
            }
        }
        
        if (Random.value < statusEffects.PoisonEffectChance)
        {
            if (_poisonStatus.Active)
            {
                _poisonStatus = new StatusEffect(true, attackStats.poisonDamage / 10, statusEffects.PoisonEffectDuration);
            }
            else
            {
                _poisonStatus = new StatusEffect(false, attackStats.poisonDamage/10, statusEffects.PoisonEffectDuration);
                _poisonCoroutine = _statusEffectable.StatusEffectCoroutine.StartCoroutine(PoisonEffectTick());
            }
        }
    }
    
    
    private IEnumerator FireEffectTick()
    {
        OnFireStatusStart?.Invoke();
        _fireStatus.Active = true;
        _fireStatus.CurrentTime = 0f;
        
        float timePause = 1f;
        while (_fireStatus.Active)
        {
            yield return new WaitForSeconds(timePause);
            
            AttackStats attackStats = new AttackStats(0,_fireStatus.Damage,0,0,0,0,0);
            _statusEffectable.TakeDamage(attackStats);
            
            _fireStatus.CurrentTime += timePause;
            if (_fireStatus.Duration + timePause <= _fireStatus.CurrentTime)
                _fireStatus.Active = false;
        }

        OnFireStatusEnd?.Invoke();
    }
    
    private IEnumerator FrozenEffectTick()
    {
        OnFrozenStatusStart?.Invoke();
        _frozenStatus.Active = true;
        _frozenStatus.CurrentTime = 0f;

        float timePause = 1f;
        while (_frozenStatus.Active)
        {
            yield return new WaitForSeconds(timePause);
            
            _frozenStatus.CurrentTime += timePause;
            if (_frozenStatus.Duration + timePause <= _frozenStatus.CurrentTime)
                _frozenStatus.Active = false;
        }
        
        OnFrozenStatusEnd?.Invoke();
    }
    
    private IEnumerator ElectricityEffectTick()
    {
        OnElectricityStatusStart?.Invoke();
        _electricityStatus.Active = true;
        _electricityStatus.CurrentTime = 0f;

        float timePause = 1f;
        while (_electricityStatus.Active)
        {
            yield return new WaitForSeconds(timePause);

            AttackStats attackStats = new AttackStats(0, 0, 0, 0, 0, _electricityStatus.Damage, 0);
            Collider2D[] targets = Physics2D.OverlapCircleAll(_statusEffectable.Position, 5F);
            foreach (var target in targets)
                if (target.CompareTag("Enemy"))
                    target.GetComponent<Enemy>().TakeDamage(attackStats);

            
            _electricityStatus.CurrentTime += timePause;
            if(_electricityStatus.Duration + timePause <= _electricityStatus.CurrentTime)
                _electricityStatus.Active = false;
        }
        
        OnElectricityStatusEnd?.Invoke();
    }
    
    private IEnumerator PoisonEffectTick()
    {
        OnPoisonStatusStart?.Invoke();
        _poisonStatus.Active = true;
        _poisonStatus.CurrentTime = 0f;
        
        float timePause = 1f;
        while (_poisonStatus.Active)
        {
            yield return new WaitForSeconds(timePause);

            AttackStats attackStats = new AttackStats(0,0,0,0,0,0,_poisonStatus.Damage);
            _statusEffectable.TakeDamage(attackStats);

            _poisonStatus.CurrentTime += timePause;
            if (_poisonStatus.Duration + timePause <= _poisonStatus.CurrentTime)
                _poisonStatus.Active = false;
        }

        OnPoisonStatusEnd?.Invoke();
    }
    
    
    private void StopFireStatusEffect()
    {
        if (_fireStatus.Active)
        {
            _fireStatus.Active = false;

            if (_fireCoroutine != null)
                _statusEffectable.StatusEffectCoroutine.StopCoroutine(_fireCoroutine);
            
            OnFireStatusEnd?.Invoke();
        }
    }
    
    private void StopFrozeStatusEffect()
    {
        if (_frozenStatus.Active)
        {
            _frozenStatus.Active = false;

            if (_frozenCoroutine != null)
                _statusEffectable.StatusEffectCoroutine.StopCoroutine(_frozenCoroutine);
            
            OnFrozenStatusEnd?.Invoke();
        }

    }
    
    private void StopElectricityStatusEffect()
    {
        if (_electricityStatus.Active)
        {
            _electricityStatus.Active = false;

            if (_electricityCoroutine != null)
                _statusEffectable.StatusEffectCoroutine.StopCoroutine(_electricityCoroutine);
            
            OnElectricityStatusEnd?.Invoke();
        }
    }

    private void StopPoisonStatusEffect()
    {
        if (_poisonStatus.Active)
        {
            _poisonStatus.Active = false;

            if (_poisonCoroutine != null)
                _statusEffectable.StatusEffectCoroutine.StopCoroutine(_poisonCoroutine);

            OnPoisonStatusEnd?.Invoke();
        }
    }
}
