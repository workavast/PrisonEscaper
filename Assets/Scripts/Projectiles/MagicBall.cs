using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Character;
using Enemies;
using UnityEngine;
using UnityEngine.VFX;

namespace Projectiles
{
    public class MagicBall : ThrowableProjectile
    {
        private readonly Dictionary<Damage, Color> _colors = new Dictionary<Damage, Color>();
        private readonly Dictionary<Damage, float> _damage = new Dictionary<Damage, float>();
        private AudioSource _audioSource;
        private VisualEffect _effect;
        
        protected override void Awake()
        {
            base.Awake();
            
            _audioSource = GetComponent<AudioSource>();
            InitColor();
        }

        public override void Init(Vector2 direction)
        {
            base.Init(direction);
        }

        public void InitColor()
        {
            _colors.Add(Damage.Fire, new Color(245, 78, 12) / 127);
            _colors.Add(Damage.Water, new Color(17, 5, 242) / 127);
            _colors.Add(Damage.Air, new Color(245, 243, 113) / 127);
            _colors.Add(Damage.Earth, new Color(36, 9, 0) / 127);
            _colors.Add(Damage.Electricity, new Color(41, 216, 255) / 127);
            _colors.Add(Damage.Poison, new Color(19, 227, 11) / 127);

            // var damage = owner.GetComponent<AttackStats>();
        
            _damage.Add(Damage.Fire, AttackStats.fireDamage);
            _damage.Add(Damage.Water, AttackStats.waterDamage);
            _damage.Add(Damage.Air, AttackStats.airDamage);
            _damage.Add(Damage.Earth, AttackStats.earthDamage);
            _damage.Add(Damage.Electricity, AttackStats.electricityDamage);
            _damage.Add(Damage.Poison, AttackStats.poisonDamage);


            var top_values = _damage.OrderByDescending(x => x.Value).Take(3).Reverse();

            var gradient_colors = new GradientColorKey[3];
            var gradient_alpha = new GradientAlphaKey[3];


            int i = 0;

            var sum = top_values.Sum(x => x.Value);
            var position = 0f;

            foreach (var entry in top_values)
            {
                position += entry.Value / sum;
                gradient_colors[i] = new GradientColorKey(_colors[entry.Key], position);
                gradient_alpha[i] = new GradientAlphaKey(1f, position);
                i++;
            }

            _effect = GetComponent<VisualEffect>();
            Gradient gradient = new Gradient();

            gradient.SetKeys(gradient_colors, gradient_alpha);

            _effect.SetGradient("New Gradient", gradient);
        }


        void OnTriggerEnter2D(Collider2D collision)
        {
            if (isPlayerWeapon)
            {
                if (collision.CompareTag("Enemy"))
                {
                    if (!HitEnemies.Contains(collision.gameObject))
                    {
                        collision.GetComponent<Enemy>().TakeDamage(AttackStats + bonusDamage);
                        HitEnemies.Add(collision.gameObject);
                    }

                    if (!isPenetratingShot)
                    {
                        StartCoroutine(DestroyAfterExplosion());

                    }
                }
                else if (!collision.CompareTag("Player"))
                {
                    StartCoroutine(DestroyAfterExplosion());
                }
            }
            else
            {
                if (collision.CompareTag("Player"))
                {
                    collision.GetComponent<Player>().TakeDamage(AttackStats, transform.position);

                    if (!isPenetratingShot)
                    {
                        StartCoroutine(DestroyAfterExplosion());
                    }
                }
                else if (collision.transform != owner)
                {
                    StartCoroutine(DestroyAfterExplosion());
                }
            }
        }
    
        private IEnumerator DestroyAfterExplosion()
        {
            _effect.SendEvent("Explode");
            _audioSource.Play();
            yield return new WaitForSeconds(.5f);
            ReturnInPool();
        }
    
        private enum Damage
        {
            Fire,
            Water,
            Air,
            Earth,
            Electricity,
            Poison
        }
    }
}