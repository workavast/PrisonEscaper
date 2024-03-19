using System.Collections;
using System.Collections.Generic;
using Character;
using Enemies;
using UnityEngine;
using UniversalStatsSystem;

namespace Projectiles
{ 
	public class ThrowableProjectile : ProjectileBase
	{
		[field : SerializeField] public AttackStats AttackStats { get; set; }
		
		public AttackStats bonusDamage = new(0);
		public bool isPenetratingShot = false;
		public bool isPlayerWeapon = true;
		public bool hasHit = false;
		public float speed = 10f;
		public Transform owner;
        
		protected List<GameObject> HitEnemies = new();
		
		private Rigidbody2D _rigidbody2D;
		private Vector2 _direction;

		protected virtual void Awake()
		{
			_rigidbody2D = GetComponent<Rigidbody2D>();
		}

		public override void HandleFixedUpdate(float fixedDeltaTime)
		{
			if (!hasHit)
				_rigidbody2D.velocity = _direction * speed;
		}
        
		private IEnumerator RemoveProjectile()
		{
			yield return new WaitForSeconds(3f);
			if (gameObject) 
				ReturnInPool();
		}

		public virtual void Init(Vector2 direction)
		{
			_direction = direction;
			if (_direction.x < 0)
				transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

			StartCoroutine(RemoveProjectile());
		}

		public override void OnElementReturnInPool()
		{
			base.OnElementReturnInPool();
			HitEnemies = new List<GameObject>();
			hasHit = false;
		}

		private void OnTriggerEnter2D(Collider2D collision)
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
						ReturnInPool();
				}
				else if (!collision.CompareTag("Player"))
				{
					ReturnInPool();
				}
			}
			else
			{
				if (collision.CompareTag("Player"))
				{
					collision.GetComponent<Player>().TakeDamage(new AttackStats(2), transform.position);

					if (!isPenetratingShot)
						ReturnInPool();
				}
				else if (collision.transform != owner)
				{
					ReturnInPool();
				}
			}
		}
	}
}
