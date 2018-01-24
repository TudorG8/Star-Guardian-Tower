using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
	[SerializeField] Animator animator ;
	[SerializeField] Collider2D coll;
	[SerializeField] Vector2  direction;
	[SerializeField] float    speed    ;

	public void SetUp(Vector2 direction) {
		this.direction = direction;

		//transform.rotation = Quaternion.LookRotation (direction, Vector3.up);
		StartCoroutine (MoveRoutine ());
	}

	public void OnLevelCollision() {
		StopAllCoroutines ();
		coll.enabled = false;
		animator.SetTrigger ("fade");
	}

	public void Destroy() {
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