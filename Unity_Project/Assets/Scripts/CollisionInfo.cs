using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollisionInfo {
	public bool above, below;
	public bool left , right;
	public bool ascendingSlope, descendingSlope;
	public bool slidingDownSlope;
	public bool hangingOnEdge;

	public float slopeAngle;
	public float slopeAngleOld;

	public void Reset() {
		above = below = left = right = false;
		ascendingSlope = descendingSlope = false;
		slidingDownSlope = false;
		hangingOnEdge = false;

		slopeAngleOld = slopeAngle;
		slopeAngle = 0;
	}
}