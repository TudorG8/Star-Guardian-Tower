using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/**
 * Holds any custom property drawers made by me.
 */
namespace CustomPropertyDrawers {
	/**
	 * A readonly field can not be edited but can still be viewed in the inspector.
	 */
	public class ReadOnly : PropertyAttribute {}

	#if UNITY_EDITOR
	[CustomPropertyDrawer(typeof(ReadOnly))]
	public class ReadOnlyDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			GUI.enabled = false;
			EditorGUI.PropertyField (position, property, label, true);
			GUI.enabled = true;
		}
	}
	#endif
}