using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldPickup : MonoBehaviour {
	[SerializeField] Animator  animator    ;

	[SerializeField] MinMaxInt goldGained  ;
	[SerializeField] int       scorePerGold;

	public void OnPickup () {
		animator.SetTrigger ("dissapear");
		int reward = goldGained.GetRandomValue ();
		ScoreSystem.Instance.GainGold  (reward);
		ScoreSystem.Instance.GainScore (reward * scorePerGold);
	}
}