﻿using System.Collections.Generic;
using Core;
using Projectiles;
using UniversalStatsSystem;
using UnityEngine;
using Zenject;

namespace Character
{
    public sealed class Player : PlayerMovement
    {
        [field: Header("Player")] 
        [field: Space]
        [field: SerializeField] public Transform CharacterCenter { get; private set; }
        [SerializeField] private PlayerAttack playerAttack;

        [Inject] private ProjectileFactory _projectileFactory;
        
        public static Player Instance { private set; get; }
        public bool AttackIsPossible { get => CanAttack && playerAttack.canAttack; }
        public bool IsAlive { private set; get; } = true;
        public bool IsHidden { set; get; }

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
            
            StatsSystem.Init();
            playerAttack.Init(this, _projectileFactory);

            StatsSystem.OnDeath.AddListener(Die);

            KeyboardObserver.OnFirstAttack += FirstAttack;
            KeyboardObserver.OnSecondAttack += SecondAttack;
            KeyboardObserver.OnInteract += InteractWithNearestInteractiveObject;
            KeyboardObserver.OnAbilityUse += UseAbility;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            CheckInteractiveObjectEnter(col);
        }
        
        private void OnTriggerExit2D(Collider2D col)
        {
            CheckInteractiveObjectExit(col);
        }

        private void OnDestroy()
        {
            UnSubOfKeyBoardObserver();
        }

        public ThrowableProjectile ThrowProjectile()
        {
            return playerAttack.ThrowProjectile();
        }
        
        public void Heal(float value)
        {
            StatsSystem.Heal(value);
        }

        public void SetAnimIdle()
        {
            animator.SetBool("Hit", true);
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
        
        public void ToggleSneak(bool isActivate)
        {
            PlayerUseAbility.Instance.ToggleSneak(isActivate);
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
        
        private void FirstAttack()
        {
            if (CanAttack)
                playerAttack.Attack();
        }
        
        private void SecondAttack()
        {
            ThrowProjectile();
        } 
        
        private void UseAbility(int spellNum)
        {
            PlayerUseAbility.Instance.UseAbility(spellNum);
        }

        private void Die()
        {
            IsAlive = false;
            UnSubOfKeyBoardObserver();

            StartCoroutine(WaitToDead());
        }

        protected override void UnSubOfKeyBoardObserver()
        {
            base.UnSubOfKeyBoardObserver();
            
            KeyboardObserver.OnFirstAttack -= FirstAttack;
            KeyboardObserver.OnSecondAttack -= SecondAttack;
            KeyboardObserver.OnAbilityUse -= UseAbility;
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
        
        private void InteractWithNearestInteractiveObject()
        {
            if (_interactiveObjects.Count <= 0) return;
            
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