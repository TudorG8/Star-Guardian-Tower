using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPropertyDrawers;

/**
 * Used to slowly change the value of an Serializable Integer.
 * The actual value will instantly be set, but the current value 
 * and difference will slowly move to that amount over a given period.
 */

public class ValueChanger : MonoBehaviour {
	// Settings
	[SerializeField] float timeTaken;

	// Read Only
	[SerializeField][ReadOnly] int currentAmount;
	[SerializeField][ReadOnly] int difference   ;

	// Properties
	public int CurrentAmount { get { return currentAmount; } }
	public int Difference    { get { return difference   ; } }

	// Methods
	/**
	 * Should be called to set up the value changer so it knows what value to use.
	 */
	public void SetUp(SerializableInt totalAmount) {
		this.currentAmount = totalAmount.Value;
	}

	/**
	 * Should be called to gain or remove an amount from a given serializable int.
	 */
	public void GainAmount (SerializableInt totalAmount, int amount) {
		totalAmount.Value += amount;
		difference = totalAmount.Value - currentAmount;

		StopAllCoroutines ();
		StartCoroutine (IncreaseOverTime (timeTaken, totalAmount));
	}

	public string GetDifferenceSign() {
		if      (difference >  0) { return "+"; } 
		else if (difference <  0) { return "-"; } 
		else   /*difference == 0*/{ return  ""; }
	}


	/**
	 * Routine for the GainAmount method.
	 */
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
	}
}