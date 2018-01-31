using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectInterfaces;
using CustomPropertyDrawers;

/**
 * A projectile dispenser creates projectiles based on how it is configured.
 */

public class ProjectileDispenser : RoomObject, IStartable, IResetable {
	// Imports
	[SerializeField] Transform  shootingPoint;
	[SerializeField] GameObject projectile   ;

	// Settings
	[SerializeField] float      delay                 ;
	[SerializeField] Direction  direction             ;
	[SerializeField] bool       startMovingOnRoomEnter;

	// Read only
	[SerializeField] List<Projectile> projectiles;
	[SerializeField][ReadOnly] bool   hasStarted ;

	// To be called by projectiles when they die
	public void RemoveProj (Projectile projectile) {
		projectiles.Remove (projectile);
	}

	public void Shoot() {
		GameObject projObj = Instantiate (projectile, shootingPoint.position, shootingPoint.rotation) as GameObject;
		projObj.transform.SetParent (this.transform);

		Projectile projScript = projObj.GetComponent<Projectile> ();
		projScript.SetUp (DirectionHelper.GetDirectionVector (direction), RemoveProj);

		projectiles.Add (projScript);
	}

	public void AutomaticShooting(float initialDelay) {
		StartCoroutine (ShootingRoutine (initialDelay));
	}

	IEnumerator ShootingRoutine(float initialDelay) {
		yield return new WaitForSeconds (initialDelay);
		while (hasStarted) {
			Shoot ();
			yield return new WaitForSeconds (delay);
		}
	}

	public void OnStart() {
		StartEffects ();
	}

	public void OnReset() {
		for (int i = 0; i < projectiles.Count; i++) {
			Destroy (projectiles [i]);
		}
		projectiles = new List<Projectile> ();
		StopAllCoroutines ();
		ResetEffects ();
	}
}