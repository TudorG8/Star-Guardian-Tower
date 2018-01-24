using UnityEngine;
using System.Collections;
using CustomPropertyDrawers;
using UnityChan;

public class PlayerController : Singleton<PlayerController> {
	// Imports --------------------------------------------------------------
	[SerializeField] PhysicsController2D physicsController;
	[SerializeField] AttackTrigger       attackTrigger    ;
	[SerializeField] Animator            animator         ;
	[SerializeField] PlayerRefs          playerRefs       ;
	[SerializeField] SpringManager springManager;
	[SerializeField] ParticleSystem dashParticles  ;
	[SerializeField] ParticleSystem damageParticles;
	[SerializeField] ParticleSystem deathParticles;

	[SerializeField] Transform           savePoint        ;

    // Settings -------------------------------------------------------------
	[SerializeField] float runSpeed               =  5.00f; // The character's running speed
	[SerializeField] float attackCooldown         =  0.50f; // Cooldown between each attacks
	[SerializeField] float maxJumpHeight          =  4.00f; // The character's maximum jump height
	[SerializeField] float minJumpHeight          =  1.00f; // The character's minimum jump height
	[SerializeField] float timeToJump             =  0.50f; // The time it takes to reach jumpHeight
	[SerializeField] float maxFallSpeed           = 20.00f; // The maximum speed the character can fall
	[SerializeField] float accelerationGrounded   =  0.10f; // How fast it takes to reach max horizontal velocity while grounded
	[SerializeField] float accelerationAirborne   =  0.20f; // How fast it takes to reach max horizontal velocity while airborne
	[SerializeField] float maxWallSlideSpeed      =  1.00f; // How fast the player can slide down a wall
	[SerializeField] float wallStickTime          =  0.25f; // How much time you have to jump after moving away form a wall
	[SerializeField] float timeToJumpAfterFalling =  0.20f; // How much time you have to jump off a platform after falling from it
	[SerializeField] Vector2 wallJumpLeap = new Vector2(12, 12); // The velocity applied when you jump away from a wall
	[SerializeField] Vector2 wallJumpHop  = new Vector2( 8, 12); // The velocity applied when you jump towards the same wall

    // Read Only ------------------------------------------------------------

	[SerializeField][ReadOnly] bool    inputEnabled   ; // Whether the player input is enabled or not
	[SerializeField][ReadOnly] bool    gravityEnabled ; // Whether gravity will act upon the player
	[SerializeField][ReadOnly] bool    handleWallSliding;
	[SerializeField][ReadOnly] int     direction      ; // The direction the player is facing (may not always be the velocity)
	[SerializeField][ReadOnly] Vector2 velocity       ; // Current velocity of the player
	[SerializeField][ReadOnly] Vector2 input          ; // Current input of the player
	[SerializeField][ReadOnly] Vector2 simulatedInput ; // Input to be used when input is disabled
	[SerializeField][ReadOnly] float   gravity        ; // Calculated based on jumpHeight and timeToJump
	[SerializeField][ReadOnly] float   maxJumpVelocity; // Calculated based on jumpHeight and timeToJump
	[SerializeField][ReadOnly] float   minJumpVelocity; // Calculated based on jumpHeight and timeToJump
	[SerializeField][ReadOnly] bool    canAttack      ; // Whether the player can attack
	[SerializeField][ReadOnly] bool    canJump        ; // Whether the player can jump
	[SerializeField][ReadOnly] bool    wallSliding    ;
	[SerializeField][ReadOnly] bool    canJumpWhileSliding;
	[SerializeField][ReadOnly] bool    dashing;
	[SerializeField][ReadOnly] bool    canTakeDamage;

	// Private Stuff --------------------------------------------------------
	float smoothingX;
	Coroutine jumpOffCoroutine;
	Coroutine simulateRoutine;

	public PlayerRefs GetPlayerRefs { get { return playerRefs; } }
	public Animator   GetAnimator   { get { return animator  ; } }

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
		inputEnabled = true;

	}

	public delegate void FunctionCall();

	IEnumerator WaitForCooldown(FunctionCall before, float time, FunctionCall after) {
		before();
		yield return new WaitForSeconds (time);
		after ();
	}

	void Awake() {
		InitiateSingleton ();
	}

	void Start() {
		CalculatePhysics   ();
		Reset ();
	}

	public void Reset() {
		canAttack           = true ;
		canJump             = true ;
		canJumpWhileSliding = false;
		inputEnabled        = true ;
		gravityEnabled      = true;
		dashing             = false;
		handleWallSliding = true;
		canTakeDamage = true;
	}

	public void UpdateSavePoint(Transform newSavePoint) {
		savePoint = newSavePoint;
	}

	public void OnDamageTaken() {
		if (canTakeDamage) {
			canTakeDamage = false;
			if (DataSaver.Instance.FinishedTutorial) {
				SessionData.Instance.TakeDamage ();
				ScoreSystem.Instance.TakeDamage ();
			}
			damageParticles.Play ();
			if (DataSaver.Instance.FinishedTutorial && SessionData.Instance.Lives.Value == SessionData.Instance.Lives.Min) {
				ScoreSystem.Instance.ShowGameoverUI ();
				inputEnabled = false;
				gravityEnabled = false;
				velocity = new Vector2 ();
				if (simulateRoutine != null)
					StopCoroutine (simulateRoutine);
				dashing = false;
				StartCoroutine (GameOverRoutine ());
				deathParticles.Play ();
				deathParticles.GetComponent<Animator> ().SetTrigger ("death");
			} else {
				StartCoroutine (DamageTakenRoutine ());
			}
		}
	}


	IEnumerator GameOverRoutine() {
		yield return new WaitForSeconds (0.5f);
		this.transform.position = new Vector2 (-200f, -200f);
	}
		
	public void UpdateAttackRange(ShopItem item) {
		attackTrigger.SetUp (item);
	}

	IEnumerator DamageTakenRoutine() {
		inputEnabled   = false;
		gravityEnabled = false;
		velocity = new Vector2 ();
		if(simulateRoutine != null)
			StopCoroutine (simulateRoutine);
		dashing = false;
		handleWallSliding = true;
		animator.SetTrigger ("damageTaken");
		animator.SetBool ("respawn", true);
		yield return new WaitForSeconds (0.5f);
		transform.position = savePoint.transform.position;
		animator.SetBool ("respawn", false);
		inputEnabled = true ;
		gravityEnabled = true;
		canTakeDamage = true;
	}

	public void EnterRoom (Room room, Direction direction) {
		if (direction != Direction.Top) {
			StartCoroutine (SimulateMovement (0.3f, DirectionHelper.GetDirectionVector (direction), room));
		}
	}
		
		
	void HandleMovement () {
		float smoothingAmount = physicsController.GetCollisionInfo.below ? accelerationGrounded : accelerationAirborne;

		float targetVelocity = Mathf.SmoothDamp (velocity.x, input.x * runSpeed, ref smoothingX, smoothingAmount);

		velocity.x = targetVelocity;

		direction = (int)Mathf.Sign (velocity.x);
	}

	void HandleGravity() {
		if (physicsController.GetCollisionInfo.below || physicsController.GetCollisionInfo.above)
			velocity.y = 0;

		if (gravityEnabled) {
			// Apply gravity
			velocity.y -= gravity * Time.deltaTime;

			// Make sure we don't fall or jump any faster than maxFallSpeed.
			velocity.y = Mathf.Clamp (velocity.y, -maxFallSpeed, maxFallSpeed);
		}
	}

	void HandleWallSliding() {
		if (handleWallSliding) {
			CollisionInfo info = physicsController.GetCollisionInfo;
			int wallDirection = info.left ? -1 : 1;
			wallSliding = false;


			if ((info.left || info.right) && !info.below && physicsController.rayHits >= physicsController.GetRaycastShooter.HorizontalRayCount - 1) {
				wallSliding = true;
				dashParticles.Stop ();

				if (velocity.y < -maxWallSlideSpeed) {
					velocity.y = -maxWallSlideSpeed;
				}

				if (!canJumpWhileSliding) {
					StartCoroutine (WaitForCooldown (
						() => {
							canJumpWhileSliding = true;
						},
						wallStickTime,
						() => {
							canJumpWhileSliding = false;
						}
					));
				} else {
					smoothingX = 0;
				}
				// Player is facing away
				direction = wallDirection * -1;
			}
		}
	}

	void HandleJumping() {
		CollisionInfo info = physicsController.GetCollisionInfo;
		int wallDirection = info.left ? -1 : 1;

		if (Input.GetButtonDown("Jump_P1")) {
			animator.SetBool ("grounded", false);
			// Hanging on an edge
			if (info.hangingOnEdge) {
				//if (input.x != wallDirection) {
					velocity.x = -wallDirection * wallJumpLeap.x;
					velocity.y = maxJumpVelocity;
					StartCoroutine (WaitForCooldown (
						() => {
							physicsController.CheckForEdges = false;
						},
						0.50f,
						() => {
							physicsController.CheckForEdges = true;
						}
					));
				//} 
				/*
				else {
					animator.SetTrigger ("climbUpEdge");
					inputEnabled = false;
					StartCoroutine (WaitForCooldown (
						() => {
							physicsController.CheckForEdges = false;
						},
						0.50f,
						() => {
							physicsController.CheckForEdges = true;
						}
					));
				}
				*/
			}
			// Wall Sliding
			else if (wallSliding) {
				// We are hopping up the wall
				if (input.x == wallDirection) {
					velocity.x = -wallDirection * wallJumpHop.x;
					velocity.y = wallJumpHop.y;
				} 
				// We are leaping off the wall
				else {
					velocity.x = -wallDirection * wallJumpLeap.x;
					velocity.y = wallJumpLeap.y;
				}
				wallSliding = false;
			}
			// Normal Jumping
			else if(canJump) {
				velocity.y = maxJumpVelocity;
			}

			canJump = false;
			if(jumpOffCoroutine != null)
				StopCoroutine (jumpOffCoroutine);
		}
		// If we end the jump early
		if (Input.GetButtonUp ("Jump_P1")) {
			if (velocity.y > minJumpVelocity) {
				velocity.y = minJumpVelocity;
			}
		}

	}

	void HandleAttacking () {
		if ( Input.GetMouseButtonDown(1) && canAttack) {
			animator.SetTrigger ("attack");
			attackTrigger.Attack ();
			StartCoroutine (WaitForCooldown (
				() => {canAttack = false;},
				attackCooldown,
				() => {canAttack = true ;}
			));
		}
	}



	void HandleFallingOffPlatforms(bool previouslyGrounded) {
		// If we are previously grounded but now arent and are falling, it means we are jumping off a platform
		if (previouslyGrounded && !physicsController.GetCollisionInfo.below && (int)Mathf.Sign (velocity.y) == -1) {
			jumpOffCoroutine = StartCoroutine (WaitForCooldown (
				() => { canJump = true; },
				timeToJumpAfterFalling,
				() => { canJump = false;}
			));
		}
	}

	void HandleHittingGround() {
		if (physicsController.GetCollisionInfo.below) {
			animator.SetBool ("grounded", true);
			canJump = true;
			if (jumpOffCoroutine != null)
				StopCoroutine (jumpOffCoroutine);
		}
	}

	void HandleAnimation() {
		if    (wallSliding)    { animator.SetBool ("sliding", true );} 
		else /*notWallSliding*/{ animator.SetBool ("sliding", false);}
		animator.SetFloat("horrizontalSpeed", Mathf.Abs(velocity.x / runSpeed));
		if    (velocity.y <  0)  { animator.SetFloat ("verticalSpeed", velocity.y / maxFallSpeed   ); } 
		else /*velocity.y >= 0*/ { animator.SetFloat ("verticalSpeed", velocity.y / maxJumpVelocity); }
	}

	void UpdateXScale(float value) {
		//Vector2 scale = transform.localScale;
		//scale.x = value;
		//transform.localScale = scale;


	}

	void HandleDashing () {
		CollisionInfo info = physicsController.GetCollisionInfo;
		if (!dashing && Input.GetKeyDown (KeyCode.LeftShift)) {
			gravityEnabled = false;
			inputEnabled   = false;
			dashing        = true;
			handleWallSliding = false;
			velocity = new Vector2 ();
			dashParticles.Play ();
			simulateRoutine = StartCoroutine (SimulateMovement (0.125f, new Vector2(direction * 5, 0), () => { 
				OnDashEnd();
			}));
		}
	}
	void OnDashEnd() {
		gravityEnabled = true;
		inputEnabled   = true;
		dashing = false;
		handleWallSliding = true;
	}

	void Update() {
		if (inputEnabled) { 
			input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
			HandleEverything();
		} 
	}

	public IEnumerator SimulateMovement(float time, Vector2 input, FunctionCall onEnd) {
		float elapsedTime = 0.0f;
		while (elapsedTime < time) {
			elapsedTime += Time.deltaTime;

			this.input = input;
			HandleEverything ();

			yield return new WaitForEndOfFrame ();
		}
		onEnd ();
	}
	
	public IEnumerator SimulateMovement(float time, Vector2 input, Room room) {
		inputEnabled = false;
		float elapsedTime = 0.0f;
		while (elapsedTime < time) {
			elapsedTime += Time.deltaTime;

			this.input = input;
			HandleEverything ();

			yield return new WaitForEndOfFrame ();
		}
		room.CloseEntryGate ();
		inputEnabled = true;
	}

	void HandleEverything() {
		bool previouslyGrounded = physicsController.GetCollisionInfo.below;

		HandleMovement    ();
		HandleGravity     ();
		HandleWallSliding ();
		HandleJumping     ();
		HandleAttacking   ();
		HandleDashing     ();

		UpdateXScale (Mathf.Abs(transform.localScale.x) * direction);


		if (physicsController.GetCollisionInfo.hangingOnEdge) {
			velocity.y = 0;
		}
		int rotation = 0;
		transform.localRotation = Quaternion.Euler (new Vector3 (0, rotation, 0));
		physicsController.Move (velocity * Time.deltaTime, input);

		rotation = direction == 1 ? 0 : 180;
		transform.localRotation = Quaternion.Euler (new Vector3 (0, rotation, 0));

		HandleFallingOffPlatforms (previouslyGrounded);
		HandleHittingGround ();
		HandleAnimation     ();
		if (!physicsController.GetCollisionInfo.below ) {
			animator.SetBool ("grounded", false);
		}
	}
}
