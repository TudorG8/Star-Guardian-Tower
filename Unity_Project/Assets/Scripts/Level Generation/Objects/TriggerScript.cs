using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerScript : MonoBehaviour {
	[SerializeField] LayerMask collisionMask;
	[SerializeField] bool  retriggable;
	[SerializeField] bool  triggered  ;
	[SerializeField] float delay      ;
	[SerializeField] UnityEvent eventToCall;

	public bool Triggered { get { return triggered; } set { triggered = value; } }

	void OnTriggerEnter2D(Collider2D other) {
		if (collisionMask == (collisionMask | (1 << other.gameObject.layer))) {
			if (!triggered) {
				eventToCall.Invoke ();
				triggered = true;
				if (retriggable) {
					StartCoroutine (Wait (delay));
				}
			}
		}
	}

	IEnumerator Wait(float delay) {
		yield return new WaitForSeconds (delay);
		triggered = false;
	}
}
