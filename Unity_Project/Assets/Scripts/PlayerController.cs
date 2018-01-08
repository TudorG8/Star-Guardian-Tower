using UnityEngine;
using System.Collections;
using CustomPropertyDrawers;

public class PlayerController : MonoBehaviour {
	// Imports --------------------------------------------------------------
	[SerializeField] PhysicsController2D physicsController;
	[SerializeField] AttackTrigger       attackTrigger    ;
	[SerializeField] Animator            animator         ;

    // Settings -------------------------------------------------------------
	[SerializeField] float runSpeed       =  5.0f; // The character's running speed
	[SerializeField] float attackCooldown =  0.5f; // Cooldown between each attacks
	[SerializeField] float maxJumpHeight  =  4.0f; // The character's maximum jump height
	[SerializeField] float minJumpHeight  =  1.0f; // The character's minimum jump height
	[SerializeField] float timeToJump     =  0.5f; // The time it takes to reach jumpHeight
	[SerializeField] float maxFallSpeed   = 20.0f; // The maximum speed the character can fall
	[SerializeField] float accelerationGrounded = 0.1f;
	[SerializeField] float accelerationAirborne = 0.2f;
	[SerializeField] float maxWallSlideSpeed = 1;
	[SerializeField] float wallStickTime = 0.25f;
	[SerializeField] float timeToJumpAfterFalling = 0.20f;
	[SerializeField] Vector2 wallJumpLeap;
	[SerializeField] Vector2 wallJumpHop ;
	[SerializeField] Vector2 wallJumpLet ;

    // Read Only ------------------------------------------------------------
	[SerializeField][ReadOnly] Vector2 velocity    ; // Current velocity of the player
	[SerializeField][ReadOnly] Vector2 input       ; // Current input of the player
	[SerializeField][ReadOnly] float   gravity     ; // Calculated based on jumpHeight and timeToJump
	[SerializeField][ReadOnly] float   maxJumpVelocity; // Calculated based on jumpHeight and timeToJump
	[SerializeField][ReadOnly] float   minJumpVelocity; // Calculated based on jumpHeight and timeToJump
	[SerializeField][ReadOnly] bool    canAttack   ;
	[SerializeField][ReadOnly] bool    canJump     ;
	[SerializeField][ReadOnly] bool    canJumpWhileSliding;

	// Private Stuff --------------------------------------------------------
	float smoothingX;
	Coroutine jumpOffCoroutine;
	public bool inputDisabled;

	// Unity Stuff ----------------------------------------------------------
	/* Solve for gravity and jumpVelocity using jumpHeight and timeToJump
	 * from physics we know:
	 * 		velocity      = initialVelocity * time + (acceleration * time^2) / 2
	 * 		finalVelocity = initialVelocity + acceleration * time;
	 * using our variables:
	 * 		jumpHeight   = (gravity * timeToJump^2) / 2
	 * 
	 * 		gravity      = 2 * jumpHeight / timeToJump^2
	 * 		jumpVelocity = gravity * timeToJump
	 */
	void CalculatePhysics () {
		gravity      = (2 * maxJumpHeight) / Mathf.Pow (timeToJump, 2);
		maxJumpVelocity = gravity * timeToJump;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpHeight);
	}

	public void EnableInput() {
		inputDisabled = false;
	}

	public delegate void FunctionCall();

	IEnumerator WaitForCooldown(FunctionCall before, float time, FunctionCall after) {
		before();
		yield return new WaitForSeconds (time);
		after ();
	}

	void Start() {
		CalculatePhysics   ();
		canAttack           = true ;
		canJump             = true;
		canJumpWhileSliding = false;
	}



	void UpdateMovementState() {
		float smoothingAmount = physicsController.raycastShooter.collisionInfo.below ? accelerationGrounded : accelerationAirborne;

		float targetVelocity = Mathf.SmoothDamp (velocity.x, input.x * runSpeed, ref smoothingX, smoothingAmount);
		// We want to accelerate towards our target velocity smoothly rather than instantly.
		velocity.x = targetVelocity;
		animator.SetFloat("horrizontalSpeed", Mathf.Abs(velocity.x / runSpeed));
	}

	void ApplyGravity() {
		if (physicsController.raycastShooter.collisionInfo.below || physicsController.raycastShooter.collisionInfo.above)
			velocity.y = 0;
		// Apply gravity
		velocity.y -= gravity * Time.deltaTime;

		// Make sure we don't fall any faster than maxFallSpeed.
		velocity.y = Mathf.Clamp(velocity.y, -maxFallSpeed, maxFallSpeed);
	}

	void UpdateJumpState() {
		CollisionInfo info = physicsController.raycastShooter.collisionInfo;
		int  wallDirection = info.left ? -1 : 1;
		bool wallSliding   = false;

		if ((info.left || info.right) && !info.below) {
			wallSliding = true;

			if (velocity.y < -maxWallSlideSpeed) {
				velocity.y = -maxWallSlideSpeed;
			}
				
			if (!canJumpWhileSliding) {
				StartCoroutine (WaitForCooldown (
					() => { canJumpWhileSliding = true ; },
					wallStickTime,
					() => { canJumpWhileSliding = false; }
				));
			} 
			else {
				//velocity.x = 0;
				smoothingX = 0;
			}
		}
		// Character can jump when standing on the ground
		if (Input.GetButtonDown("Jump_P1")) {
			animator.SetBool ("grounded", false);
			if (info.hangingOnEdge) {
				GetComponent<Animator> ().SetTrigger ("jump");
				inputDisabled = true;
				StartCoroutine (WaitForCooldown (
					() => { physicsController.checkForEdges = false; },
					0.50f,
					() => { physicsController.checkForEdges = true ; }
				));
			}
			else if (wallSliding) {
				// We are hopping up the wall
				if (input.x == wallDirection) {
					velocity.x = -wallDirection * wallJumpHop.x;
					velocity.y = wallJumpHop.y;
				} 
				// We are just jumping off
				else if (input.x == 0) {
					velocity.x = -wallDirection * wallJumpLet.x;
					velocity.y = wallJumpLet.y;
				} 
				// We are leaping off the wall
				else {
					velocity.x = -wallDirection * wallJumpLeap.x;
					velocity.y = wallJumpLeap.y;
				}
			}
			else if(canJump) {
				velocity.y = maxJumpVelocity;
			}
			canJump = false;
			if(jumpOffCoroutine != null)
				StopCoroutine (jumpOffCoroutine);
		}
		if (Input.GetButtonUp ("Jump_P1")) {
			if (velocity.y > minJumpVelocity) {
				velocity.y = minJumpVelocity;
			}
		}
	}

	void UpdateAttackState() {
		if (Input.GetKeyDown (KeyCode.LeftControl) && canAttack) {
			attackTrigger.Attack (new Vector2(Mathf.Sign(velocity.x), 0));
			StartCoroutine (WaitForCooldown (
				() => {canAttack = false;},
				attackCooldown,
				() => {canAttack = true ;}
			));
		}
	}

	void Update() {
		if (!inputDisabled) {
			input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
			UpdateMovementState ();
			ApplyGravity ();
			UpdateJumpState ();
			UpdateAttackState ();

			if (velocity.x != 0) {
				Vector2 scale = transform.localScale;
				scale.x = Mathf.Sign (velocity.x);
				transform.localScale = scale;
			}



			// Move character
			bool previouslyGrounded = physicsController.raycastShooter.collisionInfo.below;

			physicsController.Move (velocity * Time.deltaTime, input);

			// If we are previously grounded but now arent and are falling, it means we are jumping off a platform
			if (previouslyGrounded && !physicsController.raycastShooter.collisionInfo.below && (int)Mathf.Sign (velocity.y) == -1) {
				jumpOffCoroutine = StartCoroutine (WaitForCooldown (
					() => { canJump = true; },
					timeToJumpAfterFalling,
					() => { canJump = false;}
				));
			}

			if (physicsController.raycastShooter.collisionInfo.below) {
				canJump = true;
				if (jumpOffCoroutine != null)
					StopCoroutine (jumpOffCoroutine);
			}

			if (physicsController.raycastShooter.collisionInfo.below || physicsController.raycastShooter.collisionInfo.above) {
				if (!physicsController.raycastShooter.collisionInfo.slidingDownSlope) {
					velocity.y = 0;
					animator.SetBool ("grounded", true);
				}
			}

			if (velocity.y < 0) {
				animator.SetFloat ("verticalSpeed", velocity.y / maxFallSpeed   );
			} 
			else {
				animator.SetFloat ("verticalSpeed", velocity.y / maxJumpVelocity);
			}
		}
	}
}
