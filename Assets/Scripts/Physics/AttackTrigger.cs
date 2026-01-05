using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Used to attack from the player.
 */

public class AttackTrigger : MonoBehaviour {
	// Imports
	[SerializeField] Animator   animator        ;
	[SerializeField] Collider2D attachedCollider;

	// Options
	[SerializeField] float initialDelay;
	[SerializeField] int   framesActive;


	void Start() {
		attachedCollider.enabled = false;
	}
		
	public void Attack() {
		StartCoroutine (AttackTime ());
	}

	IEnumerator AttackTime() {
		yield return new WaitForSeconds (initialDelay);
		animator.SetTrigger ("attack");
		attachedCollider.enabled = true ;
		for (int i = 0; i < framesActive; i++) {
			yield return new WaitForEndOfFrame ();
		}
		attachedCollider.enabled = false;
	}
}
