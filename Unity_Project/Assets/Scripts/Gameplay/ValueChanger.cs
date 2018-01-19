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

	public void SetUp(SerializableInt totalAmount) {
		this.currentAmount = totalAmount.Value;
	}

	public void GainAmount (SerializableInt totalAmount, int amount) {
		totalAmount.Value += amount;
		difference = totalAmount.Value - currentAmount;

		StopAllCoroutines ();
		StartCoroutine (IncreaseOverTime (2f, totalAmount));
	}
	IEnumerator IncreaseOverTime(float time, SerializableInt target) {
		int initialValue = currentAmount;
		float currentTime = 0f;
		while (currentTime < time) {
			float progress = currentTime / time;
			currentAmount = (int)Mathf.Lerp (initialValue, target.Value, progress);
			difference    = target.Value - currentAmount;

			currentTime += Time.deltaTime;

			yield return null;
		}
			
		currentAmount = target.Value;
		difference    = 0;
		Debug.Log (difference);
	}
}