using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectInterfaces;

public class ProjectileDispenser : RoomObject, IResetable {
	[SerializeField] Transform  shootingPoint;
	[SerializeField] TriggerScript triggerScript;
	[SerializeField] GameObject projectile;
	[SerializeField] float delay;
	[SerializeField] Direction  direction ;

	public void Shoot() {
		GameObject projObj = Instantiate (projectile, shootingPoint.position, shootingPoint.rotation) as GameObject;
		projObj.transform.SetParent (this.transform);

		Projectile projScript = projObj.GetComponent<Projectile> ();
		projScript.SetUp (DirectionHelper.GetDirectionVector (direction));
	}

	public void AutomaticShooting(float initialDelay) {
		StartCoroutine (ShootingRoutine (initialDelay));
	}

	public void StopShooting() {
		StopAllCoroutines ();
	}

	IEnumerator ShootingRoutine(float initialDelay) {
		yield return new WaitForSeconds (initialDelay);
		while (true) {
			Shoot ();
			yield return new WaitForSeconds (delay);
		}
	}

	public void Reset() {
		if (triggerScript != null)
			triggerScript.Reset ();
		StopShooting ();
	}
}