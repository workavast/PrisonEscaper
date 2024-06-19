using UnityEngine;
using Zenject;

namespace Character
{
	public class PlayerMovement : CharacterController2D
	{
		[Header("PlayerMovement")] 
		
		[Inject] protected KeyboardObserver KeyboardObserver;
		private float _horizontalMove = 0f;
		private bool _jump = false;
		private bool _dash = false;
		
		protected override void OnAwake()
		{
			base.OnAwake();
			
			OnFallEvent.AddListener(OnFall);
			OnLandEvent.AddListener(OnLanding);
		}

		protected virtual void SubOfKeyBoardObserver()
		{
			KeyboardObserver.OnDash += Dash;
			KeyboardObserver.OnJump += Jump;
			KeyboardObserver.OnPlatformDrop += PlatformDrop;
			KeyboardObserver.OnMove += Move;
		}
		
		protected virtual void UnSubOfKeyBoardObserver()
		{
			KeyboardObserver.OnDash -= Dash;
			KeyboardObserver.OnJump -= Jump;
			KeyboardObserver.OnPlatformDrop -= PlatformDrop;
			KeyboardObserver.OnMove -= Move;
		} 
		
		private void Update()
		{
			animator.SetFloat("Speed", Mathf.Abs(_horizontalMove));
		}

		private void Jump() => _jump = true;

		private void PlatformDrop() => IsPlatformDrop = true;

		private void Dash() => _dash = true;

		private void Move(float direction)
		{
			_horizontalMove = direction * StatsSystem.MainStats.WalkSpeed;
		}
		
		public void OnFall()
		{
			animator.SetBool("IsJumping", true);
		}

		public void OnLanding()
		{
			animator.SetBool("IsJumping", false);
		}

		protected override void OnFixedUpdate()
		{
			base.OnFixedUpdate();
			
			if(!animator.GetBool("IsAttacking") && !animator.GetBool("IsDead"))
				Move(_horizontalMove * Time.fixedDeltaTime, _jump, _dash);
			else
			{
				if(Grounded)
					Stop();
			}
			_jump = false;
			_dash = false;
		}
	}
}