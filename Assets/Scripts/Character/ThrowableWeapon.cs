using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalStatsSystem;

namespace Character
{ 
	public class ThrowableWeapon : MonoBehaviour
	{
		[field : SerializeField] public AttackStats AttackStats { get; private set; }
		
		public Vector2 direction;
		public bool hasHit = false;
		public float speed = 10f;
		public bool isPlayerWeapon = true;

		public Transform owner;
		void FixedUpdate()
		{
			if (!hasHit)
				GetComponent<Rigidbody2D>().velocity = direction * speed;
		}

		private IEnumerator RemoveProjectile()
		{
			yield return new WaitForSeconds(5f);
			if (gameObject) Destroy(gameObject);
		}

		private void Start()
        {
			if (direction.x < 0)
			{
				transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
			}
			StartCoroutine(RemoveProjectile());
		}

        void OnCollisionEnter2D(Collision2D collision)
		{
			if (isPlayerWeapon) 
			{
				if (collision.gameObject.tag == "Enemy")
				{
					collision.gameObject.GetComponent<Enemy>().TakeDamage(AttackStats);
					Destroy(gameObject);
				}
				else if (collision.gameObject.tag != "Player")
				{
					Destroy(gameObject);
				}
			}
			else
			{
				if (collision.gameObject.tag == "Player")
				{
					collision.gameObject.GetComponent<Player>().TakeDamage(new AttackStats(2), transform.position);
					Destroy(gameObject);
				}
				else if (collision.transform != owner)
				{
					Destroy(gameObject);
				}
			}
		}
	}
}
