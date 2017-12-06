using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTrigger : MonoBehaviour {
	public Animator animator;
	public Collider2D attachedCollider;

	public LayerMask collisionMask;

	public int framesActive = 1;
	public int pushback = 1;

	Vector2 direction;

	void Start() {
		attachedCollider.enabled = false;
	}
		
	public void Attack(Vector2 direction) {
		this.direction = direction;
		animator.SetTrigger ("attack");
		StartCoroutine (AttackTime ());
	}

	IEnumerator AttackTime() {
		attachedCollider.enabled = true ;
		for (int i = 0; i < framesActive; i++) {
			yield return new WaitForEndOfFrame ();
		}
		attachedCollider.enabled = false;
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (collisionMask == (collisionMask | (1 << other.gameObject.layer))) {
			other.GetComponent<Rigidbody2D> ().AddForce (direction * pushback);
			Destroy (other.gameObject, 0.5f);
		}
	}
}
