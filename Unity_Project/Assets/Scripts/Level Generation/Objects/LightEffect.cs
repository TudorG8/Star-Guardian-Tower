using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPropertyDrawers;
using ObjectInterfaces;

public class LightEffect : RoomObjectEffect, IStartable, IResetable {
	public GameObject light;

	public void OnStart() {
		light.SetActive (true );
	}

	public void OnReset() {
		light.SetActive (false);
	}
}