#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
[SelectionBase]
public class RoomGeneratorRoomHelper : MonoBehaviour {
	[System.Serializable]
	public class PlatformRefs {
		public GameObject top1   , top2   ;
		public GameObject bottom1, bottom2;
		public GameObject left1  , left2  ;
		public GameObject right1 , right2 ;

		public List<GameObject> platforms;

		public void SetXScale(PointDTO.Direction side, float xScale) {
			foreach (GameObject platform in platforms) {
				if(platform.name.Contains(side.ToString())) {
					platform.transform.localScale = new Vector2 (xScale, platform.transform.localScale.y);
				}
			}
		}
		public void SetXScale(PointDTO.Direction side, int index, float xScale) {
			Debug.Log (side + " " + index);
			foreach (GameObject platform in platforms) {
				if(platform.name.Contains(side.ToString()) && platform.name.Contains(index.ToString())) {
					platform.transform.localScale = new Vector2 (xScale, platform.transform.localScale.y);
				}
			}
		}
	}

	[System.Serializable]
	public class PointRefs {
		public GameObject top1   , top2   ;
		public GameObject bottom1, bottom2;
		public GameObject left1  , left2  ;
		public GameObject right1 , right2 ;

		public List<PointHelper> points;

		public void TurnOff(PointDTO.Direction side) {
			foreach (PointHelper point in points) {
				if(point.name.Contains(side.ToString())) {
					point.gameObject.SetActive (false);
				}
			}
		}
		public void TurnOn (PointDTO.Direction side) {
			foreach (PointHelper point in points) {
				if(point.name.Contains(side.ToString())) {
					point.gameObject.SetActive (true);
				}
			}
		}
	}
	[System.Serializable]
	public class Neighbours {
		public RoomGeneratorRoomHelper top   ;
		public RoomGeneratorRoomHelper bottom;
		public RoomGeneratorRoomHelper left  ;
		public RoomGeneratorRoomHelper right ; 
	}

	// Prefabs
	[SerializeField] GameObject roomPrefab; // Room that will be spawned

	// Imports
	[SerializeField] public RoomGenerator roomGenerator; // Parent that holds all editor information
	[SerializeField] public Room          roomScript   ; // This is were the final data used for level generation is stored
	[SerializeField] public PlatformRefs  platformRefs ; // References to the platforms
	[SerializeField] public PointRefs     pointRefs    ; // References to the points

	// Readonly
	[SerializeField] public Vector2    index;
	[SerializeField] Neighbours neighbours;

	public void SetName() {
		name = "Room " + index.y + " " + index.x;
	}
	public void SetNeighbour(PointDTO.Direction side, RoomGeneratorRoomHelper roomHelper) {
		if      (side == PointDTO.Direction.Top   ) neighbours.top    = roomHelper;
		else if (side == PointDTO.Direction.Bottom) neighbours.bottom = roomHelper;
		else if (side == PointDTO.Direction.Left  ) neighbours.left   = roomHelper;
		else if (side == PointDTO.Direction.Right ) neighbours.right  = roomHelper;
	}

	public void ResetNeighbour(PointDTO.Direction side) {
		if      (side == PointDTO.Direction.Top   ) neighbours.top    = null;
		else if (side == PointDTO.Direction.Bottom) neighbours.bottom = null;
		else if (side == PointDTO.Direction.Left  ) neighbours.left   = null;
		else if (side == PointDTO.Direction.Right ) neighbours.right  = null;
	}

	public void DeleteRoom() {
		if(roomGenerator.DeleteRoom (index))
			DestroyImmediate (this.gameObject);
	}
		
	void AddRoom(PointDTO.Direction side) {
		RoomGeneratorRoomHelper newRoom = this.roomGenerator.CreateRoom (transform.localPosition, side);

		this   .SetNeighbour (side                      , newRoom);
		newRoom.SetNeighbour (PointDTO.GetOpposite(side), this   );

		roomGenerator.AddRoom (index, side, newRoom);
	}

	public void AddRoomToTheTop   () {
		if (neighbours.top    != null) return;	
		AddRoom (PointDTO.Direction.Top   );
	}
	public void AddRoomToTheBottom() {
		if (neighbours.bottom != null) return;	
		AddRoom (PointDTO.Direction.Bottom);
	}
	public void AddRoomToTheLeft  () {
		if (neighbours.left   != null) return;	
		AddRoom (PointDTO.Direction.Left  );
	}
	public void AddRoomToTheRight () {
		if (neighbours.right  != null) return;	
		AddRoom (PointDTO.Direction.Right );
	}
}
#endif