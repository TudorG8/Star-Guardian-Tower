using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Wrapper for integers to make them reference objects.
 */
[System.Serializable]
public class SerializableInt {
	[SerializeField] int val;

	public int Value { get { return val; } set { val = value; } }
}