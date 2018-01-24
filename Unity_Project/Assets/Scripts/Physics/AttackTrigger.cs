using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTrigger : MonoBehaviour {
	[SerializeField] Animator animator;
	[SerializeField] Collider2D attachedCollider;

	[SerializeField] float initialDelay;

	[SerializeField] int framesActive = 1;

	public void SetUp(ShopItem item) {
		// Set the mesh renderer
		// Set the range of the slash
	}

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
