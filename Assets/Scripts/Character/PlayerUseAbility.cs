using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalStatsSystem;

namespace Character
{
    public class PlayerUseAbility : MonoBehaviour
    {
        [SerializeField] private GameObject treeRoots;
        private Coroutine _sneakTimeout;
        public static PlayerUseAbility Instance { private set; get; }
        private Player player;
        // Start is called before the first frame update
        void Start()
        {
            player = Player.Instance;
            Instance = this;
        }

        IEnumerator SneakEnd(float spell_duration)
        {
            yield return new WaitForSeconds(spell_duration);
            if (player.IsHidden) ToggleSneak(false);
        }

        public void ToggleSneak(bool isActivate)
        {
            float spell_price = 10f, spell_duration = 20f;

            if (isActivate && !player.StatsSystem.SetMana(-spell_price)) return;

            SpriteRenderer sr = gameObject.GetComponentInChildren<SpriteRenderer>();
            Color tempColor = sr.color;
            tempColor.a = isActivate ? 0.5f : 1f;
            sr.color = tempColor;

            player.IsHidden = isActivate;
            player.StatsSystem.isInvincible = isActivate;
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), player.IsHidden);

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

        private GameObject FindClosestEnemy()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            GameObject closest = null;
            float closestDistance = float.MaxValue;

            foreach (GameObject enemy in enemies)
            {
                float distance = Vector3.Distance(player.transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = enemy;
                }
            }

            return closest;
        }

        private GameObject CreateRoots(Enemy target)
        {
            GameObject roots = GameObject.Instantiate(treeRoots,
            target.transform.position - new Vector3(0, 1f, 0),
            Quaternion.identity) as GameObject;
            roots.transform.SetParent(target.transform);
            return roots;
        }

        IEnumerator TreesEnd(Enemy target, GameObject roots, float spell_duration)
        {
            yield return new WaitForSeconds(spell_duration);
            GameObject.Destroy(roots);
            target.SetFrozenStatus(false);
        }

        private void TreesTaking()
        {
            float spell_price = 15f, spell_duration = 8f;
         
            GameObject _near_enemy = FindClosestEnemy();
            if (!_near_enemy) return;
            Enemy near_enemy = _near_enemy.GetComponent<Enemy>();
            if (!near_enemy) return;

            if (player.StatsSystem.SetMana(-spell_price))
            {
                near_enemy.TakeDamage(new AttackStats(player.StatsSystem.AttackStats.earthDamage));
                if (near_enemy.Health > 0)
                {
                    near_enemy.SetFrozenStatus(true);
                    StartCoroutine(TreesEnd(near_enemy, CreateRoots(near_enemy), spell_duration));
                }
            }
        }

        public void UseAbility(int spellNum)
        {
            switch (spellNum)
            {
                case 1: // скрытность
                    ToggleSneak(true);
                    break;
                case 2:
                    TreesTaking(); // захват ближайшего противника в корни
                    break;
                default:
                   // Debug.LogError("Spell not found");
                    break;
            }
        }
    }
}