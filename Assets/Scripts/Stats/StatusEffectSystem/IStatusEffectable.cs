using UniversalStatsSystem;
using UnityEngine;

public interface IStatusEffectable
{
    MonoBehaviour StatusEffectCoroutine { get; }
    public Vector2 Position { get; }

    public void AddStatusEffect(AttackStats attackStats);
    public void TakeDamage(AttackStats attackStats);
}
