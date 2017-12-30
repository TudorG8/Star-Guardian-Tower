using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelGenerator))]
public class LevelGeneratorEditor : Editor {
	public override void OnInspectorGUI () {
		DrawDefaultInspector ();
		LevelGenerator script = (LevelGenerator)target;
		if (GUILayout.Button ("Test")) { script.Test (); }
	}
}
