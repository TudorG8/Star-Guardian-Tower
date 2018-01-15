using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DirectionTriggerScript : MonoBehaviour {
	[SerializeField] LayerMask collisionMask;
	[SerializeField] bool  retriggable;
	[SerializeField] bool  triggered  ;
	[SerializeField] float delay      ;
	[SerializeField] Direction currentDirection;
	[SerializeField] UnityEvent leftEvent ;
	[SerializeField] UnityEvent rightEvent;

	public bool Triggered { get { return triggered; } set { triggered = value; } }

	void OnTriggerEnter2D(Collider2D other) {
		if (CorrectTarget (other)) {
			float difference = this.transform.position.x - other.transform.position.x;
			currentDirection = difference > 0 ? Direction.Left : Direction.Right;

			UnityEvent eventToCall = currentDirection == Direction.Left ? leftEvent : rightEvent;

			OnTrigger (eventToCall);
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if(CorrectTarget(other)) {
			float difference = this.transform.position.x - other.transform.position.x;
			Direction newDirection = difference > 0 ? Direction.Left : Direction.Right;

			// If we came in, but then came out right away
			if (newDirection == currentDirection) {
				UnityEvent eventToCall = newDirection == Direction.Left ? rightEvent : leftEvent;
				currentDirection = newDirection;
				OnTrigger (eventToCall);
			}
		}
	}

	void OnTrigger(UnityEvent eventToCall) {
		if (!triggered) {
			eventToCall.Invoke ();
			triggered = true;
			if (retriggable) {
				StartCoroutine (Wait (delay));
			}
		}
	}

	bool CorrectTarget(Collider2D other) {
		return collisionMask == (collisionMask | (1 << other.gameObject.layer));
	}

	IEnumerator Wait(float delay) {
		yield return new WaitForSeconds (delay);
		triggered = false;
	}
}
