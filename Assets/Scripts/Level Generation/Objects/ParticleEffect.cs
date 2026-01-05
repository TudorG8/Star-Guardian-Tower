using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPropertyDrawers;
using ObjectInterfaces;

public class ParticleEffect : RoomObjectEffect, IStartable, IResetable {
	public ParticleSystem particles;

	public void OnStart() {
		particles.Play ();
	}

	public void OnReset() {
		particles.Stop ();
	}
}