using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ControllerBase : MonoBehaviour {
	/**
	 * This method is supposed to move the player when there is no input present.
	 */
	public virtual void Move(Vector2 velocity) {
		Move (velocity, new Vector2 ());
	}

	/**
	 * This method is supposed to move the player. 
	 * Core method that must be implemented by any children.
	 */
	public abstract void Move(Vector2 velocity, Vector2 input);
}
