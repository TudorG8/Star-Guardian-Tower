using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPropertyDrawers;

public class PhysicsController2D : ControllerBase {
	[SerializeField] Transform player;

	[SerializeField] int   maxSlopeAngle  = 60;
	[SerializeField] float edgeGrabAmount = 0.1f;

	[SerializeField][ReadOnly] bool fallingThroughPlatform = false;

	PlayerController.StateInfo stateInfo;

	[SerializeField] Vector2 previousVelocity;
	[SerializeField] int rayHits;

	public int RayHits { get { return rayHits; } }

	public delegate void FunctionCall();

	IEnumerator WaitForCooldown(FunctionCall before, float time, FunctionCall after) {
		before();
		yield return new WaitForSeconds (time);
		after ();
	}

	StatementInfo HorrizontalRayFunction (ref Vector2 velocity, RayInfo rayInfo) {
		CollisionInfo   collisionInfo = raycastShooter.GetCollisionInfo  ;
		ColliderCorners boxCorners    = raycastShooter.GetColliderCorners;

		if (rayInfo.hit) {
			rayHits++;

			if (rayInfo.hit.distance == 0) { return StatementInfo.Continue; }

			float slopeAngle = Vector2.Angle (rayInfo.hit.normal, Vector2.up);

			// We check the bottommost ray to see if we are climbing a slope
			if (rayInfo.rayIndex == 0 && slopeAngle < maxSlopeAngle) {
				float distanceToSlopeStart = 0;
				if (slopeAngle != collisionInfo.slopeAngleOld) {
					distanceToSlopeStart = rayInfo.hit.distance-boxCorners.Inset;
					velocity.x -= distanceToSlopeStart * rayInfo.direction;
				}

				collisionInfo.slopeAngle = slopeAngle;
				// Going from an descending slope to an ascending slope
				if (collisionInfo.descendingSlope) {
					collisionInfo.descendingSlope = false;
					velocity = previousVelocity;
				}

				ClimbSlope (ref velocity, slopeAngle);
				velocity.x += distanceToSlopeStart * rayInfo.direction;
			} 

			// Check for edges
			if (stateInfo != null && stateInfo.HoldingOnEdge.GetState == PlayerController.State.CanDoAction && rayInfo.rayIndex == raycastShooter.HorizontalRayCount - 1) {
				Collider2D target = rayInfo.hit.transform.GetComponent<Collider2D> ();
				// We actually hit a target
				if (target != null && target.name.Contains("Grabbable")) {
					Vector2 point = rayInfo.hit.point;
					float distance = Mathf.Abs (target.bounds.max.y - point.y);
					if (distance < edgeGrabAmount) {
						stateInfo.HoldingOnEdge.GetState = PlayerController.State.DoingAction;
						velocity = new Vector2 (0, distance - 0.01f);
						return StatementInfo.Break;
					}
				}
			}

			// Check if we encountered an unclimbable slope
			if (!collisionInfo.ascendingSlope || slopeAngle > maxSlopeAngle) {
				velocity.x = rayInfo.direction * (rayInfo.hit.distance - boxCorners.Inset);
				rayInfo.rayLength  = rayInfo.hit.distance;

				if (collisionInfo.ascendingSlope) {
					float targetYVelocity = Mathf.Tan (collisionInfo.slopeAngle  * Mathf.Deg2Rad) * Mathf.Abs (velocity.x);
					velocity.y = targetYVelocity;
				}

				collisionInfo.left  = rayInfo.direction == -1;
				collisionInfo.right = rayInfo.direction ==  1;
			}
		}

		return StatementInfo.Continue;
	}

	StatementInfo VerticalRayFunction (ref Vector2 velocity, Vector2 input, RayInfo rayInfo) {
		CollisionInfo   collisionInfo = raycastShooter.GetCollisionInfo  ;
		ColliderCorners boxCorners    = raycastShooter.GetColliderCorners;

		if (rayInfo.hit) {
			rayInfo.rayHits++;
			if (rayInfo.hit.collider.name.Contains("Through")) {
				if(rayInfo.direction == 1 || rayInfo.hit.distance == 0 || fallingThroughPlatform)
					return StatementInfo.Continue;

				if (input.y == -1) {
					StartCoroutine(WaitForCooldown( 
						() => {fallingThroughPlatform = true;}, 
						0.5f, 
						() => {fallingThroughPlatform = false;}
					));
					return StatementInfo.Continue;
				}
			}
			if(rayInfo.hit.distance <= boxCorners.Inset) {
				rayInfo.hit.distance = boxCorners.Inset;
			}

			velocity.y = rayInfo.direction * (rayInfo.hit.distance - boxCorners.Inset);
			rayInfo.rayLength  = rayInfo.hit.distance;

			if (collisionInfo.ascendingSlope) {
				velocity.x = velocity.y / Mathf.Tan (collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign (velocity.x);
			}

			collisionInfo.below = rayInfo.direction == -1;
			collisionInfo.above = rayInfo.direction ==  1;
		}

		return StatementInfo.Continue;
	}

	void ClimbSlope(ref Vector2 velocity, float slopeAngle) {
		float moveDistance = Mathf.Abs (velocity.x);
		float targetYVelocity = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;

		if (velocity.y <= targetYVelocity) {
			velocity.y = targetYVelocity;
			velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);
			raycastShooter.GetCollisionInfo.below          = true;
			raycastShooter.GetCollisionInfo.ascendingSlope = true;
		}
	}

	void DescendSlope(ref Vector2 velocity) {
		CollisionInfo   collisionInfo = raycastShooter.GetCollisionInfo;
		ColliderCorners boxCorners    = raycastShooter.GetColliderCorners   ;

		float direction = Mathf.Sign (velocity.x);
		Vector2 rayOrigin = direction == -1 ? boxCorners.BottomRight : boxCorners.BottomLeft;
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

		if (hit) {
			float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);

			if (slopeAngle != 0 && 
				slopeAngle <= maxSlopeAngle && 
				Mathf.Sign (hit.normal.x) == direction && 
				(hit.distance - boxCorners.Inset) <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x) 
			){
				float moveDistance = Mathf.Abs (velocity.x);
				float targetYVelocity = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
				velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);
				velocity.y -= targetYVelocity;
				collisionInfo.descendingSlope = true;
			}
		}
	}

	void CheckForAngleChange(ref Vector2 velocity) {
		CollisionInfo   collisionInfo = raycastShooter.GetCollisionInfo;
		ColliderCorners boxCorners    = raycastShooter.GetColliderCorners   ;

		if (collisionInfo.ascendingSlope) {
			float direction = Mathf.Sign(velocity.x);
			float rayLength = Mathf.Abs (velocity.x) + boxCorners.Inset;
			Vector2 rayOrigin = direction == -1 ? boxCorners.BottomLeft : boxCorners.BottomRight;
			rayOrigin += Vector2.up * velocity.y;

			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right * direction, rayLength, collisionMask);
			if (hit) {
				float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);
				if (slopeAngle != collisionInfo.slopeAngle) {
					velocity.x = (hit.distance - boxCorners.Inset) * direction;
					collisionInfo.slopeAngle = slopeAngle;
				}
			}
		}
	}

	public void Update() {
		//PrintVelocities ();
		PlayerController.Instance.transform.localRotation = Quaternion.Euler (new Vector3 ());
		Move (GetVelocity() * Time.deltaTime, PlayerController.Instance.GetInput (), PlayerController.Instance.GetStateInfo ());
		PlayerController.Instance.AfterMove ();
		ResetVelocity ();
	}

	public override void Move(Vector2 velocity, Vector2 input, PlayerController.StateInfo stateInfo) {
		raycastShooter.Reset ();
		this.stateInfo = stateInfo;
		previousVelocity = velocity;
			
		// Handle horrizontal movement
		rayHits = 0;
		raycastShooter.ShootHorrizontalRays (ref velocity, Color.red, velocity.x, false, true, collisionMask, (rayInfo) => {
			return HorrizontalRayFunction(ref velocity, rayInfo);
		});

		if (!raycastShooter.GetCollisionInfo.hangingOnEdge) {
			// Handle vertical movement
			if (velocity.y != 0) {
				raycastShooter.ShootVerticalRays (ref velocity, Color.red, velocity.y, true, true, collisionMask, (rayInfo) => {
					return VerticalRayFunction (ref velocity, input, rayInfo);
				});

				CheckForAngleChange (ref velocity);
			}
		}

		// Always check downwards if we are hitting something
		// If for example we are on a platform, the checks before will never launch
		Vector2 downwards = new Vector2 (0f, -0.5f);
		if(HasVelocity("Gravity") && HasVelocity("Jump")) {
			if (velocities["Gravity"].y == 0 && velocities["Jump"].y <= 0) {
					raycastShooter.ShootVerticalRays (ref downwards, Color.blue, -1f, true, true, collisionMask, (rayInfo) => {
					if (rayInfo.hit) {
						GetCollisionInfo.below = true;
						return StatementInfo.Break;
					}
					return StatementInfo.Continue;
				});
			}
		}

		player.Translate (velocity);
	}
}
