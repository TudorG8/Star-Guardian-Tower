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
	[SerializeField] Transform  objectParent  ;

	// Realonly
	[SerializeField][ReadOnly] long    id   ; // Unique ID in the cache
	[SerializeField][ReadOnly] bool    inUse; // Whether the room can be used by the cache
	[SerializeField][ReadOnly] Vector2 size ; // The number of rooms on both coordinates
	[SerializeField][ReadOnly] Vector2 previousPosition; // The position in the cache, before it gets moved on the screen

	// Information
	[SerializeField] PointDTO entry; 
	[SerializeField] PointDTO exit ;
	[SerializeField] SegmentArray rooms;

	[SerializeField] HashSet<RoomObject> objects  ;
	[SerializeField] HashSet<RoomObject> hazards  ;
	[SerializeField] HashSet<RoomObject> treasure ;
	[SerializeField] HashSet<RoomObject> platforms;

	// Properties
	public RoomHelper RoomGeneratorScript  { get { return roomGenerator ; }}
	public Transform  Center               { get { return center        ; }}
	public Transform  ObjectParent         { get { return objectParent  ; }}

	public long    Id    { get { return id   ; } set { id    = value; }}
	public bool    InUse { get { return inUse; } set { inUse = value; }}
	public Vector2 Size  { get { return size ; } set { size  = value; }}
	public Vector2 PreviousPosition { get { return previousPosition; } set { previousPosition = value; }}

	public PointDTO Entry { get { return entry; } }
	public PointDTO Exit  { get { return exit ; } }
	public SegmentArray Rooms { get { return rooms; } }

	public HashSet<RoomObject> Objects   { get { return objects  ; } }
	public HashSet<RoomObject> Hazards   { get { return hazards  ; } }
	public HashSet<RoomObject> Treasure  { get { return treasure ; } }
	public HashSet<RoomObject> Platforms { get { return platforms; } }

	// Methods
	public void Reset() {
		transform.localPosition = previousPosition;
		entry.triggerScript.Triggered = false;
		inUse = false;
	}

	public void OnEntry() {
		LevelGenerator.Instance.WhenPlayerEntersNewRoom (this);
	}

	public void CloseEntryGate () {
		
	}
}