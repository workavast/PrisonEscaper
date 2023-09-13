using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

[RequireComponent(typeof(ItemDropper))]
public class Enemy2 : Entity
{
    //TODO fix attack mechanic because too hard to kill without take damage 
    
    [Header("Enemy")]
    [Space]
    [Header("Moving")]
    [SerializeField] private float _movespeed;
    [SerializeField] private Vector2 _patrolRange;
    [SerializeField] private float _stayCooldown;

    [Header("Attack")]
    // [SerializeField] private float _damage;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _attackCooldown;
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private Transform _target;
    
    [SerializeField] private AudioClip walk, attack;
    
        

    private AudioSource _source;
    private Vector3 _startPosition;
    private ItemDropper _itemDropper;
    
    private Rigidbody2D _rigidbody;
    private bool _stay;
    private bool _following, _attacking;
    private float direction = 1f;
    
    void Start()
    {
        _startPosition = transform.position;
        _rigidbody = GetComponent<Rigidbody2D>();
        _itemDropper = GetComponent<ItemDropper>();
        DeathEvent.AddListener(DropItems);
        _source = GetComponent<AudioSource>();
        _source.playOnAwake = false;
    }

    private void DropItems()
    {
        Debug.Log("items has ben dropped");  
        _itemDropper.DropItems();
    }


    void FixedUpdate()
    {
        if (IsAlive)
            Move();
    }

    private void Move()
    {
        float distanceToTarget = Vector2.Distance(_target.position, transform.position);
        if (distanceToTarget < _patrolRange.x / 2 || _following)
        {
            _following = true;
            if (!_attacking)
            {
                Follow();
                var distance = Vector2.Distance(_target.position, _attackPoint.position);
                if (distance < _attackRange)
                {
                    _source.clip = attack;
                    _source.loop = false;
                    _source.Play();
                    Attack();
                }
            }
        }
        else
        {
            if (!_stay)
            {
                Patrol();
            }
            else
            {
                _rigidbody.velocity = Vector2.zero;
            }
        }

        var localScale = transform.localScale;
        if (((_rigidbody.velocity.x < 0 && localScale.x > 0) || (_rigidbody.velocity.x > 0 && localScale.x < 0))
            && Mathf.Abs(_rigidbody.velocity.x) > 0.001f)
        {
            transform.localScale = new Vector3(localScale.x * -1, localScale.y, localScale.z);
        }

        
        _animator.SetFloat("velocity", Math.Abs(_rigidbody.velocity.x / _movespeed));
    }

    private void Patrol()
    {
        bool outOfBounds = transform.position.x > _startPosition.x + _patrolRange.x / 2 ||
                           transform.position.x < _startPosition.x - _patrolRange.x / 2;
        
        if (outOfBounds && !_stay)
        {
            direction *= -1;
            _stay = true;
            StartCoroutine(ResetStayState(_stayCooldown));
        }

        _rigidbody.velocity = new Vector2(_movespeed * direction, _rigidbody.velocity.y);
    }

    
    private void Follow()
    {
        if (transform.position.x > _target.position.x)
            _rigidbody.velocity = new Vector2(-_movespeed, _rigidbody.velocity.y);
        else
            _rigidbody.velocity = new Vector2(_movespeed, _rigidbody.velocity.y);
    }

    private void Attack()
    {
        
        _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y);
        _attacking = true;
        _animator.SetTrigger("Attack");
        StartCoroutine(ResetAttackFlag(_attackCooldown * _abilitySystem.Stats.AttackSpeed));
    }

    private IEnumerator ResetAttackFlag(float attackCooldown)
    {
        yield return new WaitForSeconds(attackCooldown / _abilitySystem.Stats.AttackSpeed);
     
        Collider2D[] targets = Physics2D.OverlapCircleAll(_attackPoint.position, _attackRange);
        foreach (var target in targets)
        {
            if (target.CompareTag("Player"))
            {
                var player = target.GetComponent<Entity>();
                // float damage = entityStats.BaseDamage;
                // if (Random.value < entityStats.CriticalChance)
                //     damage *= entityStats.CriticalMultiply;
                
                player.TakeDamage(_abilitySystem.Stats.BaseDamage);
            }
        }
        
        _attacking = false;
    }

    private IEnumerator ResetStayState(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        _stay = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, _patrolRange);
        Gizmos.DrawWireSphere(_attackPoint.position, _attackRange);
    }
}
