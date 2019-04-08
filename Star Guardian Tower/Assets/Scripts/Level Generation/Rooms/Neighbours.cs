using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Holds references to the neighbours of a room.
 * Should probably be generic some day...
 */

[System.Serializable]
public class Neighbours {
	[SerializeField] RoomSegment top   ;
	[SerializeField] RoomSegment bottom;
	[SerializeField] RoomSegment left  ;
	[SerializeField] RoomSegment right ; 

	public RoomSegment Top    { get { return top   ; } set { top    = value; }}
	public RoomSegment Bottom { get { return bottom; } set { bottom = value; }}
	public RoomSegment Left   { get { return left  ; } set { left   = value; }}
	public RoomSegment Right  { get { return right ; } set { right  = value; }}
}