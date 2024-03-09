using System;
using System.Collections;
using Character;
using UnityEngine;
using UniversalStatsSystem;
using Random = UnityEngine.Random;

namespace Enemies
{
    [RequireComponent(typeof(ItemDropper))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Enemy : EnemyBase
    {
        [SerializeField] private EnemyID enemyID;
        public override event Action<EnemyBase> OnEndDie;
        public override EnemyID EnemyID => enemyID;
        
        //TODO fix attack mechanic because too hard to kill without take damage 
        
        [Header("Enemy")]
        [Space]
        [Header("Moving")]
        [SerializeField] private Vector2 patrolRange;
        [SerializeField] private float stayCooldown;
        [Header("Attack")]
        
        [SerializeField] private float attackRange;
        [SerializeField] protected float attackCooldown;
        [SerializeField] protected Transform attackPoint;
        [SerializeField] protected Transform target;
        [SerializeField]  private float minRangeDistAttack;
        [SerializeField] protected GameObject throwableObject;
        private enum AttackType { Melee, Ranged }
        [SerializeField] private AttackType attackType;
    
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
        [SerializeField] protected Animator animator;
    
        private Vector3 _startPosition;
        private ItemDropper _itemDropper;
        private bool _isAlive, _canMove;    
        private Rigidbody2D _rigidbody;
        private bool _stay;
        protected bool _following, _attacking;
        private float _direction = 1f;
        private bool _frozen = false;
        private float _agrTimeout = 0;
        protected bool IsDead { get => !CheckLifeState(); }
    
        protected override void OnAwake()
        {
            base.OnAwake();
            
            _rigidbody = GetComponent<Rigidbody2D>();
            _itemDropper = GetComponent<ItemDropper>();
            // DeathEvent.AddListener(DropItems);
            _isAlive = _canMove = true;
            
            StatsSystem.Init();
            
            StatusEffectSystem.OnFrozenStatusStart += FrozenStatus;
            StatusEffectSystem.OnFrozenStatusEnd += UnFrozenStatus;
    
            _direction = Random.value >= 0.5f ? 1 : -1;
        }
    
        protected override void OnStart()
        {
            base.OnStart();
    
            _startPosition = transform.position;
    
            if (target == null)
                target = Player.Instance.CharacterCenter;
        }
        
        public override void HandleUpdate(float deltaTime)
        {
            
        }
        
        public override void HandleFixedUpdate(float fixedDeltaTime)
        {
            if (_isAlive && _canMove && !_frozen)
                Move();
        }
        
        private void FrozenStatus()
        {
            _frozen = true;
            _rigidbody.velocity = Vector2.zero;
            animator.SetFloat("velocity", 0);
        }
        private void UnFrozenStatus()
        {
            _frozen = false;
        }
    
        public void SetFrozenStatus(bool is_activate)
        {
            if (is_activate)
            {
                FrozenStatus();
            }
            else UnFrozenStatus();
        }
    
    
        public void AddFireStatus(float chance, float duration, AttackStats baseDamage)
        {
            AttackStats fireEffect = baseDamage;
            fireEffect.statusEffects = new StatusEffects(chance, duration, 0, 0, 0, 0, 0, 0);
    
            StatusEffectSystem.AddStatusEffects(fireEffect);
        }
        
        public virtual void ThrowWeapon()
        {
            if (IsDead) return;
            GameObject throwableWeapon = GameObject.Instantiate(throwableObject,
                transform.position + new Vector3(transform.localScale.x * 0.5f, 0),
                Quaternion.identity) as GameObject;
            
            Vector2 direction = new Vector2(transform.localScale.x, 0);
            ThrowableWeapon weaponObj = throwableWeapon.GetComponent<ThrowableWeapon>();
            weaponObj.AttackStats = StatsSystem.AttackStats;
            weaponObj.speed = 14;
            weaponObj.direction = direction;
            weaponObj.isPlayerWeapon = false;
            weaponObj.owner = transform;
            throwableWeapon.name = "ThrowableWeapon";
        }
    
        IEnumerator DistanceAttack(float minDelay, float maxDelay)
        {
            _attacking = true;
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
            if (!target || !transform || IsDead) yield break;
            float directionToTarget = target.position.x - transform.position.x;
            if (Mathf.Sign(directionToTarget) != Mathf.Sign(transform.localScale.x))
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
    
            Attack();
            ThrowWeapon();
        }
    
        private bool ComparePosition()
        {
            float directionToTarget = target.position.x - transform.position.x;
            if (Mathf.Sign(directionToTarget) == Mathf.Sign(transform.localScale.x)) return true;
            return false;
        }
        
        private bool CheckFloorDiff()
        {
            const float allowedDifference = 0.5f;
            float errorMargin = Player.Instance.Grounded? 2f:6f;
            float difference = Mathf.Abs(target.position.y - transform.position.y);
            bool res = difference >= (allowedDifference - errorMargin) && difference <= (allowedDifference + errorMargin);
            if (!res)
            {
                _agrTimeout = Time.time;
            }
            return res;
        }
        
        private bool TimeoutCheck()
        {
            if (_agrTimeout + 1f < Time.time)
            { 
                return true; 
            }
            return false;
        }
        
        protected virtual void Move()
        {
            if (target == null ||
                IsDead ||
                transform == null)
                return;
    
            float distanceToTargetX = Mathf.Abs(target.position.x - transform.position.x);
            float distanceToTargetY = Mathf.Abs(target.position.y - transform.position.y);
            if (CheckFloorDiff() && distanceToTargetX < patrolRange.x && distanceToTargetY < patrolRange.y && TimeoutCheck() && !Player.Instance.IsHidden)
            {
                if (!_attacking)
                {
                    switch (attackType)
                    {
                        case AttackType.Melee:
                            if ((!WallFrontCheck() || !ComparePosition()) && GroundCheck())
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
                                Attack();
                            }
                            break;
                        case AttackType.Ranged:
                            if (distanceToTargetX < minRangeDistAttack && (!WallFrontCheck() || ComparePosition()) && GroundCheck() && Random.value>=0.005f)
                            {
                                Retreat();
                            }
                            else
                            {
                                StartCoroutine(DistanceAttack(0.2f, 1.5f));
                            }
                            break;
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
    
        protected virtual void Patrol()
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
    
        private void MoveProto(int direction)
        {
            if (IsDead) return;
            if (transform.position.x > target.position.x)
                _rigidbody.velocity = new Vector2(-StatsSystem.MainStats.WalkSpeed * direction, _rigidbody.velocity.y);
            else
                _rigidbody.velocity = new Vector2(StatsSystem.MainStats.WalkSpeed * direction, _rigidbody.velocity.y);
        }
    
        private void Retreat()
        {
            MoveProto(-1);
        }
    
        protected virtual void Follow()
        {
            MoveProto(1);
        }
        
        protected virtual void Attack()
        {
            if (IsDead) return;
            _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y);
            _attacking = true;
            animator.SetTrigger("Attack");
            StartCoroutine(ResetAttackFlag(attackCooldown / StatsSystem.AttackStats.attackCooldown));
        }
    
        protected virtual void TakeDamageCont()
        {
            animator.SetTrigger("Hurt");
            StartCoroutine(Stun());
    
            if (IsDead)
            {
                StartCoroutine(Die());
            }
        }
        
        private bool CheckLifeState()
        {
            return StatsSystem.MainStats.Health.CurrentValue > 0;
        }
    
        public override void TakeDamage(AttackStats attackStats)
        {
            if (IsDead) return;
            base.TakeDamage(attackStats);
            StatsSystem.TakeDamage(attackStats);
    
            TakeDamageCont();
        }
    
        public void TakeDamage(float damageValue, StatsSystem.DamageType damageType)
        {
            if (IsDead) return;
            StatsSystem.TakeDamage(damageValue, damageType);
            TakeDamageCont();
        }
    
        public void TakeDamage(float damageValue) // ���� � ����� ��������
        {
            if (IsDead) return;
            StatsSystem.TakeDamage(damageValue);
            TakeDamageCont();
        }
    
        private IEnumerator Stun()
        {
            _canMove = false;
            yield return new WaitForSeconds(.2f);
            _canMove = true;
        }
    
        /*  private IEnumerator Die()
          {
              animator.SetTrigger("Dead");
              yield return new WaitForSeconds(.4f);
              Destroy(gameObject);
              _itemDropper.DropItems();
          }*/
    
        protected virtual IEnumerator Die()
        {
            _rigidbody.velocity = Vector2.zero;
            animator.SetTrigger("Dead");
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            float animOffset = 0f;
            while (!stateInfo.IsName("Dead"))
            {
                yield return new WaitForSeconds(0.01f);
                stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                animOffset += 0.01f;
            }
            float waitTime = stateInfo.length - (0.1f + animOffset);
            yield return new WaitForSeconds(waitTime);
            _itemDropper.DropItems();
            OnEndDie?.Invoke(this);
            Destroy(gameObject);
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
}
