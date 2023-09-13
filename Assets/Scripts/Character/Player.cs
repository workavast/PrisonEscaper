using New_Folder.Healthbar;
using PlayerInventory;
using PlayerInventory.Scriptable;
using UniversalStatsSystem;
using UnityEngine;
using UnityEngine.Rendering;

namespace Character
{
    public class Player : PlayerMovement
    {
        [field: Header("Player")] [field: Space]
        
        [SerializeField] private Transform characterCenter;
        [SerializeField] private PlayerAttack playerAttack;
        public Inventory Inventory;
        
        public Transform CharacterCenter => characterCenter;
        public bool AttackIsPossible { get => CanAttack && playerAttack.canAttack; }
        public bool IsAlive { private set; get; }

        public static Player Instance { private set; get; }

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

        public void Attack()
        {
            if(CanAttack)
                playerAttack.Attack();
        }

        public void ThrowWeapon()
        {
            playerAttack.ThrowWeapon();
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
    }
}