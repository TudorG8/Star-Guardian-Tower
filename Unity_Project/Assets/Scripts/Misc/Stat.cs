using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Used to hold a value while provisind a minimum and maximum value around it.
 * Should be used for stats. Can be extended to add modifiers (probably not anytime soon).
 */

public class Stat {
	[SerializeField] float current;
	[SerializeField] float min    ;
	[SerializeField] float max    ;

	public float Value { 
		get { return current; } 
		set { 
			current += value;
			current = Mathf.Clamp (current, min, max);
		}
	}
	public float Min { get { return min; } set { min = value; } }
	public float Max { get { return max; } set { max = value; } }
}