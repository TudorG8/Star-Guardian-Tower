using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectInterfaces;

/**
 * A Gold Pickup is an object that gives gold when picked up.
 */
public class GoldPickup : RoomObject, IPickable, IResetable {
	[SerializeField] Animator      animator     ;
	[SerializeField] TriggerScript triggerScript;

	[SerializeField] MinMaxInt goldGained  ;
	[SerializeField] int       scorePerGold;

	public void OnPickup () {
		animator.SetTrigger ("pick");
		int reward = goldGained.GetRandomValue ();
		ScoreSystem.Instance.GainGold  (reward);
		ScoreSystem.Instance.GainScore (reward * scorePerGold);
	}
		
	public void Reset() {
		if (triggerScript.Triggered) {
			animator.SetTrigger ("reset");
			triggerScript.Reset ();
		}
	}
}