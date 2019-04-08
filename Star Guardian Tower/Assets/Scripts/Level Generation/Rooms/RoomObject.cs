using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectInterfaces;

/**
 * Base class for all objects in the rooms.
 * Hoping that one Unity introduces a GetInterface<T> so I wouldn't have to make empty objects
 */

[System.Serializable]
public class RoomObject : MonoBehaviour {
	[SerializeField] protected List<RoomObjectEffect> effects;

	public void ResetEffects() {
		for (int i = 0; i < effects.Count; i++) {
			if (effects [i] is IResetable) {
				IResetable resetableEffect = (IResetable)effects [i];
				resetableEffect.OnReset ();
			}
		}
	}

	public void StartEffects() {
		for (int i = 0; i < effects.Count; i++) {
			if (effects [i] is IStartable) {
				IStartable startableObject = (IStartable)effects [i];
				startableObject.OnStart ();
			}
		}
	}
}