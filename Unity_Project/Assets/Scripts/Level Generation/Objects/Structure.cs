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
	[SerializeField] LayerMask collisionMask;

	void OnTriggerEnter2D(Collider2D other) {
		if (collisionMask == (collisionMask | (1 << other.gameObject.layer))) {
			animator.SetTrigger ("destroy");
			StartCoroutine (WaitForSeconds (0.2f, () => { Destroy(); }));
		}
	}

	IEnumerator WaitForSeconds (float time, Action action) {
		yield return new WaitForSeconds (time);
		action ();
	}

	public void Reset  () {
		animator  .SetTrigger ("reset");
		gameObject.SetActive  (true   );
	}

	public void Destroy() {
		gameObject.SetActive (false);
	}
}
