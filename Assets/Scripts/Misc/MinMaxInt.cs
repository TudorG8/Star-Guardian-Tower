using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Holds a minimum and maximum value and allows getting a random value
 * from between them.
 */
[System.Serializable]
public class MinMaxInt {
	[SerializeField] int min;
	[SerializeField] int max;

	public int Min { get { return min; } }
	public int Max { get { return max; } }

	public int GetRandomValue() {
		if (min > max) return max;

		return Random.Range (min, max + 1);
	}
}