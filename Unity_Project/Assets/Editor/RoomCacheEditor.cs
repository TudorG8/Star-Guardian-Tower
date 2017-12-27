using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoomCache))]
public class RoomCacheEditor : Editor {
	Room room;
	public override void OnInspectorGUI () {
		DrawDefaultInspector ();
		RoomCache script = (RoomCache)target;
		if (GUILayout.Button ("Print" )) { script.PrintCache (); }
		if (GUILayout.Button ("Reset" )) { script.Reset      (); }
		if (GUILayout.Button ("Remove")) { 
			script.RemoveRoom (room); 
			room = null;
		}
		room = EditorGUILayout.ObjectField (room, typeof(Room), true) as Room;
	}
}
