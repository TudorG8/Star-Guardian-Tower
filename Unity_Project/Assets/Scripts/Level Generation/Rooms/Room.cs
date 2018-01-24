using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPropertyDrawers;

/**
 * Holds runtime information related to rooms.
 */

public class Room : MonoBehaviour {
	// Imports
	[SerializeField] RoomHelper roomGenerator  ;
	[SerializeField] Transform  objectParent   ;
	[SerializeField] Transform  hazardParent   ;
	[SerializeField] Transform  treasureParent ;
	[SerializeField] Transform  platformsParent;

	// Readonly
	[SerializeField][ReadOnly] long    id   ; // Unique ID in the cache
	[SerializeField][ReadOnly] bool    inUse; // Whether the room can be used by the cache
	[SerializeField][ReadOnly] Vector2 size ; // The number of rooms on both coordinates
	[SerializeField][ReadOnly] Vector2 previousPosition; // The position in the cache, before it gets moved on the screen

	// Information
	[SerializeField] PointDTO entry; 
	[SerializeField] PointDTO exit ;
	[SerializeField] SegmentArray rooms;

	[SerializeField] List<RoomObject> objects  ;
	[SerializeField] List<RoomObject> hazards  ;
	[SerializeField] List<RoomObject> treasure ;
	[SerializeField] List<RoomObject> platforms;

	// Properties
	public RoomHelper RoomGeneratorScript  { get { return roomGenerator  ; }}
	public Transform  ObjectParent         { get { return objectParent   ; }}
	public Transform  HazardParent         { get { return hazardParent   ; }}
	public Transform  TreasureParent       { get { return treasureParent ; }}
	public Transform  PlatformsParent      { get { return platformsParent; }}

	public long    Id    { get { return id   ; } set { id    = value; }}
	public bool    InUse { get { return inUse; } set { inUse = value; }}
	public Vector2 Size  { get { return size ; } set { size  = value; }}
	public Vector2 PreviousPosition { get { return previousPosition; } set { previousPosition = value; }}

	public PointDTO Entry { get { return entry; } }
	public PointDTO Exit  { get { return exit ; } }
	public SegmentArray Rooms { get { return rooms; } }

	public List<RoomObject> Objects   { get { return objects  ; } }
	public List<RoomObject> Hazards   { get { return hazards  ; } }
	public List<RoomObject> Treasure  { get { return treasure ; } }
	public List<RoomObject> Platforms { get { return platforms; } }

	// Methods
	public void Reset() {
		transform.localPosition = previousPosition;
		entry.TriggerScript.Reset ();
		inUse = false;
		entry.Door.SetTrigger ("open");

		ResetList (objects  );
		ResetList (hazards  );
		ResetList (treasure );
		ResetList (platforms);
	}

	public void ResetList(List<RoomObject> list) {
		for (int i = 0; i < list.Count; i++) {
			if (list [i] is IResetable) {
				IResetable resetableObj = (IResetable)list [i];
				resetableObj.Reset ();
			}
		}
	}

	public void OnEntry() {
		LevelGenerator.Instance.WhenPlayerEntersNewRoom (this);
	}

	public void OnExit () {
		LevelGenerator.Instance.WhenPlayerFinishesARoom (this);
	}

	public void CloseEntryGate () {
		
	}
}