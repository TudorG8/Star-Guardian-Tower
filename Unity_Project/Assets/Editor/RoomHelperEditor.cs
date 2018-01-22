using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoomHelper))]
public class RoomHelperEditor : Editor {
	public override void OnInspectorGUI () {
		RoomHelper script = (RoomHelper)target;

		if (script.Editable) {
			if (GUILayout.Button ("Deactivate Editor Tools")) {
				script.SetActive (false);
			}
		} 
		else {
			if (GUILayout.Button ("Activate Editor Tools")) {
				script.SetActive (true);
			}
		}

		if (script.Editable) {
			DrawDefaultInspector ();

			if (GUILayout.Button ("Reset"       )) { script.Reset          (); }
			if (GUILayout.Button ("Print"       )) { script.PrintRooms     (); }
			if (GUILayout.Button ("Add To Cache")) { script.AddRoomToCache (); }
		}
		else {
			GUI.enabled = false;
			DrawDefaultInspector ();
			GUI.enabled = true;
		}	
	}
}
