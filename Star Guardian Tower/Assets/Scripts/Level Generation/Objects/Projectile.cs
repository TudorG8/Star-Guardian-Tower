using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectInterfaces;

/**
 * A projectile is a runtime object created by object spawners.
 * Ideally, I would introduce a cache for this
 */

public class Projectile : RoomObject, IResetable {
	[SerializeField] Animator animator ;
	[SerializeField] Collider2D coll;
	[SerializeField] Vector2  direction;
	[SerializeField] float    speed    ;
	[SerializeField] OnProjectileDeath onProjectileDeath;

	public delegate void OnProjectileDeath (Projectile projectile);

	public void SetUp(Vector2 direction, OnProjectileDeath onProjectileDeath) {
		this.direction = direction;
		this.onProjectileDeath = onProjectileDeath;

		StartCoroutine (MoveRoutine ());
		StartEffects ();
	}

	public void OnLevelCollision() {
		StopAllCoroutines ();
		coll.enabled = false;
		animator.SetTrigger ("fade");
		onProjectileDeath (this);
		ResetEffects ();
	}

	public void OnReset () {
		Destroy (gameObject);
	}

	IEnumerator MoveRoutine() {
		while (true) {
			Vector2 position = transform.position;
			position += direction * speed * Time.deltaTime;
			transform.position = position;
			yield return new WaitForEndOfFrame ();
		}
	}
}