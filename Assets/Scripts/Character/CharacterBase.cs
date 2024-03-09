using UnityEngine;
using UniversalStatsSystem;

public abstract class CharacterBase : MonoBehaviour, IDamageable, IStatusEffectable
{
    [field: SerializeField] public StatsSystem StatsSystem { get; private set; }
    public float Health => StatsSystem.Health.CurrentValue;
    public bool Invincible { get => StatsSystem.isInvincible; protected set => StatsSystem.isInvincible = value; }
    
    protected StatusEffectSystem StatusEffectSystem = new StatusEffectSystem();
    public MonoBehaviour StatusEffectCoroutine => this;
    public Vector2 Position => transform.position;
    
    private void Awake()
        => OnAwake();

    private void Start()
        => OnStart();
    
    protected virtual void OnAwake()
    {
        StatusEffectSystem.OnAwake(this);
    }

    protected virtual void OnStart ()
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
