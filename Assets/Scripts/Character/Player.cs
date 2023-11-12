using System.Collections.Generic;
using PlayerInventory;
using UniversalStatsSystem;
using UnityEngine;
using System.Collections;

namespace Character
{
    public sealed class Player : PlayerMovement
    {
        [field: Header("Player")] 
        [field: Space]
        [field: SerializeField] public Transform CharacterCenter { get; private set; }
        [SerializeField] private PlayerAttack playerAttack;
        
        public Inventory Inventory;
        public bool AttackIsPossible { get => CanAttack && playerAttack.canAttack; }
        public bool IsAlive { private set; get; }
        public bool IsHidden { set; get; }

        public static Player Instance { private set; get; }
        private readonly List<IInteractive> _interactiveObjects = new List<IInteractive>();

        protected override void OnAwake()
        {
            base.OnAwake();
            
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;

            DontDestroyOnLoad(gameObject);
            
            Inventory.Init();
            StatsSystem.Init();
            playerAttack.Init();

            IsAlive = true;
            StatsSystem.OnDeath.AddListener(Die);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            CheckNearestInteractiveObject();
        }
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            CheckInteractiveObjectEnter(col);
        }
        
        private void OnTriggerExit2D(Collider2D col)
        {
            CheckInteractiveObjectExit(col);
        }
        
        public void Attack()
        {
            if (CanAttack)
            {
                playerAttack.Attack();
            }
        }


        public void ToggleSneak(bool isActivate)
        {
            PlayerUseAbility.Instance.ToggleSneak(isActivate);
        }

        public void UseAbility(int spellNum)
        {
            PlayerUseAbility.Instance.UseAbility(spellNum);
        }

        public ThrowableWeapon ThrowWeapon()
        {
            return playerAttack.ThrowWeapon();
        }
        
        public void Heal(float value)
        {
            StatsSystem.Heal(value);
        }
        
        public void TakeDamage(AttackStats attackStats, Vector3 position)
        {
            if(Invincible) return;
            
            TakeDamage(attackStats);
            TakeDamage(position);
        }

        public override void TakeDamage(AttackStats attackStats)
        {
            base.TakeDamage(attackStats);

            StatsSystem.TakeDamage(attackStats);
        }
        
        private void Die()
        {
            IsAlive = false;

            StartCoroutine(WaitToDead());
        }
        
        public void ApplyItemStats(UniversalStatsSystem.Stats stats, AttackStats attackStats, ResistStats resistStats)
        {
            StatsSystem.ApplyStats(stats,attackStats,resistStats);

            StatsSystem.OnStatsChanged.Invoke();
        }

        public void RemoveItemStats(UniversalStatsSystem.Stats stats, AttackStats attackStats, ResistStats resistStats)
        {
            StatsSystem.RemoveStats(stats,attackStats,resistStats);

            StatsSystem.OnStatsChanged.Invoke();
        }
        
        #region InteractiveWithObjects
        private void CheckInteractiveObjectEnter(Collider2D col)
        {
            if (col.TryGetComponent(out IInteractive loot))
            {
                _interactiveObjects.Add(loot);
            }
        }

        private void CheckInteractiveObjectExit(Collider2D col)
        {
            if (col.TryGetComponent(out IInteractive loot))
            {
                if(_interactiveObjects.Contains(loot))
                    _interactiveObjects.Remove(loot);
            }
        }
        
        private void CheckNearestInteractiveObject()
        {
            if (!Input.GetKeyDown(KeyCode.F) || _interactiveObjects.Count <= 0) return;
            
            IInteractive nearestInteractiveObj = null;
            foreach (var interactiveObj in _interactiveObjects)
            {
                if (interactiveObj.Interactable)
                {
                    nearestInteractiveObj = interactiveObj;
                    break;
                }
            }
            if(nearestInteractiveObj is null) return;
            
            float minDistance = Vector2.Distance(transform.position, nearestInteractiveObj.transform.position);
            foreach (var loot in _interactiveObjects)
            {
                if (loot.Interactable && minDistance > Vector2.Distance(transform.position, loot.transform.position))
                {
                    nearestInteractiveObj = loot;
                    minDistance = Vector2.Distance(transform.position, loot.transform.position);
                }
            }

            nearestInteractiveObj.Interact();
        }
        #endregion
    }
}