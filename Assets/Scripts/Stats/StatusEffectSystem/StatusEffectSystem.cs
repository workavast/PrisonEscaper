using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using GameCode.StatsSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using UniversalStatsSystem;


[Serializable]
public class StatusEffectSystem
{
    [Serializable]
    protected class StatusEffectData
    {
        public bool Active;
        public float Damage;
        public float Duration;
        public float CurrentTime;

        public StatusEffectData(bool active, float damage, float duration)
        {
            Active = active;
            Damage = damage;
            Duration = duration;
            CurrentTime = 0;
        }
    }

    private readonly IStatusEffectable _statusEffectable;
    private readonly EffectsVisualization _effectsVisualization;

    private StatusEffectData _fireStatus = new StatusEffectData(false, 0f,0f);
    private Coroutine _fireCoroutine;
    
    private StatusEffectData _frozenStatus = new StatusEffectData(false, 0f,0f);
    private Coroutine _frozenCoroutine;

    private StatusEffectData _electricityStatus = new StatusEffectData(false, 0f,0f);
    private Coroutine _electricityCoroutine;

    private StatusEffectData _poisonStatus = new StatusEffectData(false, 0f,0f);
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
    
    public StatusEffectSystem(IStatusEffectable statusEffectable, EffectsVisualization effectsVisualization)
    {
        _statusEffectable = statusEffectable;
        _effectsVisualization = effectsVisualization;
    }

    public void AddStatusEffects(AttackStats attackStats)
    {
        StatusEffects statusEffects = attackStats.statusEffects;
        
        if (Random.value < statusEffects.FireEffectChance)
        {
            if (_fireStatus.Active)
            {
                _fireStatus = new StatusEffectData(true, attackStats.fireDamage / 8, statusEffects.FireEffectDuration);
            }
            else
            {
                _fireStatus = new StatusEffectData(false, attackStats.fireDamage / 8, statusEffects.FireEffectDuration);
                _fireCoroutine = _statusEffectable.StatusEffectCoroutine.StartCoroutine(FireEffectTick());
            }
        }
        
        if (Random.value < statusEffects.FrozenEffectChance)
        {
            if (_frozenStatus.Active)
            {
                _frozenStatus = new StatusEffectData(true,0, statusEffects.FrozenEffectDuration);
            }
            else
            {
                _frozenStatus = new StatusEffectData(false,0, statusEffects.FrozenEffectDuration);
                _frozenCoroutine = _statusEffectable.StatusEffectCoroutine.StartCoroutine(FrozenEffectTick());
            }
        }

        if (Random.value < statusEffects.ElectricityEffectChance)
        {
            if (_electricityStatus.Active)
            {
                _electricityStatus = new StatusEffectData(true, attackStats.electricityDamage / 8,
                    statusEffects.ElectricityEffectDuration);
            }
            else
            {
                _electricityStatus = new StatusEffectData(false, attackStats.electricityDamage / 8,
                    statusEffects.ElectricityEffectDuration);
                _electricityCoroutine = _statusEffectable.StatusEffectCoroutine.StartCoroutine(ElectricityEffectTick());
            }
        }
        
        if (Random.value < statusEffects.PoisonEffectChance)
        {
            if (_poisonStatus.Active)
            {
                _poisonStatus = new StatusEffectData(true, attackStats.poisonDamage / 8,
                    statusEffects.PoisonEffectDuration);
            }
            else
            {
                _poisonStatus = new StatusEffectData(false, attackStats.poisonDamage / 8,
                    statusEffects.PoisonEffectDuration);
                _poisonCoroutine = _statusEffectable.StatusEffectCoroutine.StartCoroutine(PoisonEffectTick());
            }
        }
    }
    
    
    private IEnumerator FireEffectTick()
    {
        OnFireStatusStart?.Invoke();
        _effectsVisualization.SetActiveEffect(StatusEffect.Fire, true);
        _fireStatus.Active = true;
        _fireStatus.CurrentTime = 0f;
        
        float timePause = 1f;
        while (_fireStatus.Active)
        {
            yield return new WaitForSeconds(timePause);

            AttackStats attackStats = new AttackStats(0, _fireStatus.Damage, 0, 0, 0, 0, 0);
            _statusEffectable.TakeDamage(attackStats);
            
            _fireStatus.CurrentTime += timePause;
            if (_fireStatus.Duration + timePause <= _fireStatus.CurrentTime)
                _fireStatus.Active = false;
        }

        _effectsVisualization.SetActiveEffect(StatusEffect.Fire, false);
        OnFireStatusEnd?.Invoke();
    }
    
    private IEnumerator FrozenEffectTick()
    {
        OnFrozenStatusStart?.Invoke();
        _effectsVisualization.SetActiveEffect(StatusEffect.Frozen, true);

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
        
        _effectsVisualization.SetActiveEffect(StatusEffect.Frozen, false);
        OnFrozenStatusEnd?.Invoke();
    }
    
    private IEnumerator ElectricityEffectTick()
    {
        OnElectricityStatusStart?.Invoke();
        _effectsVisualization.SetActiveEffect(StatusEffect.Electricity, true);

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
        
        _effectsVisualization.SetActiveEffect(StatusEffect.Electricity, false);
        OnElectricityStatusEnd?.Invoke();
    }
    
    private IEnumerator PoisonEffectTick()
    {
        OnPoisonStatusStart?.Invoke();
        _effectsVisualization.SetActiveEffect(StatusEffect.Poison, true);

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

        _effectsVisualization.SetActiveEffect(StatusEffect.Poison, false);
        OnPoisonStatusEnd?.Invoke();
    }
    
    
    private void StopFireStatusEffect()
    {
        if (_fireStatus.Active)
        {
            _fireStatus.Active = false;

            if (_fireCoroutine != null)
                _statusEffectable.StatusEffectCoroutine.StopCoroutine(_fireCoroutine);
            
            _effectsVisualization.SetActiveEffect(StatusEffect.Fire, false);
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
            
            _effectsVisualization.SetActiveEffect(StatusEffect.Frozen, false);
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
            
            _effectsVisualization.SetActiveEffect(StatusEffect.Electricity, false);
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

            _effectsVisualization.SetActiveEffect(StatusEffect.Poison, false);
            OnPoisonStatusEnd?.Invoke();
        }
    }
}
