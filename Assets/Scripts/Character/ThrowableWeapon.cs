using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;
using UniversalStatsSystem;

namespace Character
{ 
	public class ThrowableWeapon : MonoBehaviour
	{
		[field : SerializeField] public AttackStats AttackStats { get; set; }
		
		public Vector2 direction;
		protected List<GameObject> hitEnemies = new List<GameObject>();
		public float speed = 10f;
		public bool hasHit = false;
		public bool isPlayerWeapon = true;
		public bool isPenetratingShot = false;
        public AttackStats bonusDamage = new(0);
        public Transform owner;
		void FixedUpdate()
		{
			if (!hasHit)
				GetComponent<Rigidbody2D>().velocity = direction * speed;
		}

		private IEnumerator RemoveProjectile()
		{
			yield return new WaitForSeconds(3f);
			if (gameObject) Destroy(gameObject);
		}

		protected virtual void Start()
        {
			if (direction.x < 0)
			{
				transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
			}
			StartCoroutine(RemoveProjectile());
		}

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (isPlayerWeapon)
            {
                if (collision.CompareTag("Enemy"))
                {
                    if (!hitEnemies.Contains(collision.gameObject))
                    {
                        collision.GetComponent<Enemy>().TakeDamage(AttackStats + bonusDamage);
                        hitEnemies.Add(collision.gameObject);
                    }

                    if (!isPenetratingShot)
                    {
                        Destroy(gameObject); // Destroy the projectile if it's not a penetrating shot
                    }
                }
                else if (!collision.CompareTag("Player"))
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                if (collision.CompareTag("Player"))
                {
                    collision.GetComponent<Player>().TakeDamage(new AttackStats(2), transform.position);

                    if (!isPenetratingShot)
                    {
                        Destroy(gameObject);
                    }
                }
                else if (collision.transform != owner)
                {
                    Destroy(gameObject);
                }
            }
        }
	}
}
