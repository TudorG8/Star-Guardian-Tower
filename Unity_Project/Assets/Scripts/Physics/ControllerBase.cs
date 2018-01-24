using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ControllerBase : MonoBehaviour {
	[SerializeField] protected RaycastShooter raycastShooter;
	[SerializeField] protected LayerMask      collisionMask ;
	/**
	 * This method is supposed to move the player when there is no input present.
	 */
	public virtual void Move(Vector2 velocity, PlayerController.StateInfo stateInfo) {
		Move (velocity, new Vector2 (), stateInfo);
	}

	/**
	 * This method is supposed to move the player. 
	 * Core method that must be implemented by any children.
	 */
	public abstract void Move(Vector2 velocity, Vector2 input, PlayerController.StateInfo stateInfo);

	public CollisionInfo GetCollisionInfo {
		get {
		return raycastShooter.GetCollisionInfo;
		}
	}

	public RaycastShooter GetRaycastShooter { get { return raycastShooter; } }
}
