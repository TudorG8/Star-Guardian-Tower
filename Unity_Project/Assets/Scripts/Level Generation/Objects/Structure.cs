using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectInterfaces;

/**
 * A Structure is an object that can be destroyed by being attacked.
 */
public class Structure : RoomObject, IResetable, IDestroyable {
	delegate void Action();

	[SerializeField] Animator  animator;
	[SerializeField] Collider2D coll;
	[SerializeField] LayerMask collisionMask;

	void OnTriggerEnter2D(Collider2D other) {
		if (UsefulMethods.IsRightLayer(collisionMask, other.gameObject.layer)) {
			StartCoroutine (WaitForSeconds (0.2f, () => { OnDestroy(); }));
		}
	}

	IEnumerator WaitForSeconds (float time, Action action) {
		yield return new WaitForSeconds (time);
		action ();
	}

	public void OnReset  () {
		animator  .SetTrigger ("reset");
		coll.enabled = true;
	}

	public void OnDestroy() {
		coll.enabled = false;
		animator.SetTrigger ("fade");
	}
}
