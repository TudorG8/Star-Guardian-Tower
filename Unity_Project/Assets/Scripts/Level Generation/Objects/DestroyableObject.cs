using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableObject : MonoBehaviour {
	public LayerMask collisionMask;

	void OnTriggerEnter2D(Collider2D other) {
		if (collisionMask == (collisionMask | (1 << other.gameObject.layer))) {
			Destroy (gameObject, 0.2f);
		}
	}
}
