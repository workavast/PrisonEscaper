using Core;
using GameCode.StatsSystem;
using UnityEngine;
using UniversalStatsSystem;

namespace MyNamespace
{
    public abstract class CharacterBase : MonoBehaviour, IDamageable, IStatusEffectable
    {
        [SerializeField] private EffectsVisualization effectsVisualization;
        [field: SerializeField] public StatsSystem StatsSystem { get; private set; }
        public float Health => StatsSystem.Health.CurrentValue;
        public bool Invincible { get => StatsSystem.isInvincible; protected set => StatsSystem.isInvincible = value; }
    
        protected StatusEffectSystem StatusEffectSystem;
        public MonoBehaviour StatusEffectCoroutine => this;
        public Vector2 Position => transform.position;
    
        private void Awake()
            => OnAwake();

        private void Start()
            => OnStart();
    
        protected virtual void OnAwake()
        {
            StatusEffectSystem = new StatusEffectSystem(this, effectsVisualization);
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
}
