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

		void FixedUpdate()
		{
			if (!hasHit)
				GetComponent<Rigidbody2D>().velocity = direction * speed;
		}

		void OnCollisionEnter2D(Collision2D collision)
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
	}
}
