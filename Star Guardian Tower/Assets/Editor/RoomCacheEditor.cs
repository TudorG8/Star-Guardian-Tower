using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoomCache))]
public class RoomCacheEditor : Editor {
	public override void OnInspectorGUI () {
		DrawDefaultInspector ();
		RoomCache script = (RoomCache)target;
		if (GUILayout.Button ("Print" )) { script.PrintCache (); }
		if (GUILayout.Button ("Reset" )) { script.Reset      (); }
		if (GUILayout.Button ("Deactivate All Room Tools" )) { script.SetRoomsAs       (active: false); }
		if (GUILayout.Button ("Activate All Room Tools"   )) { script.SetRoomsAs       (active: true ); }
		if (GUILayout.Button ("Activate All Rooms"        )) { script.ActivateAllRooms (active: false ); }
		if (GUILayout.Button ("Deactivate All Rooms"      )) { script.ActivateAllRooms (active: true ); }
		if (GUILayout.Button ("Prepare for release "      )) { script.PrepareForRelease   (); }
		if (GUILayout.Button ("UnPrepare for release "    )) { script.UnPrepareForRelease (); }
	}
}
