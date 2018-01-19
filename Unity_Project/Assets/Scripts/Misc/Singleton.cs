using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Extend this if you want to have a singleton class that can be called from anywhere
 * by saying <Name>.Instance.
 */
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
	static T instance;

	public static T Instance { get { return instance; } }

	// Should be called by children in their awake method.
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