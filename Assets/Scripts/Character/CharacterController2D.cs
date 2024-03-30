using System;
using System.Collections;
using MyNamespace;
using UnityEngine;
using UnityEngine.Events;

namespace Character
{
	public class CharacterController2D : CharacterBase
	{
		[Header("CharacterController2D")] [Space]

		const float DoubleJumpScaleModifier = .8f; //For reduce jump force doing second jump

		#region SerializeFields
		[SerializeField] [Range(0, .3f)] private float movementSmoothing = .05f; // How much to smooth out the movement
		[Space]
		[SerializeField] private float gravityForce = -9.8f;
		[SerializeField] [Range(0, 5f)] private float gravityScale = 1f;
		[Space]
		[SerializeField] private float dashForce = 25f;
		[Space]
		[SerializeField] private float jumpForce = 10f; // Amount of force added when the player jumps.
		[SerializeField] private float limitFallSpeed = 10f; // Limit fall speed
		[SerializeField] private bool canDoubleJump = true; //If player can double jump
		[SerializeField] private bool airControl = false; // Whether or not a player can steer while jumping;
		[Space]
		[SerializeField] private LayerMask whatIsGround; // A mask determining what is ground to the character
		[SerializeField] private Transform wallUpperCheck;
		[SerializeField] private Transform wallLowerCheck;
		[SerializeField] private Transform groundLedgeCheck;
		[SerializeField] private float wallCheckWidth;
		[Tooltip("Max height of ledge in percent of height of player collider")] 
		[SerializeField] [Range(0,100)] private float groundLedgeMaxHeight;
		[SerializeField] private Vector2 groundCheckSize;
		[Space]
		[SerializeField] protected Animator animator;
		[Space]
		public ParticleSystem particleJumpUp; //Trail particles
		public ParticleSystem particleJumpDown; //Explosion particles
		#endregion
		
		public bool CanAttack { get; private set; }
		public bool Grounded { get; private set; }

		#region ProtectedFields
		protected bool IsPlatformDrop;
		protected UnityEvent OnFallEvent;
		protected UnityEvent OnLandEvent;
		#endregion
		
		#region PrivateFields
		private Rigidbody2D _rigidbody2D;
		private Vector3 _velocity = Vector3.zero;

		private bool _canMove = true;
		private bool _facingRight = true; // For determining which way the player is currently facing.
		private bool _canDash = true;
		private bool _isDashing;
		private bool _isWallUpper; //If there is a up part of wall in front of the player
		private bool _isWallLower; //If there is a down part of wall in front of the player
		private bool _isDead;
		private bool _isJump;

		private float _colliderHeight;
		private float _groundLedgeMaxHeight;

		private Vector2 _wallUpperCheckSize;
		private Vector2 _wallLowerCheckSize;
		private Vector2 _groundLedgeCheckSize;
		private Vector2 _groundLedgeClimbingHitPoint = new Vector2(0,0);
		private Vector2 _groundLedgeClimbingCheckPosition = new Vector2(0,0);
		
		private Vector2 _platformDescentRayCastLocalStart;
		private Vector2 _platformDecentHitWorldPoint = new Vector2(0,0);
		private Vector2 _platformDecentCheckBoxLocalPosition = new Vector2(0,0);
		#endregion

		protected override void OnAwake()
		{
			base.OnAwake();
			
			if (gameObject.layer == whatIsGround)
				Debug.LogWarning("Attention!: player layer == ground layer");
			
			if (!_rigidbody2D)
				_rigidbody2D = GetComponent<Rigidbody2D>();
			
			if (!animator)
				animator = GetComponent<Animator>();

			if (OnFallEvent == null)
				OnFallEvent = new UnityEvent();

			if (OnLandEvent == null)
				OnLandEvent = new UnityEvent();

			var capsuleCollider2D = GetComponent<CapsuleCollider2D>();
			Vector2 colliderSize = capsuleCollider2D.size;
			Vector2 localScale = transform.localScale;
			
			_groundLedgeCheckSize = _platformFreePlaceCheckBoxSize = colliderSize * localScale;
			_colliderHeight = colliderSize.y * localScale.y;

			_groundLedgeMaxHeight = _colliderHeight * (groundLedgeMaxHeight / 100);
			_wallUpperCheckSize = _wallLowerCheckSize = new Vector2(wallCheckWidth, _colliderHeight / 2.1f);

			_platformRaycastCheckLocalStart = new Vector2(0, _colliderHeight + platformMaxHeight + 0.1f);
			_platformDescentRayCastLocalStart = new Vector2(0, - platformMaxHeight - 0.1f);
		}

		private void FixedUpdate()
			=> OnFixedUpdate();
		
		protected virtual void OnFixedUpdate()
		{
			GroundCheck();
			WallsCheck();

			CanAttack = (Grounded && !_isDashing && !_isDead);
		}

		#region OnFixedUpdates
		private void GroundCheck()
		{
			bool groundContact = Physics2D.BoxCast(transform.position, groundCheckSize, 
				0f, Vector2.zero, 0f, whatIsGround);
			
			if(groundContact && !Grounded){
				OnLandEvent.Invoke();
				if (!_isWallUpper && !_isWallLower && !_isDashing)
					particleJumpDown.Play();
				canDoubleJump = true;
			}
			
			Grounded = groundContact;
			
			if (!Grounded)
			{
				OnFallEvent.Invoke();
				
				_rigidbody2D.velocity += new Vector2(0, gravityForce * gravityScale * Time.fixedDeltaTime);
			}
		}

		private void WallsCheck()
		{
			_isWallUpper = Physics2D.BoxCast(wallUpperCheck.position, _wallUpperCheckSize, 
				0f, Vector2.zero, 0f, whatIsGround);
			
			_isWallLower = Physics2D.BoxCast(wallLowerCheck.position, _wallLowerCheckSize, 
				0f, Vector2.zero, 0f, whatIsGround);
				
			if (_isWallUpper || _isWallLower) _isDashing = false;
		}
		#endregion

		protected void Move(float move, bool jump, bool dash)
		{
			if(move > 0 && _facingRight || move < 0 && ! _facingRight ) GroundLedgeClimbing();
			
			PlatformClimbing();

			if (IsPlatformDrop)
			{
				PlatformDescent();
				IsPlatformDrop = false;
			}

			if (!_canMove 
			    // || animator.GetBool("IsGroundLedgeClimbing") 
			    // || animator.GetBool("IsPlatformClimbing")
			    ) 
				return;
			
			MainMove(move);
			Dash(dash);
			Jump(jump);
		}

		#region Moves
		private void GroundLedgeClimbing()//TODO: need add climbing animation
		{
			if (!_isWallUpper && _isWallLower && Grounded && !animator.GetBool("IsGroundLedgeClimbing"))
			{
				RaycastHit2D raycastHit = Physics2D.Raycast(groundLedgeCheck.position, Vector2.down, 3, whatIsGround);
				_groundLedgeClimbingHitPoint = raycastHit.point;
				if (raycastHit.point != Vector2.zero)
				{
					float ledgeHeight = _colliderHeight - Vector2.Distance(groundLedgeCheck.position, raycastHit.point);
					
					if (ledgeHeight < 0 || ledgeHeight > _groundLedgeMaxHeight) return;

					float distance = _colliderHeight/2 - ledgeHeight;
					_groundLedgeClimbingCheckPosition = new Vector2(groundLedgeCheck.position.x,
						groundLedgeCheck.position.y - distance + 0.05f);

					bool haveFreePlace = !Physics2D.BoxCast(_groundLedgeClimbingCheckPosition, _groundLedgeCheckSize, 
						0f, Vector2.zero, 0f, whatIsGround);
					if (haveFreePlace)
					{
						transform.position = raycastHit.point;
						_rigidbody2D.velocity = Vector2.zero;
						
						animator.SetBool("IsGroundLedgeClimbing", true);
					}
				}
			}
		}
		
		[SerializeField] [Range(0, 2)] private float platformMaxHeight;
		
		private Vector2 _platformRaycastCheckLocalStart;
		private Vector2 _platformFreePlaceCheckBoxSize;
		private Vector2 _platformClimbingHitWorldPoint = new Vector2(0, 0);
		private Vector2 _platformClimbingCheckBoxLocalPosition = new Vector2(0, 0);

		private void PlatformClimbing()
		{
			if (Grounded || _rigidbody2D.velocity.y <= 0) return;
			
			bool contactWithPotentialPlatform = Physics2D.BoxCast(
				(Vector2)transform.position + new Vector2(0, _colliderHeight + platformMaxHeight / 2),
				new Vector2(1, platformMaxHeight), 0f, Vector2.zero, 0f, whatIsGround);
			if (!contactWithPotentialPlatform) return;
				
			RaycastHit2D raycastHit = Physics2D.Raycast((Vector3)_platformRaycastCheckLocalStart + transform.position,
				Vector2.down, _colliderHeight + platformMaxHeight, whatIsGround);
			_platformClimbingHitWorldPoint = raycastHit.point;
			if (raycastHit.point == Vector2.zero) return;
			
			float platformHeight = Vector2.Distance(new Vector3(0, _colliderHeight,0) + transform.position, _platformClimbingHitWorldPoint);
			if (platformHeight > platformMaxHeight) return;
					
			_platformClimbingCheckBoxLocalPosition = _platformClimbingHitWorldPoint + new Vector2(0,_colliderHeight / 2 + 0.01f);
			bool haveFreePlace = !Physics2D.BoxCast((Vector3)_platformClimbingCheckBoxLocalPosition, _platformFreePlaceCheckBoxSize, 
				0f, Vector2.zero, 0f, whatIsGround);
			if (!haveFreePlace) return;
				
			transform.position = _platformClimbingHitWorldPoint;
			_rigidbody2D.velocity = Vector2.zero;

			//TODO: add platform climbing animation
			//animator.SetBool("IsPlatformClimbing", true);
		}

		private void PlatformDescent()
		{
			if (!Grounded) return;

			RaycastHit2D raycastHit = Physics2D.Raycast((Vector3)_platformDescentRayCastLocalStart + transform.position,
				Vector2.up, _colliderHeight + platformMaxHeight, whatIsGround);
			_platformDecentHitWorldPoint = raycastHit.point;
			if (raycastHit.point == Vector2.zero) return;
				
			float platformHeight = Vector2.Distance((Vector2)transform.position, _platformDecentHitWorldPoint);
			if (platformHeight > platformMaxHeight) return;

			_platformDecentCheckBoxLocalPosition = _platformDecentHitWorldPoint + new Vector2(0, -_colliderHeight / 2 - 0.01f);
			bool haveFreePlace = !Physics2D.BoxCast((Vector3)_platformDecentCheckBoxLocalPosition, _platformFreePlaceCheckBoxSize, 
				0f, Vector2.zero, 0f, whatIsGround);
			if (!haveFreePlace) return;
			
			transform.position = _platformDecentHitWorldPoint + Vector2.down * _colliderHeight;
			_rigidbody2D.velocity = Vector2.zero;

			//TODO: add platform decent animation
			//animator.SetBool("IsPlatformClimbing", true);
		}
		
		private void MainMove(float move)
		{
			//only control the player if grounded or airControl is turned on
			if (!_isDashing && (Grounded || airControl))
			{
				if (_rigidbody2D.velocity.y < -limitFallSpeed)
					_rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, -limitFallSpeed);

				//if we in air and we try move in wall, then return
				if(!Grounded && (_isWallLower || _isWallUpper) && (_facingRight && move > 0 || !_facingRight && move <0)) return;
				
				// Move the character by finding the target velocity
				Vector3 targetVelocity = new Vector2(move * 10f, _rigidbody2D.velocity.y);
				// And then smoothing it out and applying it to the character
				_rigidbody2D.velocity = Vector3.SmoothDamp(_rigidbody2D.velocity, targetVelocity, ref _velocity,
					movementSmoothing);

				// If the input is moving the player right and the player is facing left...
				if (move > 0 && !_facingRight)
				{
					Flip();
				}
				// Otherwise if the input is moving the player left and the player is facing right...
				else if (move < 0 && _facingRight)
				{
					Flip();
				}
			}
		}
		
		private void Dash(bool dash)
		{
			if (dash && _canDash)
			{
				StartCoroutine(DashCooldown());
			}
			
			if (_isDashing)
			{
				_rigidbody2D.velocity = new Vector2(transform.localScale.x * dashForce, 0);
			}

			if (_isWallUpper && !Grounded && dash && _canDash)
			{
				canDoubleJump = true;
				StartCoroutine(DashCooldown());
			}
		}
		
		private void Jump(bool jump)
		{
			// If the player should jump...
			if (Grounded && jump)
			{
				// Add a vertical force to the player.
				animator.SetBool("IsJumping", true);
				animator.SetBool("JumpUp", true);
				Grounded = false;
				_rigidbody2D.AddForce(new Vector2(0f, jumpForce * _rigidbody2D.mass), ForceMode2D.Impulse);
				canDoubleJump = true;
				particleJumpDown.Play();
				particleJumpUp.Play();
			}
			else if (!Grounded && jump && canDoubleJump)
			{
				canDoubleJump = false;
				_rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, 0);
				_rigidbody2D.AddForce(new Vector2(0f, jumpForce * DoubleJumpScaleModifier * _rigidbody2D.mass), ForceMode2D.Impulse);
				animator.SetBool("IsDoubleJumping", true);
			}
		}
		#endregion

		private void Flip()
		{
			// Switch the way the player is labelled as facing.
			_facingRight = !_facingRight;

			// Multiply the player's x local scale by -1.
			Vector3 theScale = transform.localScale;
			theScale.x *= -1;
			transform.localScale = theScale;
		}

		protected void TakeDamage(Vector3 position)
		{
			animator.SetBool("Hit", true);
				
			Vector2 damageDir = Vector3.Normalize(transform.position - position) * 40f;
			_rigidbody2D.velocity = Vector2.zero;
			_rigidbody2D.AddForce(damageDir * 10);
			
			StartCoroutine(Stun(0.25f));
			StartCoroutine(MakeInvincible(1f));
		}

		#region Coroutines
		IEnumerator DashCooldown()
		{
			animator.SetBool("IsDashing", true);
			_isDashing = true;
			_canDash = false;
			yield return new WaitForSeconds(0.1f);
			_isDashing = false;
			yield return new WaitForSeconds(0.5f);
			_canDash = true;
		}

		IEnumerator Stun(float time)
		{
			_canMove = false;
			yield return new WaitForSeconds(time);
			_canMove = true;
		}

		IEnumerator MakeInvincible(float time)
		{
			Invincible = true;
			yield return new WaitForSeconds(time);
			Invincible = false;
		}

		IEnumerator WaitToMove(float time)
		{
			_canMove = false;
			yield return new WaitForSeconds(time);
			_canMove = true;
		}

		protected IEnumerator WaitToDead()
		{
			_isDead = true;
			animator.SetBool("IsDead", true);
			_canMove = false;
			Invincible = true;
			yield return new WaitForSeconds(0.4f);
			_rigidbody2D.velocity = new Vector2(0, _rigidbody2D.velocity.y);
			yield return new WaitForSeconds(1.1f);
			Destroy(gameObject);
			UI.UI_Controller.SetWindow(UI.ScreenEnum.GameplayMenuScreen);
		}
		#endregion


#if UNITY_EDITOR
		#region Gizmos
		[Header("Gizmos")]
		[SerializeField] private GizmosData groundCheck;
		[SerializeField] private GizmosData frontWallsChecks;
		[SerializeField] private GizmosData groundLedge;
		[SerializeField] private GizmosData platformClimbing;
		[SerializeField] private GizmosData platformDescent;

		private void OnDrawGizmosSelected()
		{
			if(groundCheck.Show) DrawGroundCheckGizmos();
			if(frontWallsChecks.Show) DrawFrontWallsChecksGizmos();
			if(groundLedge.Show) DrawGroundLedgeGizmos();
			if(platformClimbing.Show) DrawPlatformClimbingGizmos();
			if(platformDescent.Show) DrawPlatformDescent();
		}

		private void DrawGroundCheckGizmos()
		{
			Gizmos.color = groundCheck.Color;
			
			Gizmos.DrawWireCube(transform.position, groundCheckSize);
		}
		
		private void DrawFrontWallsChecksGizmos()
		{
			Gizmos.color = frontWallsChecks.Color;

			if(wallUpperCheck) Gizmos.DrawWireCube(wallUpperCheck.position, _wallUpperCheckSize);
			if(wallLowerCheck) Gizmos.DrawWireCube(wallLowerCheck.position, _wallLowerCheckSize);
		}

		private void DrawGroundLedgeGizmos()
		{
			Gizmos.color = groundLedge.Color;

			if(groundLedgeCheck) Gizmos.DrawLine(groundLedgeCheck.position, groundLedgeCheck.position + Vector3.down * 3);
			
			Gizmos.DrawWireCube(_groundLedgeClimbingHitPoint, new Vector2(0.1f,0.1f));
			Gizmos.DrawWireCube(_groundLedgeClimbingCheckPosition, _groundLedgeCheckSize);
		}
		
		private void DrawPlatformClimbingGizmos()
		{
			Gizmos.color = platformClimbing.Color;
			
			Gizmos.DrawLine((Vector3)_platformRaycastCheckLocalStart + transform.position, transform.position + new Vector3(0, _colliderHeight));
			Gizmos.DrawWireCube(transform.position + new Vector3(0,_colliderHeight + platformMaxHeight/2), new Vector2(1,platformMaxHeight));
			
			Gizmos.DrawWireCube(_platformClimbingHitWorldPoint, new Vector2(0.1f,0.1f));
			Gizmos.DrawWireCube(_platformClimbingCheckBoxLocalPosition, _platformFreePlaceCheckBoxSize);
		}

		private void DrawPlatformDescent()
		{
			Gizmos.color = platformDescent.Color;
			
			Gizmos.DrawLine(transform.position + (Vector3)_platformDescentRayCastLocalStart, transform.position);
			Gizmos.DrawWireCube(transform.position - new Vector3(0,platformMaxHeight/2), new Vector2(1,platformMaxHeight));
			
			Gizmos.DrawWireCube(_platformDecentHitWorldPoint, new Vector2(0.1f,0.1f));
			Gizmos.DrawWireCube(_platformDecentCheckBoxLocalPosition, _platformFreePlaceCheckBoxSize);
		}

		[Serializable]
		private class GizmosData
		{
			[field: SerializeField] public bool Show { get; private set; } = true;
			[field: SerializeField] public Color Color { get; private set; } = new Color(1,1,1,1);
		}
		#endregion
#endif
	}
}