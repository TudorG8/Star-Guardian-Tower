using System;

public class SerializableInt {
	[Serializable] int val;

	public int Value { get { return val; } set { val = value; } }
}