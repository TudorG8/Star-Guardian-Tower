using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : ControllerBase {
	public Transform      platform      ;



	List<PassangerMovement> passangerMovement;

	StatementInfo MovePassangers(ref Vector2 velocity, RayInfo rayInfo) {
		if (rayInfo.hit) {
			Debug.Log (rayInfo.hit.distance);
			if (!rayInfo.targetsHit.Contains (rayInfo.hit.transform) && rayInfo.hit.distance != 0) {
				rayInfo.targetsHit.Add (rayInfo.hit.transform);
				float pushX = velocity.x;
				float pushY = velocity.y;

				passangerMovement.Add(new PassangerMovement(rayInfo.hit.transform, new Vector2(pushX, pushY), moveBeforePlatform:Mathf.Sign(velocity.y) == 1));
			}
		}

		return StatementInfo.Continue;
	}

	StatementInfo VerticalMove(ref Vector2 velocity, RayInfo rayInfo) {
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

	void MovePassangers(bool beforeMovePlatform) {
		foreach (PassangerMovement passsanger in passangerMovement) {
			if (passsanger.moveBeforePlatform == beforeMovePlatform) {
				ControllerBase targetController = passsanger.transform.GetComponent<ControllerBase> ();
				targetController.Move (passsanger.velocity, null);
			}
		}
	}

	public override void Move(Vector2 velocity, Vector2 input, PlayerController.StateInfo stateInfo) {
		raycastShooter.Reset ();

		passangerMovement = new List<PassangerMovement> ();

		Vector2 upwardsVector = new Vector2(0, raycastShooter.GetColliderCorners.Inset * 2);
		raycastShooter.ShootVerticalRays (ref upwardsVector, Color.blue, upwardsVector.y, false, true, collisionMask, (rayInfo) => {
			return MovePassangers(ref velocity, rayInfo);
		});

		if (velocity.y != 0) {
			raycastShooter.ShootVerticalRays (ref velocity, Color.red, velocity.y, false, false, collisionMask, (rayInfo) => {
				return VerticalMove(ref velocity, rayInfo);
			});
		}
			
		if (velocity.x != 0) {
			raycastShooter.ShootHorrizontalRays(ref velocity, Color.red, velocity.x, false, false, collisionMask, (rayInfo) => {
				if(rayInfo.hit) {
					if (!rayInfo.targetsHit.Contains (rayInfo.hit.transform)) {
						rayInfo.targetsHit.Add (rayInfo.hit.transform);
						float pushX = velocity.x - (rayInfo.hit.distance - raycastShooter.GetColliderCorners.Inset) * rayInfo.direction;
						//float pushY = - raycastShooter.GetColliderCorners.Inset;

						passangerMovement.Add(new PassangerMovement(rayInfo.hit.transform, new Vector2(pushX, 0), moveBeforePlatform:false));
					}
				}
				return StatementInfo.Continue;
			});
		}

			
		MovePassangers (true);
		platform.Translate (velocity);
		MovePassangers (false);
	}

	void CalculatePassengerMovement(Vector2 velocity) {
		
	}

	struct PassangerMovement {
		public Transform transform;
		public Vector2 velocity;
		public bool moveBeforePlatform;

		public PassangerMovement(Transform transform, Vector2 velocity, bool moveBeforePlatform) {
			this.transform = transform;
			this.velocity = velocity;
			this.moveBeforePlatform = moveBeforePlatform;
		}
	}
}
/*
 * if (velocity.x != 0) {	
			raycastShooter.ShootHorrizontalRays (ref velocity, Color.red, velocity.x, (rayInfo) =>  {
				return MovePassangers(ref velocity, rayInfo);
			});
		}
 */