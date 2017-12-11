using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoomGeneratorRoomHelper))]
public class RoomSceneEditorEditor : Editor {
	public override void OnInspectorGUI () {
		DrawDefaultInspector ();
		RoomGeneratorRoomHelper script = (RoomGeneratorRoomHelper)target;
		if (GUILayout.Button ("Add Top Room")) {
			script.AddRoomToTheTop ();
		}
	}
}
