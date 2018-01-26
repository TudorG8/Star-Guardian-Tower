using UnityEngine;
using System.Collections;
using CustomPropertyDrawers;
using UnityChan;

public class PlayerController : Singleton<PlayerController>, InputableEntity {
	// Imports -------------------------------------------------------------------------------------------------------------
	[SerializeField] PhysicsController2D physicsController;
	[SerializeField] AttackTrigger       attackTrigger    ;
	[SerializeField] Animator            animator         ;
	[SerializeField] PlayerRefs          playerRefs       ;
	[SerializeField] ParticleSystem      dashParticles    ;
	[SerializeField] ParticleSystem      damageParticles  ;
	[SerializeField] ParticleSystem      deathParticles   ;
	[SerializeField] Transform           savePoint        ;

	// Settings ------------------------------------------------------------------------------------------------------------
	[SerializeField] float runSpeed               =  5.00f; // The character's running speed
	[SerializeField] float attackCooldown         =  0.50f; // Cooldown between each attacks
	[SerializeField] float dashingCooldown        =  0.50f; // The minimum delay between each dash
	[SerializeField] float dashingDuration        =  0.12f; // How long the dash lasts
	[SerializeField] float dashingSpeed           =  5.00f; // How quick the dash ish
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

	// Read Only -----------------------------------------------------------------------------------------------------------
	[SerializeField][ReadOnly] bool      inputEnabled      ; // Whether the player input is enabled or not
	[SerializeField][ReadOnly] bool      gravityEnabled    ; // Whether gravity will act upon the player
	[SerializeField][ReadOnly] bool      wallSlidingEnabled; // Whether wall sliding is enabled
	[SerializeField][ReadOnly] int       direction         ; // The direction the player is facing (may not always be the velocity)
	//[SerializeField][ReadOnly] Vector2   velocity          ; // Current velocity of the player
	[SerializeField][ReadOnly] Vector2 gravityVelocity;
	[SerializeField][ReadOnly] Vector2 jumpVelocity;
	[SerializeField][ReadOnly] Vector2 movementVelocity;
	[SerializeField][ReadOnly] Vector2   input             ; // Current input of the player
	[SerializeField][ReadOnly] Vector2   simulatedInput    ; // Input to be used when input is disabled
	[SerializeField][ReadOnly] float     gravity           ; // Calculated based on jumpHeight and timeToJump
	[SerializeField][ReadOnly] float     maxJumpVelocity   ; // Calculated based on jumpHeight and timeToJump
	[SerializeField][ReadOnly] float     minJumpVelocity   ; // Calculated based on jumpHeight and timeToJump
	[SerializeField][ReadOnly] bool previouslyGrounded;
	[SerializeField]           StateInfo stateInfo;

	public enum State {
		CanDoAction, DoingAction, CantDoAction, Waiting
	}
	[System.Serializable]
	public class RoutineState {
		[SerializeField][ReadOnly] State state;
		[SerializeField] Coroutine routine;

		public State     GetState { get { return state  ; } set { state   = value;} }
		public Coroutine Routine  { get { return routine; } set { routine = value;} }

		public void Reset(MonoBehaviour parent) {
			state = State.CanDoAction;
			if (routine != null) {
				parent.StopCoroutine (routine);
			}
		}
	}

	[System.Serializable]
	public class StateInfo {
		[SerializeField] RoutineState attacking    ;
		[SerializeField] RoutineState jumping      ;
		[SerializeField] RoutineState dashing      ;
		[SerializeField] RoutineState takingDamage ;
		[SerializeField] RoutineState wallSliding  ;
		[SerializeField] RoutineState holdingOnEdge;

		public RoutineState Attacking      { get { return attacking    ; } set { attacking     = value;} }
		public RoutineState Jumping        { get { return jumping      ; } set { jumping       = value;} }
		public RoutineState Dashing        { get { return dashing      ; } set { dashing       = value;} }
		public RoutineState TakingDamage   { get { return takingDamage ; } set { takingDamage  = value;} }
		public RoutineState WallSliding    { get { return wallSliding  ; } set { wallSliding   = value;} }
		public RoutineState HoldingOnEdge  { get { return holdingOnEdge; } set { holdingOnEdge = value;} }

		public void Reset(MonoBehaviour monobehaviour) {
			attacking    .Reset (monobehaviour);
			jumping      .Reset (monobehaviour);
			dashing      .Reset (monobehaviour);
			takingDamage .Reset (monobehaviour);
			wallSliding  .Reset (monobehaviour);
			holdingOnEdge.Reset (monobehaviour);
		}
	}


	// Private Stuff --------------------------------------------------------
	float     smoothingX     ; // Horizontal smoothing
	Coroutine simulateRoutine; // Routine for whenever we simulate movement

	// Properties
	public PlayerRefs GetPlayerRefs { get { return playerRefs; } }
	public Animator   GetAnimator   { get { return animator  ; } }

	public Vector2 GetInput() {
		return input;
	}
	public StateInfo GetStateInfo() {
		return stateInfo;
	}

	// Physics -------------------------------------------------------------------------------------------------------------
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

	// Misc Stuff ----------------------------------------------------------------------------------------------------------
	public delegate void FunctionCall();

	IEnumerator WaitForCooldown(FunctionCall before, float time, FunctionCall after) {
		before();
		yield return new WaitForSeconds (time);
		after ();
	}

	void Awake() { InitiateSingleton (); }

	void Start() {
		CalculatePhysics   ();
		Reset ();
	}

	public void Reset() {
		stateInfo.Reset(this);
		SetActive (gravity: true, input: true, wallSliding: true);
	}

	void SetActive(bool gravity, bool input, bool wallSliding) {
		gravityEnabled     = gravity    ;
		inputEnabled       = input      ;
		wallSlidingEnabled = wallSliding;
	}

	// Save Points ---------------------------------------------------------------------------------------------------------
	public void UpdateSavePoint(Transform newSavePoint) {
		savePoint = newSavePoint;
	}

	// Damage --------------------------------------------------------------------------------------------------------------
	public void OnDamageTaken() {
		if (stateInfo.TakingDamage.GetState == State.CanDoAction) {
			stateInfo.TakingDamage.GetState = State.DoingAction;
			if (DataSaver.Instance.FinishedTutorial) {
				SessionData.Instance.TakeDamage ();
				ScoreSystem.Instance.TakeDamage ();
			}
			damageParticles.Play ();
			if (DataSaver.Instance.FinishedTutorial && SessionData.Instance.Lives.Value == SessionData.Instance.Lives.Min) {
				ScoreSystem.Instance.ShowGameoverUI ();
				StopDash ();
				SetActive (gravity: false, input: false, wallSliding: false);

				StartCoroutine (GameOverRoutine ());
				deathParticles.Play ();
				deathParticles.GetComponent<Animator> ().SetTrigger ("death");
			} 
			else {
				StartCoroutine (DamageTakenRoutine ());
			}
		}
	}

	IEnumerator GameOverRoutine() {
		yield return new WaitForSeconds (0.5f);
		this.transform.position = new Vector2 (-200f, -200f);
	}

	IEnumerator DamageTakenRoutine() {
		StopDash ();
		SetActive (gravity: false, input: false, wallSliding: false);

		animator.SetTrigger ("damageTaken");
		animator.SetBool ("respawn", true);

		yield return new WaitForSeconds (0.5f);

		transform.position = savePoint.transform.position;

		animator.SetBool ("respawn", false);

		SetActive (gravity: true, input: true, wallSliding: true);

		stateInfo.TakingDamage.GetState = State.CanDoAction;
	}

	// Handle Input --------------------------------------------------------------------------------------------------------
	void HandleMovement () {
		float smoothingAmount = physicsController.GetCollisionInfo.below ? accelerationGrounded : accelerationAirborne;

		float targetVelocity = Mathf.SmoothDamp (movementVelocity.x, input.x * runSpeed, ref smoothingX, smoothingAmount);

		movementVelocity.x = targetVelocity;

		direction = (int)Mathf.Sign (movementVelocity.x);
	}

	void HandleGravity() {
		if (physicsController.GetCollisionInfo.below || physicsController.GetCollisionInfo.above)
			gravityVelocity.y = 0;

		if (gravityEnabled) {
			// Apply gravity
			gravityVelocity.y -= gravity * Time.deltaTime;

			// Make sure we don't fall or jump any faster than maxFallSpeed.
			gravityVelocity.y = Mathf.Clamp (gravityVelocity.y, -maxFallSpeed, maxFallSpeed);
		}
	}

	void HandleWallSliding() {
		CollisionInfo info = physicsController.GetCollisionInfo;
		int wallDirection = info.left ? -1 : 1;

		if (wallSlidingEnabled && stateInfo.Jumping.GetState != State.CantDoAction) {
			// We are wall sliding if there is a collision to the left or right, no collision below and if most of the rays are hitting
			if ((info.left || info.right) && !info.below && physicsController.RayHits >= physicsController.GetRaycastShooter.HorizontalRayCount - 1) {
				if (stateInfo.WallSliding.GetState == State.CanDoAction) {
					gravityVelocity = new Vector2 (0, gravityVelocity.y + jumpVelocity.y);
					Debug.Log (gravityVelocity.y + jumpVelocity.y);
					stateInfo.WallSliding.GetState = State.DoingAction;
				}
				jumpVelocity = new Vector2 ();
				dashParticles.Stop ();

				if (gravityVelocity.y < -maxWallSlideSpeed) { gravityVelocity.y = -maxWallSlideSpeed;	}

				// We check if the player is trying to move away from a wall
				if (stateInfo.Jumping.GetState == State.CanDoAction && input.x != 0 && input.x != wallDirection) {
					// If they are, we start this routine (once)
					if(stateInfo.Jumping.Routine == null) {
						stateInfo.Jumping.Routine = StartCoroutine (WaitForCooldown (
							() => { },
							wallStickTime,
							() => { 
								// At the end of it, the player cant jump anymore
								stateInfo.Jumping    .GetState = State.CantDoAction; 
								stateInfo.WallSliding.Routine = StartCoroutine(WaitForCooldown (
									() => { stateInfo.WallSliding.GetState = State.CantDoAction; },
									0.1f,
									// We also disable wall sliding for a little 
									() => { 
										stateInfo.WallSliding.GetState = State.CanDoAction ; 
										stateInfo.Jumping.Routine = null;
									}
								));
							}
						));
					}
				}
				// Still hanging on the wall
				else {
					stateInfo.Jumping.Reset (this);
				}

				movementVelocity.x = wallDirection;
				smoothingX = 0;

				// Player is facing away
				direction = wallDirection * -1;
			}
		}
		else {
			if (info.left ) direction =  1;
			if (info.right) direction = -1;
		}	
	}

	void HandleJumping() {
		CollisionInfo info = physicsController.GetCollisionInfo;
		int wallDirection = info.left ? -1 : 1;

		if (Input.GetButtonDown("Jump_P1")) {
			animator.SetBool ("grounded", false);
			// Hanging on an edge
			if (stateInfo.HoldingOnEdge.GetState == State.DoingAction) {
				stateInfo.HoldingOnEdge.Routine = StartCoroutine (WaitForCooldown (
					() => { stateInfo.HoldingOnEdge.GetState = State.CantDoAction; },
					0.50f,
					() => { stateInfo.HoldingOnEdge.GetState = State.CanDoAction ; }
				));

				if (input.x != 0 && input.x != wallDirection) {
					movementVelocity.x = -wallDirection * wallJumpLeap.x;
					jumpVelocity.y = maxJumpVelocity;
				} 
				// Either jumping upwards or towards the wall
				else {
					Vector2 dir = new Vector2 (0, 1);
					dir.x = wallDirection * 2.5f;
					jumpVelocity = new Vector2 ();
					simulateRoutine = StartCoroutine(SimulateMovement (0.25f, dir, () => { 
						inputEnabled = true;
					}, true, 6f));
					return;
				}
			}
			// Wall Sliding
			else if (stateInfo.WallSliding.GetState == State.DoingAction) {
				// We are hopping up the wall
				if (input.x == wallDirection) {
					movementVelocity.x = -wallDirection * wallJumpHop.x;
					gravityVelocity.y = 0;
					jumpVelocity.y = wallJumpHop.y;
				} 
				// We are leaping off the wall
				else {
					movementVelocity.x = -wallDirection * wallJumpLeap.x;
					gravityVelocity.y = 0;
					jumpVelocity.y = wallJumpLeap.y;
				}
				stateInfo.WallSliding.GetState = State.CanDoAction;
			}
			// Normal Jumping
			else if(stateInfo.Jumping.GetState == State.CanDoAction) {
				gravityVelocity.y = 0;
				jumpVelocity.y = maxJumpVelocity;
				if (physicsController.HasVelocity ("Platform")) {
					movementVelocity.x = physicsController.GetSingleVelocity ("Platform").x;
				}
			}

			stateInfo.Jumping.GetState = State.DoingAction;
			if(stateInfo.Jumping.Routine != null)
				StopCoroutine (stateInfo.Jumping.Routine);
		}
		// If we end the jump early
		if (Input.GetButtonUp ("Jump_P1")) {
			float speed = gravityVelocity.y + jumpVelocity.y;
			if (speed > minJumpVelocity) { jumpVelocity.y = minJumpVelocity; }
		}

	}

	void HandleAttacking () {
		if ( Input.GetMouseButtonDown(1) && stateInfo.Attacking.GetState == State.CanDoAction ) {
			animator.SetTrigger ("attack");
			attackTrigger.Attack ();
			stateInfo.Attacking.Routine = StartCoroutine (WaitForCooldown (
				() => {stateInfo.Attacking.GetState = State.Waiting; },
				attackCooldown,
				() => {stateInfo.Attacking.GetState = State.CanDoAction ; }
			));
		}
	}



	void HandleFallingOffPlatforms(bool previouslyGrounded) {
		float speed = gravityVelocity.y + jumpVelocity.y;
		// If we are previously grounded but now arent and are falling, it means we are jumping off a platform
		if (previouslyGrounded && !physicsController.GetCollisionInfo.below && (int)Mathf.Sign (speed) == -1) {
			stateInfo.Jumping.Routine = StartCoroutine (WaitForCooldown (
				() => { stateInfo.Jumping.GetState = State.CanDoAction ; },
				timeToJumpAfterFalling,
				() => { stateInfo.Jumping.GetState = State.CantDoAction; }
			));
		}
	}

	void HandleHittingGround() {
		CollisionInfo info = physicsController.GetCollisionInfo;
		if (physicsController.GetCollisionInfo.below) {
			if (stateInfo.WallSliding.GetState == State.DoingAction) {
				// Make a grateful landing
				movementVelocity.x = direction * 0.001f;
			}
			if (stateInfo.Dashing.Routine == null) {
				// Make a grateful landing
				stateInfo.Dashing    .Reset (this);
			}
			animator.SetBool ("grounded", true);
			stateInfo.Jumping    .Reset (this);
			stateInfo.WallSliding.Reset (this);

			jumpVelocity    = new Vector2 ();
			gravityVelocity = new Vector2 ();
		}
		if (physicsController.GetCollisionInfo.above) {
			jumpVelocity    = new Vector2 ();
			gravityVelocity = new Vector2 ();
		}
	}

	void HandleAnimation() {
		float speed = gravityVelocity.y + jumpVelocity.y;
		if      (stateInfo.WallSliding  .GetState == State.DoingAction) { animator.SetBool ("sliding", true );} 
		else if (stateInfo.HoldingOnEdge.GetState == State.DoingAction) { animator.SetBool ("sliding", true );} 
		else   /*not wall sliding */                                    { animator.SetBool ("sliding", false);}
		animator.SetFloat("horrizontalSpeed", Mathf.Abs(movementVelocity.x / runSpeed));
		if    (speed <  0)  { animator.SetFloat ("verticalSpeed", speed / maxFallSpeed   ); } 
		else /*speed >= 0*/ { animator.SetFloat ("verticalSpeed", speed / maxJumpVelocity); }
		if (!physicsController.GetCollisionInfo.below ) { animator.SetBool ("grounded", false); }
	}
		
	void HandleDashing () {
		CollisionInfo info = physicsController.GetCollisionInfo;
		if (stateInfo.Dashing.GetState == State.CanDoAction && Input.GetKeyDown (KeyCode.LeftShift)) {
			SetActive (gravity: false, input: false, wallSliding: false);
			stateInfo.WallSliding.Reset (this);
			stateInfo.Dashing.GetState = State.DoingAction;

			gravityVelocity = new Vector2 ();
			jumpVelocity = new Vector2 ();
			dashParticles.Play ();

			simulateRoutine = StartCoroutine (SimulateMovement (dashingDuration, new Vector2(direction * dashingSpeed, 0), () => { 
				OnDashEnd();
			}));
		}
	}

	void OnDashEnd() {
		SetActive (gravity: true, input: true, wallSliding: true);
		stateInfo.Dashing.Routine = StartCoroutine(WaitForCooldown( 
			() => { stateInfo.Dashing.GetState = State.Waiting; },
			dashingCooldown,
			() => { 
				if(stateInfo.Jumping.GetState != State.DoingAction) {
					stateInfo.Dashing.GetState = State.CanDoAction ; 
				}
				stateInfo.Dashing.Routine = null;
			}
		));
	}

	void StopDash() {
		gravityVelocity = new Vector2 ();
		jumpVelocity = new Vector2 ();
		stateInfo.Dashing.Reset (this);
		if (simulateRoutine != null) {
			StopCoroutine (simulateRoutine);
		}
		OnDashEnd ();
	}
		
	void HandleHangingOnEdge() {
		if (stateInfo.HoldingOnEdge.GetState == State.DoingAction) { 
			stateInfo.WallSliding.Reset (this);
			gravityVelocity.y = 0; 
		}
	}

	// Entering a Room -----------------------------------------------------------------------------------------------------
	public void EnterRoom (Room room, Direction direction) {
		Vector2 dir = DirectionHelper.GetDirectionVector (DirectionHelper.GetOpposite(room.Entry.Main));
		inputEnabled = false;

		if (room.Entry.Main == Direction.Bottom) {
			dir.x = Input.GetAxisRaw ("Horizontal");
			simulateRoutine = StartCoroutine (SimulateMovement (0.2f, dir, () => {
				room.CloseEntryGate ();
				inputEnabled = true; 
			}, true, 8f));
		} 
		else {
			simulateRoutine = StartCoroutine (SimulateMovement (0.35f, dir, () => {
				room.CloseEntryGate ();
				inputEnabled = true; 
			}));
		}
	}	

	// Movement ------------------------------------------------------------------------------------------------------------
	void Update() {
		if (inputEnabled) { 
			input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
			HandleEverything();
		} 
	}
		
	public IEnumerator SimulateMovement(float time, Vector2 input, FunctionCall onEnd, bool overrideYVelocity = false, float YVelocity = 0f) {
		float elapsedTime = 0.0f;
		while (elapsedTime < time) {
			elapsedTime += Time.deltaTime;

			this.input = input;
			HandleEverything (overrideYVelocity, YVelocity);

			yield return new WaitForEndOfFrame ();
		}
		onEnd ();
	}

	void HandleEverything(bool overrideYVelocity = false, float YVelocity = 0f) {
		HandleMovement      ();
		HandleGravity       ();
		HandleWallSliding   ();
		HandleHangingOnEdge ();
		HandleJumping       ();
		HandleAttacking     ();
		HandleDashing       ();

		if (overrideYVelocity) { 
			gravityVelocity.y = YVelocity; 
			jumpVelocity.y = YVelocity;
		}

		physicsController.AddVelocity ("Movement", movementVelocity);
		physicsController.AddVelocity ("Gravity" , gravityVelocity );
		physicsController.AddVelocity ("Jump"    , jumpVelocity    );
	}

	public void AfterMove() {
		previouslyGrounded = physicsController.GetCollisionInfo.below;

		//transform.localRotation = Quaternion.Euler (new Vector3 ());
		//physicsController.Move (physicsController.GetVelocity() * Time.deltaTime, input, stateInfo);
		transform.localRotation = Quaternion.Euler (new Vector3 (0, direction == 1 ? 0 : 180, 0));

		HandleFallingOffPlatforms (previouslyGrounded);
		HandleHittingGround ();
		HandleAnimation     ();
	}
}
// -------------------------------------------------------------------------------------------------------------------------