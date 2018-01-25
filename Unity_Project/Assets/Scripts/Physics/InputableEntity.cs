using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface InputableEntity {
	Vector2 GetInput();
	PlayerController.StateInfo GetStateInfo ();
	void AfterMove();
}