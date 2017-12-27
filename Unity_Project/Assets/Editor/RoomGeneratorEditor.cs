using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoomGenerator))]
public class RoomGeneratorEditor : Editor {
	public override void OnInspectorGUI () {
		DrawDefaultInspector ();
		RoomGenerator script = (RoomGenerator)target;
		if (GUILayout.Button ("Reset" )) { script.Reset          (); }
		if (GUILayout.Button ("Print" )) { script.PrintRooms     (); }
		if (GUILayout.Button ("Create")) { script.AddRoomToCache (); }
		Room room = null;
		room = EditorGUILayout.ObjectField (room, typeof(Room), false) as Room;
	}
}
