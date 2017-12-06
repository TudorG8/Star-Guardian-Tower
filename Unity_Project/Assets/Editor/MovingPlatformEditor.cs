using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(MovingPlatform))]
public class MovingPlatformEditor : Editor {
	public override void OnInspectorGUI () {
		DrawDefaultInspector ();
		MovingPlatform script = (MovingPlatform)target;
		if (GUILayout.Button ("Reverse")) {
			script.Reverse ();
		}
	}
}
