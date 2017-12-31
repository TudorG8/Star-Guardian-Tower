using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerScript : MonoBehaviour {
	public bool triggered;
	public UnityEvent eventToCall;
	void OnTriggerEnter2D(Collider2D other) {
		if (!triggered) {
			eventToCall.Invoke ();
			triggered = true;
		}
	}
}
