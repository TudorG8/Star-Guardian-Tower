using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
	static T instance;

	public static T Instance { get { return instance; } }

	protected void InitiateSingleton() {
		if (instance == null) {
			instance = this as T;
		} 
		else {
			DestroyImmediate (this);
			Debug.LogError ("Attempted to create another instance of " + this + " when it is a singleton. New Object Deleted.");
		}
	}
}