using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoomHelper))]
public class RoomHelperEditor : Editor {
	public override void OnInspectorGUI () {
		DrawDefaultInspector ();
		RoomHelper script = (RoomHelper)target;
		if (GUILayout.Button ("Reset"       )) { script.Reset          (); }
		if (GUILayout.Button ("Print"       )) { script.PrintRooms     (); }
		if (GUILayout.Button ("Add To Cache")) { script.AddRoomToCache (); }
	}
}
