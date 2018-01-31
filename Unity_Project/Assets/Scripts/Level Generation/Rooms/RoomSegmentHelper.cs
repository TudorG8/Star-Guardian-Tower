using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/**
 * Editor Tool for Room Segments.
 * Allows adding a new segment to the north, east, west and south.
 * Also allows deletion of rooms.
 */

[ExecuteInEditMode]
[SelectionBase]
[System.Serializable]
public class RoomSegmentHelper : MonoBehaviour {
	#if UNITY_EDITOR
	// Imports
	[SerializeField] public RoomHelper    roomGenerator; // Parent that holds all editor information
	[SerializeField] public RoomSegment   roomSegment  ; // Attached script for the room segment
	[SerializeField] public PlatformRefs  platformRefs ; // References to the platforms
	[SerializeField] public SegmentPoints pointRefs    ; // References to the points

	// Will disable all the points used for the arrows
	public void SetActive (bool active) {
		pointRefs.SetActive (active);
	}

	public void SetNeighbour(Direction side, RoomSegment roomHelper) {
		Neighbours neighbours = roomSegment.SegmentNeighbours; 
		if      (side == Direction.Top   ) neighbours.Top    = roomHelper;
		else if (side == Direction.Bottom) neighbours.Bottom = roomHelper;
		else if (side == Direction.Left  ) neighbours.Left   = roomHelper;
		else if (side == Direction.Right ) neighbours.Right  = roomHelper;
	}

	public void ResetNeighbour(Direction side) {
		Neighbours neighbours = roomSegment.SegmentNeighbours; 
		if      (side == Direction.Top   ) neighbours.Top    = null;
		else if (side == Direction.Bottom) neighbours.Bottom = null;
		else if (side == Direction.Left  ) neighbours.Left   = null;
		else if (side == Direction.Right ) neighbours.Right  = null;
	}

	// Deletes the segment and removes it from the room
	public void DeleteRoom() {
		if(roomGenerator.DeleteRoomSegment (roomSegment.Index))
			DestroyImmediate (this.gameObject);
	}

	// Creates a room segment in the given direction (if there isnt one already)
	void AddRoom(Direction side) {
		RoomSegment newRoom = this.roomGenerator.CreateRoomSegment (transform.localPosition, side);

		roomGenerator.AddRoomSegment (roomSegment.Index, side, newRoom);
	}

	public void AddRoomToTheTop   () {
		if (roomSegment.SegmentNeighbours.Top    != null) return;	
		AddRoom (Direction.Top   );
	}
	public void AddRoomToTheBottom() {
		if (roomSegment.SegmentNeighbours.Bottom != null) return;	
		AddRoom (Direction.Bottom);
	}
	public void AddRoomToTheLeft  () {
		if (roomSegment.SegmentNeighbours.Left   != null) return;	
		AddRoom (Direction.Left  );
	}
	public void AddRoomToTheRight () {
		if (roomSegment.SegmentNeighbours.Right  != null) return;	
		AddRoom (Direction.Right );
	}
	#endif
}