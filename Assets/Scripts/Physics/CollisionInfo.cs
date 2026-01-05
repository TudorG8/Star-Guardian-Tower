using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Holds informations about the collisions and other details that happened last frame.
 */

[System.Serializable]
public class CollisionInfo {
	[SerializeField] bool above, below;
	[SerializeField] bool left , right;
	[SerializeField] bool ascendingSlope, descendingSlope;
	[SerializeField] bool hangingOnEdge;

	[SerializeField] float slopeAngle;
	[SerializeField] float slopeAngleOld;

	public void Reset() {
		above = below = left = right = false;
		ascendingSlope = descendingSlope = false;
		hangingOnEdge  = false;

		slopeAngleOld = slopeAngle;
		slopeAngle = 0;
	}

	public bool  Above           { get { return above          ; } set { above           = value; }}
	public bool  Below           { get { return below          ; } set { below           = value; }}
	public bool  Left            { get { return left           ; } set { left            = value; }}
	public bool  Right           { get { return right          ; } set { right           = value; }}
	public bool  AscendingSlope  { get { return ascendingSlope ; } set { ascendingSlope  = value; }}
	public bool  DescendingSlope { get { return descendingSlope; } set { descendingSlope = value; }}
	public bool  HangingOnEdge   { get { return hangingOnEdge  ; } set { hangingOnEdge   = value; }}
	public float SlopeAngle      { get { return slopeAngle     ; } set { slopeAngle      = value; }}
	public float SlopeAngleOld   { get { return slopeAngleOld  ; } set { slopeAngleOld   = value; }}
}