using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : ControllerBase {
	[SerializeField] Transform platform;

	List<PassangerMovement> passangerMovement;

	StatementInfo MovePassangers(ref Vector2 velocity, RayInfo rayInfo) {
		if (rayInfo.hit) {
			if (!rayInfo.targetsHit.Contains (rayInfo.hit.transform) && rayInfo.hit.distance != 0) {
				rayInfo.targetsHit.Add (rayInfo.hit.transform);
				float pushX = velocity.x;
				float pushY = velocity.y;

				passangerMovement.Add(new PassangerMovement(rayInfo.hit.transform, new Vector2(pushX, pushY), (Mathf.Sign(velocity.y) == 1), true));
			}
		}

		return StatementInfo.Continue;
	}

	StatementInfo VerticalRayFunction (ref Vector2 velocity, RayInfo rayInfo) {
		if (rayInfo.hit) {
			if (!rayInfo.targetsHit.Contains (rayInfo.hit.transform) && rayInfo.hit.distance != 0) {
				rayInfo.targetsHit.Add (rayInfo.hit.transform);
				float pushX = (rayInfo.direction == 1) ? velocity.x : 0;
				float pushY = velocity.y- (rayInfo.hit.distance - raycastShooter.GetColliderCorners.Inset) * rayInfo.direction;

				passangerMovement.Add(new PassangerMovement(rayInfo.hit.transform, new Vector2(pushX, pushY), moveBeforePlatform:false));
			}
		}

		return StatementInfo.Continue; 
	}


	StatementInfo HorrizontalRayFunction (ref Vector2 velocity, RayInfo rayInfo) {
		if(rayInfo.hit) {
			if (!rayInfo.targetsHit.Contains (rayInfo.hit.transform)) {
				rayInfo.targetsHit.Add (rayInfo.hit.transform);
				float pushX = velocity.x - (rayInfo.hit.distance - raycastShooter.GetColliderCorners.Inset) * rayInfo.direction;
				//float pushY = - raycastShooter.GetColliderCorners.Inset;

				passangerMovement.Add(new PassangerMovement(rayInfo.hit.transform, new Vector2(pushX, 0), moveBeforePlatform:false));
			}
		}
		return StatementInfo.Continue;
	}

	void MovePassangers(bool beforeMovePlatform) {
		foreach (PassangerMovement passsanger in passangerMovement) {
			if (passsanger.moveBeforePlatform == beforeMovePlatform) {
				ControllerBase targetController = passsanger.transform.GetComponent<ControllerBase> ();
				if (passsanger.reset) {
					targetController.SetVelocity ("Gravity", new Vector2 ());
					targetController.GetRaycastShooter.GetCollisionInfo.below = true;
				}
				targetController.AddVelocity ("Platform", passsanger.velocity * (1/Time.deltaTime));
			}
		}
	}

	public void Update() {
		Move (GetVelocity(), new Vector2(), null);
		ResetVelocity ();
	}

	public override void Move(Vector2 velocity, Vector2 input, PlayerController.StateInfo stateInfo) {
		raycastShooter.Reset ();

		passangerMovement = new List<PassangerMovement> ();

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


		MovePassangers (beforeMovePlatform:true);
		platform.Translate (velocity);
		MovePassangers (beforeMovePlatform:false);
	}

	struct PassangerMovement {
		public Transform transform;
		public Vector2 velocity;
		public bool moveBeforePlatform;
		public bool reset;

		public PassangerMovement(Transform transform, Vector2 velocity, bool moveBeforePlatform, bool reset = false) {
			this.transform = transform;
			this.velocity = velocity;
			this.moveBeforePlatform = moveBeforePlatform;
			this.reset = reset;
		}
	}
}