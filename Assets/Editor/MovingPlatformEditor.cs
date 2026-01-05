using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MovingObject))]
public class MovingPlatformEditor : Editor {
	public override void OnInspectorGUI () {
		DrawDefaultInspector ();
		MovingObject script = (MovingObject)target;
		if (GUILayout.Button ("Reverse")) {
			script.Reverse ();
		}
	}
}
