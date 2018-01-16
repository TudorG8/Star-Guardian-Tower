using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerializableInt {
	[SerializeField] int val;

	public int Value { get { return val; } set { val = value; } }
}