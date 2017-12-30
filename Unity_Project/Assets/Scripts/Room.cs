using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {
	public LevelGenerator levelGenerator;
	public Transform center;

	public long id;
	public bool inUse;
	public PointDTO entry;
	public PointDTO exit ;
	public Vector2 size;

	public Vector2 previousPosition;

	public void CopyValuesFrom(Room other) {
		entry = other.entry.GetCopy();
		exit  = other.exit.GetCopy();
		size  = other.size;
	}

	public void Reset() {
		inUse = false;
	}

	public void OnEntry() {
		Debug.Log ("yes");
		levelGenerator.WhenPlayerEntersNewRoom (center);
	}
}
