using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomArrayList <T> {
	public List<T> list;

	public CustomArrayList() {
		list = new List<T>();
	}
}