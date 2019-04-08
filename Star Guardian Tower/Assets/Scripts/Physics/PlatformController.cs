using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Controller for platforms used to carry passangers
 */

public class PlatformController : ControllerBase {
	// Every time we hit a ray, we hold this information about them so we can use it later
	class TargetInformation {
		[SerializeField] Transform transform         ;
		[SerializeField] Vector2   velocity          ;
		[SerializeField] bool      moveBeforePlatform;
		[SerializeField] bool      reset             ;

		public TargetInformation (Transform transform, Vector2 velocity, bool moveBeforePlatform, bool reset) {
			this.transform          = transform         ;
			this.velocity           = velocity          ;
			this.moveBeforePlatform = moveBeforePlatform;
			this.reset              = reset             ;
		}

		public Transform GetTransform       { get { return transform         ; } set { transform          = value; } }
		public Vector2   Velocity           { get { return velocity          ; } set { velocity           = value; } }
		public bool      MoveBeforePlatform { get { return moveBeforePlatform; } set { moveBeforePlatform = value; } }
		public bool      Reset              { get { return reset             ; } set { reset              = value; } }
	}

	[SerializeField] Transform platform;

	List<TargetInformation> targetInformation;

	// Player is on top of the platform, so just move it across
	StatementInfo MovePassangers(ref Vector2 velocity, RayInfo rayInfo) {
		if (rayInfo.Hit) {
			if (!rayInfo.TargetsHit.Contains (rayInfo.Hit.transform) && rayInfo.Hit.distance != 0) {
				rayInfo.TargetsHit.Add (rayInfo.Hit.transform);
				float pushX = velocity.x;
				float pushY = velocity.y;

				targetInformation.Add(new TargetInformation(rayInfo.Hit.transform, new Vector2(pushX, pushY), (Mathf.Sign(velocity.y) == 1), reset:true));
			}
		}

		return StatementInfo.Continue;
	}

	// Since the above one has priority, this means the platform is pushing down the player
	StatementInfo VerticalRayFunction (ref Vector2 velocity, RayInfo rayInfo) {
		if (rayInfo.Hit) {
			if (!rayInfo.TargetsHit.Contains (rayInfo.Hit.transform) && rayInfo.Hit.distance != 0) {
				rayInfo.TargetsHit.Add (rayInfo.Hit.transform);
				float pushX = (rayInfo.Direction == 1) ? velocity.x : 0;
				float pushY = velocity.y- (rayInfo.Hit.distance - raycastShooter.GetColliderCorners.Inset) * rayInfo.Direction;

				targetInformation.Add(new TargetInformation(rayInfo.Hit.transform, new Vector2(pushX, pushY), moveBeforePlatform:false, reset:false));
			}
		}

		return StatementInfo.Continue; 
	}

	// Pushing the player to the left or right
	StatementInfo HorrizontalRayFunction (ref Vector2 velocity, RayInfo rayInfo) {
		if(rayInfo.Hit) {
			if (!rayInfo.TargetsHit.Contains (rayInfo.Hit.transform)) {
				rayInfo.TargetsHit.Add (rayInfo.Hit.transform);
				float pushX = velocity.x - (rayInfo.Hit.distance - raycastShooter.GetColliderCorners.Inset) * rayInfo.Direction;

				targetInformation.Add(new TargetInformation(rayInfo.Hit.transform, new Vector2(pushX, 0), moveBeforePlatform:false, reset:false));
			}
		}
		return StatementInfo.Continue;
	}

	// Based on what information we gathered, move the player
	void MovePassangers(bool whenToMove) {
		foreach (TargetInformation passsanger in targetInformation) {
			if (passsanger.MoveBeforePlatform == whenToMove) {
				ControllerBase targetController = passsanger.GetTransform.GetComponent<ControllerBase> ();
				if (passsanger.Reset) {
					targetController.SetVelocity ("Gravity", new Vector2 ());
					targetController.GetRaycastShooter.GetCollisionInfo.Below = true;
				}
				targetController.AddVelocity ("Platform", passsanger.Velocity * (1/Time.deltaTime));
			}
		}
	}

	public void Update() {
		Move (GetVelocity(), new Vector2(), null);
		ResetVelocity ();
	}

	public override void Move(Vector2 velocity, Vector2 input, StateInfo stateInfo) {
		raycastShooter.Reset ();

		targetInformation = new List<TargetInformation> ();

		// Always shoot rays upwards to check if there is something on the platform
		Vector2 upwardsVector = new Vector2(0, raycastShooter.GetColliderCorners.Inset * 2);
		raycastShooter.ShootVerticalRays (ref upwardsVector, Color.blue, upwardsVector.y, false, true, collisionMask, (rayInfo) => {
			return MovePassangers(ref velocity, rayInfo);
		});

		// If the player is upwards, this doesnt really matter, but need to check downwards if the platform is going down
		if (velocity.y != 0) {
			raycastShooter.ShootVerticalRays (ref velocity, Color.red, velocity.y, false, false, collisionMask, (rayInfo) => {
				return VerticalRayFunction(ref velocity, rayInfo);
			});
		}
			
		// Check if the platform is pushing something horizontally
		if (velocity.x != 0) {
			raycastShooter.ShootHorrizontalRays(ref velocity, Color.red, velocity.x, false, false, collisionMask, (rayInfo) => {
				return HorrizontalRayFunction(ref velocity, rayInfo);
			});
		}


		MovePassangers (true);
		platform.Translate (velocity);
		MovePassangers (false);
	}
}