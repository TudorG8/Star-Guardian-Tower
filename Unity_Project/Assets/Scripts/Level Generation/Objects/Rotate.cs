using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectInterfaces;

/**
 * Rotate an object effect thing.
 */

public class Rotate : RoomObjectEffect, IStartable, IResetable {
	public enum Direction { Clockwise, Anticlockwise }

	[SerializeField] Transform obj;

	[SerializeField] float     timeToSpinOnce;
	[SerializeField] Vector3   degrees       ;
	[SerializeField] Direction direction     ;

	[SerializeField] bool      active        ;

	void Update() {
		if (active) {
			int dir = (direction == Direction.Clockwise ? 1 : -1);
			transform.Rotate (dir * degrees * Time.deltaTime);
		}
	}

	public void OnStart() {
		active = true;
	}

	public void OnReset() {
		active = false;
	}
}