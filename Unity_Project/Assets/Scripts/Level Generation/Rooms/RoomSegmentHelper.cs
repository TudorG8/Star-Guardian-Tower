#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/**
 * Editor Tool for Room Segments
 */
[ExecuteInEditMode]
[SelectionBase]
[System.Serializable]
public class RoomSegmentHelper : MonoBehaviour {
	[System.Serializable]
	public class PlatformRefs {
		public List<GameObject> platforms;

		public void SetXScale(Direction side, float xScale) {
			foreach (GameObject platform in platforms) {
				if(platform.name.Contains(side.ToString())) {
					platform.transform.localScale = new Vector2 (xScale, platform.transform.localScale.y);
				}
			}
		}
		public void SetXScale(Direction side, int index, float xScale) {
			foreach (GameObject platform in platforms) {
				if(platform.name.Contains(side.ToString()) && platform.name.Contains(index.ToString())) {
					platform.transform.localScale = new Vector2 (xScale, platform.transform.localScale.y);
				}
			}
		}

		public GameObject Get(Direction side, int index) {
			for (int i = 0; i < platforms.Count; i++) {
				if (platforms [i].name.Contains (side.ToString ()) && platforms [i].name.Contains (index.ToString ()))
					return platforms [i];
			}

			Debug.LogError ("There was no platform of type " + side.ToString () + " " + index);
			return null;
		}

		public Direction GetFromIndex(Direction main, int index) {
			if (main == Direction.Top  || main == Direction.Bottom) {
				if (index == 1) return Direction.Left ;
				if (index == 2) return Direction.Right;
			}
			if (main == Direction.Left || main == Direction.Right ) {
				if (index == 1) return Direction.Bottom;
				if (index == 2) return Direction.Top   ;
			}
			Debug.LogError ("Bad Input");
			return Direction.None;
		}
	}

	[System.Serializable]
	public class AllPointRefs {
		public List<PointRefs> points;

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

	// Imports
	[SerializeField] public RoomHelper    roomGenerator; // Parent that holds all editor information
	[SerializeField] public RoomSegment   roomSegment  ; // Attached script for the room segment

	[SerializeField] public PlatformRefs  platformRefs       ; // References to the platforms
	[SerializeField] public AllPointRefs  pointRefs          ; // References to the points

	public void SetActive (bool active) {
		for (int i = 0; i < pointRefs.points.Count; i++) {
			PointRefs point = pointRefs.points [i];
			point.gameObject.SetActive (active);
		}
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

	public void DeleteRoom() {
		if(roomGenerator.DeleteRoomSegment (roomSegment.Index))
			DestroyImmediate (this.gameObject);
	}
		
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
}
#endif