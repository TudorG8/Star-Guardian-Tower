using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoomSegmentHelper))]
public class RoomSegmentHelperEditor : Editor {
	public override void OnInspectorGUI () {
		DrawDefaultInspector ();
		RoomSegmentHelper script = (RoomSegmentHelper)target;
		if (script.roomGenerator.Editable) {
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
			if (GUILayout.Button ("Delete Room")) {
				script.DeleteRoom ();
			}
		}
	}
}
