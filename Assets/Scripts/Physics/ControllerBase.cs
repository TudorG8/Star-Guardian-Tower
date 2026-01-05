using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Base for all controllers, with everything they share
 */

public abstract class ControllerBase : MonoBehaviour {
	// Imports
	[SerializeField] protected RaycastShooter raycastShooter;
	[SerializeField] protected LayerMask      collisionMask ;

	[SerializeField] protected Dictionary<string, Vector2>  velocities = new Dictionary<string, Vector2>();

	// Properties ------------------------------------------------------------------------------------------------
	public CollisionInfo  GetCollisionInfo  { get { return raycastShooter.GetCollisionInfo;}}
	public RaycastShooter GetRaycastShooter { get { return raycastShooter; } }

	// Velocities ------------------------------------------------------------------------------------------------
	public Vector2 GetSingleVelocity(string name) {
		return velocities [name];
	}
	public bool HasVelocity(string name) {
		return velocities.ContainsKey (name);
	}

	public void AddVelocity(string name, Vector2 velocity) {
		velocities[name] = velocity;
	}
	public Vector2 GetVelocity() {
		Vector2 returnVelocity = new Vector2 ();
		foreach (KeyValuePair<string, Vector2> velocity in velocities) {
			returnVelocity += velocity.Value;
		}
		return returnVelocity;
	}

	public void SetVelocity(string name, Vector2 velocity) {
		if (velocities.ContainsKey (name)) {
			velocities [name] = velocity;
		}
	}

	public void ResetVelocity() {
		velocities = new Dictionary<string, Vector2> ();
	}

	public void PrintVelocities() {
		Debug.Log ("------------");
		foreach (KeyValuePair<string, Vector2> velocity in velocities) {
			Debug.Log (velocity.Key + " " + velocity.Value);
		}
		Debug.Log ("------------");
	}
	// Input -----------------------------------------------------------------------------------------------------
	/**
	 * This method is supposed to move the player when there is no input present.
	 */
	public virtual void Move(Vector2 velocity, StateInfo stateInfo) {
		Move (velocity, new Vector2 (), stateInfo);
	}

	/**
	 * This method is supposed to move the player. 
	 * Core method that must be implemented by any children.
	 */
	public abstract void Move(Vector2 velocity, Vector2 input, StateInfo stateInfo);
}
// ---------------------------------------------------------------------------------------------------------------