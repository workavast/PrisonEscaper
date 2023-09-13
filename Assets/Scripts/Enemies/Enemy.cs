using System;
using System.Collections;
using Character;
using UnityEngine;
using UniversalStatsSystem;
using Random = UnityEngine.Random;

[RequireComponent(typeof(ItemDropper))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class Enemy : CharacterBase
{
    //TODO fix attack mechanic because too hard to kill without take damage 
    
    [Header("Enemy")]
    [Space]
    [Header("Moving")]
    [SerializeField] private Vector2 patrolRange;
    [SerializeField] private float stayCooldown;
    [Header("Attack")]
    
    [SerializeField] private float attackRange;
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Transform target;
    
    [Header("Orientation checks")]
    [SerializeField] private Transform wallFrontCheckPoint;
    [SerializeField] private LayerMask wallFrontCheckLayers;
    [Space]
    [SerializeField] private Transform wallBackCheckPoint;
    [SerializeField] private LayerMask wallBackCheckLayers;
    [Space]
    [SerializeField] private float wallCheckHeight;
    [SerializeField] private float wallCheckWidth;
    [Space]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundCheckLayers;

    [Header("Other")]
    [SerializeField] private Animator animator;
    [SerializeField] private AudioClip walk, attack;
    
    private AudioSource _source;
    private Vector3 _startPosition;
    private ItemDropper _itemDropper;
    private bool _isAlive, _canMove;    
    private Rigidbody2D _rigidbody;
    private bool _stay;
    private bool _following, _attacking;
    private float _direction = 1f;
    private bool _frozen = false;

    protected override void OnAwake()
    {
        base.OnAwake();
        
        _rigidbody = GetComponent<Rigidbody2D>();
        _itemDropper = GetComponent<ItemDropper>();
        // DeathEvent.AddListener(DropItems);
        _isAlive = _canMove = true;
        _source = GetComponent<AudioSource>();
        _source.playOnAwake = false;
        
        StatsSystem.Init();
        
        StatusEffectSystem.OnFrozenStatusStart += FrozenStatus;
        StatusEffectSystem.OnFrozenStatusEnd += UnFrozenStatus;

        _direction = Random.value >= 0.5f ? 1 : -1;
    }

    protected override void OnStart()
    {
        base.OnStart();

        _startPosition = transform.position;
        
        if(target == null)
            target = Player.Instance.CharacterCenter;
    }
    
    private void FrozenStatus()
    {
        _frozen = true;
        animator.SetFloat("velocity", 0);
    }
    private void UnFrozenStatus()
    {
        _frozen = false;
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        
        if (_isAlive && _canMove && !_frozen)
            Move();
    }

    private void Move()
    {
        if(target == null) return;
        
        float distanceToTargetX = Mathf.Abs(target.position.x - transform.position.x);
        float distanceToTargetY = Mathf.Abs(target.position.y - transform.position.y);
        if (distanceToTargetX < patrolRange.x && distanceToTargetY < patrolRange.y)
        {
            if (!_attacking)
            {
                if (!WallFrontCheck() && GroundCheck())
                {
                    Follow();
                }
                else
                {
                    _rigidbody.velocity = Vector2.zero;
                }
                
                var distance = Vector2.Distance(target.position, attackPoint.position);
                if (distance < attackRange)
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

        
        animator.SetFloat("velocity", Math.Abs(_rigidbody.velocity.x / StatsSystem.MainStats.WalkSpeed));
    }
    private void Patrol()
    {
        bool outOfBounds;
        if (_direction > 0)
            outOfBounds = transform.position.x > _startPosition.x + patrolRange.x / 2;
        else
            outOfBounds = transform.position.x < _startPosition.x - patrolRange.x / 2;
        
        if ((outOfBounds || WallFrontCheck() || !GroundCheck()) && !_stay)
        {
            _direction *= -1;
            _stay = true;
            StartCoroutine(ResetStayState(stayCooldown));
        }

        _rigidbody.velocity = new Vector2(StatsSystem.MainStats.WalkSpeed * _direction, _rigidbody.velocity.y);
    }

    private bool WallFrontCheck()
    {
        return Physics2D.OverlapBox(wallFrontCheckPoint.position, new Vector2(wallCheckWidth, wallCheckHeight),0f,wallFrontCheckLayers);
    }
    
    private bool WallBackCheck()
    {
        return Physics2D.OverlapBox(wallBackCheckPoint.position, new Vector2(wallCheckWidth, wallCheckHeight),0f,wallBackCheckLayers);
    }
    
    private bool GroundCheck()
    {
        return Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundCheckLayers);
    }

    private void Follow()
    {
        if (transform.position.x > target.position.x)
            _rigidbody.velocity = new Vector2(-StatsSystem.MainStats.WalkSpeed, _rigidbody.velocity.y);
        else
            _rigidbody.velocity = new Vector2(StatsSystem.MainStats.WalkSpeed, _rigidbody.velocity.y);
    }

    private void Attack()
    {
        _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y);
        _attacking = true;
        animator.SetTrigger("Attack");
        StartCoroutine(ResetAttackFlag(attackCooldown / StatsSystem.AttackStats.attackCooldown));
    }

    public override void TakeDamage(AttackStats attackStats)
    {
        base.TakeDamage(attackStats);
        
        StatsSystem.TakeDamage(attackStats);
        Debug.Log(StatsSystem.MainStats.Health);
        animator.SetTrigger("Hurt");
        StartCoroutine(Stun());

        if (StatsSystem.MainStats.Health <= 0)
        {
            StartCoroutine(Die());
        }
    }

    private IEnumerator Stun()
    {
        _canMove = false;
        yield return new WaitForSeconds(.2f);
        _canMove = true;
    }

    private IEnumerator Die()
    {
        animator.SetTrigger("Dead");
        yield return new WaitForSeconds(.4f);
        Destroy(gameObject);
        _itemDropper.DropItems();
    }

    private IEnumerator ResetAttackFlag(float attackCooldown)
    {
        yield return new WaitForSeconds(.2f);

        Collider2D[] targets = Physics2D.OverlapCircleAll(attackPoint.position, attackRange);
        foreach (var target in targets)
        {
            if (target.CompareTag("Player"))
            {
                var player = target.GetComponent<Player>();
                player.TakeDamage(StatsSystem.GetDamage(), transform.position);
            }
        }
  
        yield return new WaitForSeconds(attackCooldown);
        
        _attacking = false;
    }

    private IEnumerator ResetStayState(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        _stay = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, patrolRange);
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        if(wallFrontCheckPoint) Gizmos.DrawWireCube(wallFrontCheckPoint.position, new Vector3(wallCheckWidth, wallCheckHeight, 0));
        if(wallBackCheckPoint) Gizmos.DrawWireCube(wallBackCheckPoint.position, new Vector3(wallCheckWidth, wallCheckHeight, 0));
        if(groundCheckPoint) Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        Gizmos.DrawWireSphere(Position, 5f);//electricity status effect radius
    }
}