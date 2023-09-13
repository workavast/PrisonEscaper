using UnityEngine;

namespace Character
{

	public class PlayerMovement : CharacterController2D
	{
		[Header("PlayerMovement")] [Space]
		
		private float _horizontalMove = 0f;
		private bool _jump = false;
		private bool _dash = false;

		//bool dashAxis = false;

		protected override void OnAwake()
		{
			base.OnAwake();
			
			OnFallEvent.AddListener(OnFall);
			OnLandEvent.AddListener(OnLanding);
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			
			// horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

			animator.SetFloat("Speed", Mathf.Abs(_horizontalMove));

			// if (Input.GetKeyDown(KeyCode.Z))
			// {
			// 	jump = true;
			// }
			//
			// if (Input.GetKeyDown(KeyCode.C))
			// {
			// 	dash = true;
			// }

			/*if (Input.GetAxisRaw("Dash") == 1 || Input.GetAxisRaw("Dash") == -1) //RT in Unity 2017 = -1, RT in Unity 2019 = 1
			{
				if (dashAxis == false)
				{
					dashAxis = true;
					dash = true;
				}
			}
			else
			{
				dashAxis = false;
			}
			*/

		}

		public void Jump()
		{
			_jump = true;
		}

		public void Dash()
		{
			_dash = true;
		}

		public void Move(float direction)
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
			
			// Move our character
			Move(_horizontalMove * Time.fixedDeltaTime, _jump, _dash);
			_jump = false;
			_dash = false;
		}
	}
}