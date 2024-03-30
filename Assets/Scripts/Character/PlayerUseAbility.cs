using System.Collections;
using Enemies;
using Projectiles;
using UnityEngine;
using UniversalStatsSystem;

namespace Character
{
    public class PlayerUseAbility : MonoBehaviour
    {
        [SerializeField] private GameObject treeRoots;
        [SerializeField] private GameObject fireParticle;
        
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

            if (isActivate && !_player.StatsSystem.SetMana(-spell_price)) return;

            SpriteRenderer sr = gameObject.GetComponentInChildren<SpriteRenderer>();
            Color tempColor = sr.color;
            tempColor.a = isActivate ? 0.5f : 1f;
            sr.color = tempColor;

            _player.IsHidden = isActivate;
            _player.StatsSystem.isInvincible = isActivate;
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), _player.IsHidden);

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

        private Enemy FindClosestEnemy()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            GameObject closest = null;
            float closestDistance = float.MaxValue;

            foreach (GameObject enemy in enemies)
            {
                float distance = Vector3.Distance(_player.transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = enemy;
                }
            }
            Enemy near_enemy = closest.GetComponent<Enemy>();
            return near_enemy;
        }

        private GameObject CreateRoots(Enemy target)
        {
            GameObject roots = GameObject.Instantiate(treeRoots,
            target.transform.position - new Vector3(0, 1f, 0),
            Quaternion.identity) as GameObject;
            roots.transform.SetParent(target.transform);
            return roots;
        }

        IEnumerator ObjectLifeEnd(GameObject removedObject, float waitingTime)
        {
            yield return new WaitForSeconds(waitingTime);
            GameObject.Destroy(removedObject);
        }

        IEnumerator TreesEnd(Enemy target, GameObject roots, float spell_duration)
        {
            yield return new WaitForSeconds(spell_duration);
            StartCoroutine(ObjectLifeEnd(roots, 0f));
            target.SetFrozenStatus(false);
        }

        private void TreesTaking()
        {
            float spell_price = 15f, spell_duration = 8f, 
                  spell_damage = _player.StatsSystem.AttackStats.earthDamage * 0.1f + 2f;

            Enemy near_enemy = FindClosestEnemy();
            if (!near_enemy) return;

            if (_player.StatsSystem.SetMana(-spell_price))
            {
                near_enemy.TakeDamage(spell_damage, StatsSystem.DamageType.Earth);
                if (near_enemy.Health > 0)
                {
                    near_enemy.SetFrozenStatus(true);
                    StartCoroutine(TreesEnd(near_enemy, CreateRoots(near_enemy), spell_duration));
                }
            }
        }

        private void SpectralArrow()
        {
            float spell_price = 5f;
            if (_player.StatsSystem.SetMana(-spell_price))
            {
                ThrowableProjectile arrow = _player.ThrowProjectile();
                arrow.isPenetratingShot = true;
                arrow.bonusDamage += (_player.StatsSystem.AttackStats * 0.4f);
                SpriteRenderer spriteRenderer = arrow.GetComponent<SpriteRenderer>();
                arrow.transform.Find("particle").gameObject.SetActive(true);
            }
        }

        private void MakeFire(Enemy target, float spell_duration)
        {
            GameObject fire = GameObject.Instantiate(fireParticle,
            target.transform.position + new Vector3(0f, 0f, -5f),
            Quaternion.identity) as GameObject;
            fire.transform.SetParent(target.transform);
            fire.transform.localScale.Set(1f, 1f, 1f);
            StartCoroutine(ObjectLifeEnd(fire, spell_duration));
        }

        private void BurnEnemy()
        {
            float spell_price = 15f, spell_duration = 10f;
            Enemy near_enemy = FindClosestEnemy();
            if (!near_enemy) return;

            if (_player.StatsSystem.SetMana(-spell_price))
            {
                near_enemy.AddFireStatus(1f, spell_duration, _player.StatsSystem.AttackStats * 0.5f);
                MakeFire(near_enemy, spell_duration);
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