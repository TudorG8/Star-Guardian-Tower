using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {
	public long id;
	public PointDTO entry;
	public PointDTO exit ;
	public Vector2 size;

	public void CopyValuesFrom(Room other) {
		entry = other.entry.GetCopy();
		exit  = other.exit.GetCopy();
		size  = other.size;
	}
}
