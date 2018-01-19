using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Generic class to hold a list of something.
 * Extend it and specify a non generic type to enable serialization.
 */
[System.Serializable]
public class CustomArrayList <T> {
	public List<T> list;

	public CustomArrayList() {
		list = new List<T>();
	}
}