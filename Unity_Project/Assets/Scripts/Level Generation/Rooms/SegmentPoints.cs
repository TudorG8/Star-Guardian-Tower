using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Holds references to the points the arrows can cling to.
 * Allows turning them on or off.
 */

[System.Serializable]
public class SegmentPoints {
	public List<PointRefs> points;

	public void SetActive (bool active) {
		for (int i = 0; i < points.Count; i++) {
			PointRefs point = points [i];
			point.gameObject.SetActive (active);
		}
	}

	public void TurnOff(Direction side) {
		foreach (PointRefs point in points) {
			if(point.name.Contains(side.ToString())) {
				point.gameObject.SetActive (false);
			}
		}
	}
	public void TurnOn (Direction side) {
		foreach (PointRefs point in points) {
			if(point.name.Contains(side.ToString())) {
				point.gameObject.SetActive (true);
			}
		}
	}
}