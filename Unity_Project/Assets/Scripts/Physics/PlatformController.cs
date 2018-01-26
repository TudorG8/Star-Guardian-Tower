using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : ControllerBase {
	[SerializeField] Transform platform;

	List<TargetInformation> targetInformation;

	StatementInfo MovePassangers(ref Vector2 velocity, RayInfo rayInfo) {
		if (rayInfo.hit) {
			if (!rayInfo.targetsHit.Contains (rayInfo.hit.transform) && rayInfo.hit.distance != 0) {
				rayInfo.targetsHit.Add (rayInfo.hit.transform);
				float pushX = velocity.x;
				float pushY = velocity.y;

				targetInformation.Add(new TargetInformation(rayInfo.hit.transform, new Vector2(pushX, pushY), (Mathf.Sign(velocity.y) == 1), true));
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

				targetInformation.Add(new TargetInformation(rayInfo.hit.transform, new Vector2(pushX, pushY), moveBeforePlatform:false));
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

				targetInformation.Add(new TargetInformation(rayInfo.hit.transform, new Vector2(pushX, 0), moveBeforePlatform:false));
			}
		}
		return StatementInfo.Continue;
	}

	void MovePassangers(bool beforeMovePlatform) {
		foreach (TargetInformation passsanger in targetInformation) {
			if (passsanger.MoveBeforePlatform == beforeMovePlatform) {
				ControllerBase targetController = passsanger.GetTransform.GetComponent<ControllerBase> ();
				if (passsanger.Reset) {
					targetController.SetVelocity ("Gravity", new Vector2 ());
					targetController.GetRaycastShooter.GetCollisionInfo.below = true;
				}
				targetController.AddVelocity ("Platform", passsanger.Velocity * (1/Time.deltaTime));
			}
		}
	}

	public void Update() {
		Move (GetVelocity(), new Vector2(), null);
		ResetVelocity ();
	}

	public override void Move(Vector2 velocity, Vector2 input, PlayerController.StateInfo stateInfo) {
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


		MovePassangers (beforeMovePlatform:true);
		platform.Translate (velocity);
		MovePassangers (beforeMovePlatform:false);
	}

	class TargetInformation {
		[SerializeField] Transform transform;
		[SerializeField] Vector2 velocity;
		[SerializeField] bool moveBeforePlatform;
		[SerializeField] bool reset;

		public TargetInformation (Transform transform, Vector2 velocity, bool moveBeforePlatform, bool reset = false) {
			this.transform = transform;
			this.velocity = velocity;
			this.moveBeforePlatform = moveBeforePlatform;
			this.reset = reset;
		}

		public Transform GetTransform       { get { return transform         ; } set { transform          = value; } }
		public Vector2   Velocity           { get { return velocity          ; } set { velocity           = value; } }
		public bool      MoveBeforePlatform { get { return moveBeforePlatform; } set { moveBeforePlatform = value; } }
		public bool      Reset              { get { return reset             ; } set { reset              = value; } }
	}
}