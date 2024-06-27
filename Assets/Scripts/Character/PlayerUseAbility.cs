using System.Collections;
using Enemies;
using GameCode;
using Projectiles;
using UnityEngine;
using UniversalStatsSystem;

namespace Character
{
    public class PlayerUseAbility : MonoBehaviour
    {
        [SerializeField] private GameObject treeRoots;
        [SerializeField] private GameObject smoke;
        
        public static PlayerUseAbility Instance { private set; get; }
        
        private Coroutine _sneakTimeout;
        private Player _player;

        private void Awake()
        {
            _player = GetComponent<Player>();
            Instance = this;
        }

        IEnumerator SneakEnd(float spell_duration)
        {
            yield return new WaitForSeconds(spell_duration);
            if (_player.IsHidden) ToggleSneak(false);
        }

        public void ToggleSneak(bool isActivate)
        {
            float spell_price = 10f, spell_duration = 20f;

            if (isActivate && !_player.StatsSystem.ChangeMana(-spell_price)) return;

            SpriteRenderer sr = gameObject.GetComponentInChildren<SpriteRenderer>();
            Color tempColor = sr.color;
            tempColor.a = isActivate ? 0.5f : 1f;
            sr.color = tempColor;

            _player.IsHidden = isActivate;
            _player.StatsSystem.isInvincible = isActivate;
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), _player.IsHidden);
            
            if(isActivate)
                Instantiate(smoke, _player.transform.position + Vector3.up * 1.25f, Quaternion.identity);
                
            if (_sneakTimeout != null)
            {
                StopCoroutine(_sneakTimeout);
                _sneakTimeout = null;
            }

            if (isActivate)
            {
                _sneakTimeout = StartCoroutine(SneakEnd(spell_duration));
            }
        }

        private bool FindClosestEnemy(out Enemy closetEnemy)
        {
            closetEnemy = null;
            
            var hits = Physics2D.CircleCastAll(transform.position, 15, Vector2.right, 0f);
            float closestDistance = float.MaxValue;
            foreach (var hit in hits)
            {
                if (hit.collider.TryGetComponent(out Enemy enemy))
                {
                    float distance = Vector3.Distance(_player.transform.position, enemy.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closetEnemy = enemy;
                    }
                }
            }

            return closetEnemy != null;
        }

        private GameObject CreateRoots(Enemy target)
        {
            GameObject roots = GameObject.Instantiate(treeRoots,
            target.transform.position - new Vector3(0, 1f, 0),
            Quaternion.identity);
            roots.transform.SetParent(target.transform);
            return roots;
        }

        IEnumerator ObjectLifeEnd(GameObject removedObject, float waitingTime)
        {
            yield return new WaitForSeconds(waitingTime);
            Destroy(removedObject);
        }

        IEnumerator TreesEnd(Enemy target, GameObject roots, float spell_duration)
        {
            yield return new WaitForSeconds(spell_duration);
            StartCoroutine(ObjectLifeEnd(roots, 0f));
            target.SetFrozenStatus(false);
        }

        private void TreesTaking()
        {
            if (!FindClosestEnemy(out var nearEnemy)) 
                return;

            const float spellPrice = 15f, spellDuration = 8f; 
            float spellDamage = _player.StatsSystem.AttackStats.earthDamage * 0.1f + 2f;
            if (_player.StatsSystem.ChangeMana(-spellPrice))
            {
                nearEnemy.TakeDamage(spellDamage, StatsSystem.DamageType.Earth);
                if (nearEnemy.Health > 0)
                {
                    nearEnemy.SetFrozenStatus(true);
                    StartCoroutine(TreesEnd(nearEnemy, CreateRoots(nearEnemy), spellDuration));
                }
            }
        }

        private void SpectralArrow()
        {
            const float spellPrice = 5f;
            if (_player.StatsSystem.ChangeMana(-spellPrice))
            {
                ThrowableProjectile arrow = _player.ThrowProjectile();
                arrow.isPenetratingShot = true;
                arrow.bonusDamage += (_player.StatsSystem.AttackStats * 0.4f);
                arrow.GetComponentInChildren<ParticleEffector>(true)?.Show();
            }
        }
        
        private void BurnEnemy()
        {
            if (!FindClosestEnemy(out var nearEnemy)) 
                return;
            
            const float spellPrice = 15f, spellDuration = 10f;
            if (_player.StatsSystem.ChangeMana(-spellPrice))
            {
                var fireEffect = new AttackStats(40);
                fireEffect += _player.StatsSystem.AttackStats * 0.5f;
                fireEffect.statusEffects = new StatusEffects(1f, spellDuration, 0, 0, 0, 0, 0, 0);
                
                nearEnemy.AddStatusEffect(fireEffect);
            }
        }

        public void UseAbility(int spellNum)
        {
            switch (spellNum)
            {
                case 1: // ����������
                    ToggleSneak(true);
                    break;
                case 2:
                    TreesTaking(); // ������ ���������� ���������� � �����
                    break;

                case 3:
                    SpectralArrow(); // ���������� ������
                    break;

                case 4:
                    BurnEnemy(); // ������� ���������� �����
                    break;
                default:
                   // Debug.LogError("Spell not found");
                    break;
            }
        }
    }
}