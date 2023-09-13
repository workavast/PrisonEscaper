using System.Collections;
using System.Collections.Generic;
using PlayerInventory.Scriptable;
using UnityEngine;
using UniversalStatsSystem;

namespace Character
{ 
	[System.Serializable]
	public class PlayerAttack
	{
		public GameObject throwableObject;
		public Transform attackCheck;
		public Animator animator;
		public bool canAttack = true;
		public bool isTimeToCheck = false;
		
		private int _attackVariantCycle = 0;
		const int attacksCount = 4;
		private Player _player;
		
		public void Init()
		{
			_player = Player.Instance;
		}
		
		public void ThrowWeapon()
		{
			Transform playerTransform = _player.transform;
			GameObject throwableWeapon = Player.Instantiate(throwableObject,
				playerTransform.position + new Vector3(playerTransform.localScale.x * 0.5f, playerTransform.localScale.y * 3f),
				Quaternion.identity) as GameObject;
			Vector2 direction = new Vector2(playerTransform.localScale.x, 0);
			throwableWeapon.GetComponent<ThrowableWeapon>().direction = direction;
			throwableWeapon.name = "ThrowableWeapon";
		}
		
		//TODO: make it possible to damage enemies
		public void Attack()
		{
			if (!canAttack || _player.Inventory.SpecialSlots[SlotType.Weapon] is null)
				return;

			AttackStats attackStats = _player.StatsSystem.GetDamage();

			Collider2D[] targets = Physics2D.OverlapCircleAll(attackCheck.position, attackStats.attackRange);
			foreach (var target in targets)
			{
				if (target.transform.CompareTag("Enemy"))
					target.GetComponent<IDamageable>().TakeDamage(attackStats);
			}
			
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