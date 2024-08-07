﻿using System.Collections;
using Core;
using PlayerInventory;
using PlayerInventory.Scriptable;
using Projectiles;
using UnityEngine;
using UniversalStatsSystem;

namespace Character
{ 
	[System.Serializable]
	public class PlayerAttack
	{
		public Transform attackCheck;
		public Animator animator;
		public bool canAttack = true;
		public bool isTimeToCheck = false;

		private int _attackVariantCycle = 0;
		const int attacksCount = 3;
		private Player _player;
		private ProjectileFactory _projectileFactory;
		
		public void Init(Player player, ProjectileFactory projectileFactory)
		{
			_player = player;
			_projectileFactory = projectileFactory;
		}
		
		public ThrowableProjectile ThrowProjectile()
		{
			Transform playerTransform = _player.transform;
			ProjectileBase throwableWeapon = _projectileFactory.Create(ProjectileId.PlayerArrow,
				playerTransform.position +
				new Vector3(playerTransform.localScale.x * 0.5f, playerTransform.localScale.y * 3f));
			
			ThrowableProjectile arrow = throwableWeapon.GetComponent<ThrowableProjectile>();
			Vector2 direction = new Vector2(playerTransform.localScale.x, 0);
			throwableWeapon.name = "ThrowableWeapon";
			arrow.Init(direction);

			if (_player.IsHidden) 
				_player.ToggleSneak(false);
			return arrow;
		}
		
		//TODO: make it possible to damage enemies
		public void Attack()
		{
			if (!canAttack || Inventory.SpecialSlots[SlotType.Weapon] is null)
				return;

			AttackStats attackStats = _player.StatsSystem.GetDamage();

			Collider2D[] targets = Physics2D.OverlapCircleAll(attackCheck.position, attackStats.attackRange);
			foreach (var target in targets)
			{
				if (target.transform.CompareTag("Enemy"))
					target.GetComponent<IDamageable>().TakeDamage(attackStats);
			}

			if (_player.IsHidden) _player.ToggleSneak(false);

			canAttack = false;
			animator.SetBool("IsAttacking", true);
			animator.SetInteger("numAttack", _attackVariantCycle++ % attacksCount);
			_player.StartCoroutine(AttackCooldown(attackStats.attackCooldown));
		}

		IEnumerator AttackCooldown(float attackCooldown)
		{
			yield return new WaitForSeconds(attackCooldown);
			canAttack = true;
		}

		//TODO: make damaged dash later...
		
		// public void DoDashDamage()
		// {
		// 	dmgValue = Mathf.Abs(dmgValue);
		// 	Collider2D[] collidersEnemies = Physics2D.OverlapCircleAll(attackCheck.position, 0.9f);
		// 	for (int i = 0; i < collidersEnemies.Length; i++)
		// 	{
		// 		if (collidersEnemies[i].gameObject.tag == "Enemy")
		// 		{
		// 			if (collidersEnemies[i].transform.position.x - transform.position.x < 0)
		// 			{
		// 				dmgValue = -dmgValue;
		// 			}
		//
		// 			collidersEnemies[i].gameObject.SendMessage("TakeDamage", dmgValue);
		// 			cam.GetComponent<CameraFollow>().ShakeCamera();
		// 		}
		// 	}
		// }
	}
}