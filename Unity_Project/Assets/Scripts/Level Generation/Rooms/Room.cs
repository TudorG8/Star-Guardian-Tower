using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPropertyDrawers;

/**
 * Holds runtime information related to rooms.
 */

public class Room : MonoBehaviour {
	// Imports
	[SerializeField] RoomHelper roomGenerator ;
	[SerializeField] Transform  center        ;

	// Realonly
	[SerializeField][ReadOnly] long    id   ; // Unique ID in the cache
	[SerializeField][ReadOnly] bool    inUse; // Whether the room can be used by the cache
	[SerializeField][ReadOnly] Vector2 size ; // The number of rooms on both coordinates
	[SerializeField][ReadOnly] Vector2 previousPosition; // The position in the cache, before it gets moved on the screen

	// Information
	[SerializeField] PointDTO entry; 
	[SerializeField] PointDTO exit ;
	[SerializeField] SegmentArray rooms;

	// Properties
	public RoomHelper RoomGeneratorScript  { get { return roomGenerator ; }}
	public Transform  Center               { get { return center        ; }}

	public long    Id    { get { return id   ; } set { id    = value; }}
	public bool    InUse { get { return inUse; } set { inUse = value; }}
	public Vector2 Size  { get { return size ; } set { size  = value; }}
	public Vector2 PreviousPosition { get { return previousPosition; } set { previousPosition = value; }}

	public PointDTO Entry { get { return entry; } }
	public PointDTO Exit  { get { return exit ; } }
	public SegmentArray Rooms { get { return rooms; } }

	// Methods
	public void Reset() {
		transform.localPosition = previousPosition;
		entry.triggerScript.Triggered = false;
		inUse = false;
	}

	public void OnEntry() {
		LevelGenerator.Instance.WhenPlayerEntersNewRoom (this);
	}
}