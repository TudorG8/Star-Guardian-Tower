using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * An entity that receives input (even if is simulated)
 */

public interface InputableEntity {
	Vector2   GetInput     (); // Gets the input for the entity
	StateInfo GetStateInfo (); // State Info contains a bunch of actions
	void      AfterMove    (); // What should happen after the entity moves
}