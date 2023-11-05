using System.Collections;
using Unity.VisualScripting;
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
		
		public bool CanAttack { get; private set; } //If player can move

		#region ProtectedFields
		protected UnityEvent OnFallEvent;
		protected UnityEvent OnLandEvent;
		#endregion
		
		#region PrivateFields
		private Rigidbody2D _rigidbody2D;
		private Vector3 _velocity = Vector3.zero;
		
		private bool _canMove = true; //If player can move
		private bool _grounded; // Whether or not the player is grounded.
		private bool _facingRight = true; // For determining which way the player is currently facing.
		private bool _canDash = true;
		private bool _isDashing = false; //If player is dashing
		private bool _isWallUpper = false; //If there is a wall in front of the player
		private bool _isWallLower = false;
		private bool _isWallSliding = false; //If player is sliding in a wall
		private bool _oldWallSliding = false; //If player is sliding in a wall in the previous frame
		private bool _isDead = false;
		private bool _limitVelOnWallJump = false; //For limit wall jump distance with low fps
		
		private float _prevVelocityX = 0f;
		private float _limitFallSpeed = 25f; // Limit fall speed
		private float _jumpWallStartX = 0;
		private float _jumpWallDistX = 0; //Distance between player and wall
		private float _colliderHeight;
		private float _groundLedgeMaxHeight;

		private Vector2 _wallUpperCheckSize;
		private Vector2 _wallLowerCheckSize;
		private Vector2 _groundLedgeCheckSize;
		private Vector2 _groundLedgeClimbingHitPoint = new Vector2(0,0);
		private Vector2 _groundLedgeClimbingCheckPosition = new Vector2(0,0);
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
			
			Vector2 colliderSize = GetComponent<CapsuleCollider2D>().size;
			colliderSize.x *= transform.localScale.x;
			colliderSize.y *= transform.localScale.y;
			_groundLedgeCheckSize = colliderSize;
			_colliderHeight = colliderSize.y;
			
			_groundLedgeMaxHeight = colliderSize.y * (groundLedgeMaxHeight / 100);
			_wallUpperCheckSize = _wallLowerCheckSize = new Vector2(wallCheckWidth, _colliderHeight / 2.1f);
		}

		protected override void OnFixedUpdate()
		{
			GroundCheck();
			WallsCheck();
			LimitVelOnWallJump();

			CanAttack = (_grounded && !_isDashing && !_isWallSliding && !_isDead);
		}

		#region OnFixedUpdates
		private void GroundCheck()
		{
			bool groundContact = Physics2D.BoxCast(transform.position, groundCheckSize, 
				0f, Vector2.zero, 0f, whatIsGround);
			
			if(groundContact && !_grounded){
				OnLandEvent.Invoke();
				if (!_isWallUpper && !_isWallLower && !_isDashing)
					particleJumpDown.Play();
				canDoubleJump = true;
				if (_rigidbody2D.velocity.y < 0f)
					_limitVelOnWallJump = false;
			}
			
			_grounded = groundContact;
			
			if (!_grounded)
			{
				OnFallEvent.Invoke();
				
				_prevVelocityX = _rigidbody2D.velocity.x;

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

		private void LimitVelOnWallJump()
		{
			if (_limitVelOnWallJump)
			{
				if (_rigidbody2D.velocity.y < -0.5f)
					_limitVelOnWallJump = false;
				
				_jumpWallDistX = (_jumpWallStartX - transform.position.x) * transform.localScale.x;
				if (_jumpWallDistX < -0.5f && _jumpWallDistX > -1f)
				{
					_canMove = true;
				}
				else if (_jumpWallDistX < -1f && _jumpWallDistX >= -2f)
				{
					_canMove = true;
					_rigidbody2D.velocity = new Vector2(10f * transform.localScale.x, _rigidbody2D.velocity.y);
				}
				else if (_jumpWallDistX < -2f || _jumpWallDistX > 0)
				{
					_limitVelOnWallJump = false;
					_rigidbody2D.velocity = new Vector2(0, _rigidbody2D.velocity.y);
				}
			}
		}
		#endregion

		protected void Move(float move, bool jump, bool dash)
		{
			if(move > 0 && _facingRight || move < 0 && ! _facingRight ) GroundLedgeClimbing();
			
			if (_canMove)// && !animator.GetBool("IsGroundLedgeClimbing"))
			{
				MainMove(move);
				WallSliding(move);
				Dash(dash);
				Jump(jump);
			}
		}

		#region Moves
		private void GroundLedgeClimbing()//TODO: need add climbing animation
		{
			if (!_isWallUpper && _isWallLower && _grounded && !animator.GetBool("IsGroundLedgeClimbing"))
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
		
		private void MainMove(float move)
		{
			//only control the player if grounded or airControl is turned on
			if ((_grounded || airControl) && !_isDashing)
			{
				if (_rigidbody2D.velocity.y < -_limitFallSpeed)
					_rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, -_limitFallSpeed);
				// Move the character by finding the target velocity
				Vector3 targetVelocity = new Vector2(move * 10f, _rigidbody2D.velocity.y);
				// And then smoothing it out and applying it to the character
				_rigidbody2D.velocity = Vector3.SmoothDamp(_rigidbody2D.velocity, targetVelocity, ref _velocity,
					movementSmoothing);

				// If the input is moving the player right and the player is facing left...
				if (move > 0 && !_facingRight && !_isWallSliding)
				{
					Flip();
				}
				// Otherwise if the input is moving the player left and the player is facing right...
				else if (move < 0 && _facingRight && !_isWallSliding)
				{
					Flip();
				}
			}
		}
		
		private void WallSliding(float move)
		{
			if (_isWallUpper && !_grounded)
			{
				if (!_oldWallSliding && _rigidbody2D.velocity.y < 0 || _isDashing)
				{
					_isWallSliding = true;
					canDoubleJump = true;
					animator.SetBool("IsWallSliding", true);
				}

				_isDashing = false;

				if (_isWallSliding)
				{
					if (move * -transform.localScale.x > 0.1f)
					{
						canDoubleJump = true;
						_isWallSliding = false;
						animator.SetBool("IsWallSliding", false);
						_oldWallSliding = false;
					}
					else
					{
						_oldWallSliding = true;
						_rigidbody2D.velocity = new Vector2(transform.localScale.x * 2, -5);
					}
				}
			}
			else if (_oldWallSliding)
			{
				_isWallSliding = false;
				animator.SetBool("IsWallSliding", false);
				_oldWallSliding = false;
				canDoubleJump = true;
			}
		}

		private void Dash(bool dash)
		{
			if (dash && _canDash && !_isWallSliding)
			{
				StartCoroutine(DashCooldown());
			}
			
			if (_isDashing)
			{
				_rigidbody2D.velocity = new Vector2(transform.localScale.x * dashForce, 0);
			}

			if (_isWallUpper && !_grounded && !_isWallSliding && dash && _canDash)
			{
				_isWallSliding = false;
				animator.SetBool("IsWallSliding", false);
				_oldWallSliding = false;
				canDoubleJump = true;
				StartCoroutine(DashCooldown());
			}
		}
		
		private void Jump(bool jump)
		{
			// If the player should jump...
			if (_grounded && jump)
			{
				// Add a vertical force to the player.
				animator.SetBool("IsJumping", true);
				animator.SetBool("JumpUp", true);
				_grounded = false;
				_rigidbody2D.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
				canDoubleJump = true;
				particleJumpDown.Play();
				particleJumpUp.Play();
			}
			else if (!_grounded && jump && canDoubleJump && !_isWallSliding)
			{
				canDoubleJump = false;
				_rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, 0);
				_rigidbody2D.AddForce(new Vector2(0f, jumpForce * DoubleJumpScaleModifier), ForceMode2D.Impulse);
				animator.SetBool("IsDoubleJumping", true);
			}
			else if (_isWallUpper && !_grounded && jump && _isWallSliding)
			{
				animator.SetBool("IsJumping", true);
				animator.SetBool("JumpUp", true);
				_rigidbody2D.velocity = new Vector2(0f, 0f);
				_rigidbody2D.AddForce(new Vector2(transform.localScale.x * jumpForce * DoubleJumpScaleModifier, jumpForce), ForceMode2D.Impulse);
				_jumpWallStartX = transform.position.x;
				_limitVelOnWallJump = true;
				canDoubleJump = true;
				_isWallSliding = false;
				animator.SetBool("IsWallSliding", false);
				_oldWallSliding = false;
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

		public bool IsGrounded()
		{
			return _grounded;
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
			UIController.SetWindow(ScreenEnum.GameplayMenuScreen);
		}
		#endregion


#if UNITY_EDITOR
		private void OnDrawGizmosSelected()
		{
			Gizmos.DrawWireCube(transform.position, groundCheckSize);
			
			if(wallUpperCheck) Gizmos.DrawWireCube(wallUpperCheck.position, _wallUpperCheckSize);
			if(wallLowerCheck) Gizmos.DrawWireCube(wallLowerCheck.position, _wallLowerCheckSize);
			if(groundLedgeCheck) Gizmos.DrawLine(groundLedgeCheck.position, groundLedgeCheck.position + Vector3.down * 3);

			Gizmos.DrawWireCube(_groundLedgeClimbingHitPoint, new Vector2(0.1f,0.1f));
			Gizmos.DrawWireCube(_groundLedgeClimbingCheckPosition, _groundLedgeCheckSize);
		}	
#endif
	}
}