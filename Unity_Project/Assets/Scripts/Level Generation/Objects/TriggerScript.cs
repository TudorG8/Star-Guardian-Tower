using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using CustomPropertyDrawers;

public class TriggerScript : RoomObject, IResetable {
	[SerializeField] LayerMask collisionMask;
	[SerializeField] bool  retriggable ; // If the script can be retriggered by walking back into it
	[SerializeField] float initialDelay; // The delay used when something steps in for the first time
	[SerializeField] bool  continuous  ; // If the script will retrigger while something is in it
	[SerializeField] float delay       ; // The delay used when retriggering continuous events
	[SerializeField] UnityEvent eventToCall;

	[SerializeField][ReadOnly] bool triggered  ;

	public delegate void FunctionCall ();

	public bool Triggered { get { return triggered; } }

	void OnTriggerEnter2D(Collider2D other) {
		if (UsefulMethods.IsRightLayer(collisionMask, other.gameObject.layer)) {
			if (!triggered) {
				TriggerEvent (initialDelay);
			}
		}
	}

	void OnTriggerStay2D(Collider2D other) {
		if (continuous) {
			if (UsefulMethods.IsRightLayer(collisionMask, other.gameObject.layer)) {
				if (!triggered) {
					TriggerEvent (delay);
				}
			}
		}
	}

	void TriggerEvent(float delay) {
		triggered = true;
		StartCoroutine(TriggerActionAfterDelay(delay, () => {
			eventToCall.Invoke ();
			if (retriggable) {
				triggered = false;
			}
		}));
	}

	IEnumerator TriggerActionAfterDelay(float delay, FunctionCall action) {
		yield return new WaitForSeconds (delay);
		action ();
	}

	public void Reset() {
		StopAllCoroutines ();
		triggered = false;
	}
}
