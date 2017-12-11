using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoomSceneEditor))]
public class RoomSceneEditorEditor : Editor {
	public override void OnInspectorGUI () {
		DrawDefaultInspector ();
		RoomSceneEditor script = (RoomSceneEditor)target;
		if (GUILayout.Button ("Add Top Room")) {
			script.AddRoomToTheTop ();
		}
		if (GUILayout.Button ("Add Bottom Room")) {
			script.AddRoomToTheBottom ();
		}
		if (GUILayout.Button ("Add Left Room")) {
			script.AddRoomToTheLeft ();
		}
		if (GUILayout.Button ("Add Right Room")) {
			script.AddRoomToTheRight ();
		}
	}
}
