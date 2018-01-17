using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueChanger : MonoBehaviour {
	[SerializeField] int fastThreshold;
	[SerializeField] int slowSpeed    ;
	[SerializeField] int fastSpeed    ;

	[SerializeField] int currentAmount;
	[SerializeField] int difference   ;

	public int CurrentAmount { get { return currentAmount; } }
	public int Difference    { get { return difference   ; } }

	public void GainAmount (SerializableInt totalAmount, int amount) {
		totalAmount.Value += amount;
		difference = SessionData.Instance.CurrentScore - currentAmount;

		int instancesRequired = (difference >= fastThreshold) ? fastSpeed : slowSpeed;

		StopAllCoroutines ();
		for (int i = 0; i < instancesRequired; i++) {
			StartCoroutine(ScoreRoutine(0.1f));
		}
	}
	IEnumerator ScoreRoutine(float speed, int amount) {
		while (currentAmount < SessionData.Instance.CurrentScore) {
			currentAmount += amount;
			difference    -= amount;
			yield return new WaitForSeconds (speed);
		}
	}
}