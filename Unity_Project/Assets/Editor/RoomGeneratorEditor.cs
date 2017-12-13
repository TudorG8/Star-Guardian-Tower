using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoomGenerator))]
public class RoomGeneratorEditor : Editor {
	public override void OnInspectorGUI () {
		DrawDefaultInspector ();
		RoomGenerator script = (RoomGenerator)target;
		if (GUILayout.Button ("Clear")) {
			script.ResetArray ();
		}
		if (GUILayout.Button ("Print")) {
			script.PrintArray ();
		}
	}
}
