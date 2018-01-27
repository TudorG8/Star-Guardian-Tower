using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPropertyDrawers;

#if UNITY_EDITOR
using UnityEditor;
#endif

/**
 * This class is used to make an editor friendly room generator.
 * It is mostly used to handle the internal logic such as spawning/deleting rooms.
 * It offers two buttons:
 * 		Reset : deletes all rooms and spawns a new one
 * 		Print : prints the internal array of rooms
 * Check <RoomGeneratorRoomHelper> for the room options.
 */
[ExecuteInEditMode]
public class RoomHelper : MonoBehaviour {
	[SerializeField] Room roomScript;
	[SerializeField] bool editable  ; // Update scripts won't run if this is set to false
	public void SetActive(bool active) {
		List<RoomSegment> validRooms = roomScript.Rooms.GetValidElements ();
		for (int i = 0; i < validRooms.Count; i++) {
			RoomSegment segment = validRooms [i];
			segment.SetHelperScriptActiveAs (active);
		}
		roomScript.Entry.Sprite.gameObject.SetActive (active);
		roomScript.Exit .Sprite.gameObject.SetActive (active);
		editable = active;
	}
	#if UNITY_EDITOR
	// Extra Classes ---------------------------------------------------------------------------------------------
	/**
	 * We use this to hold information about the previous point, platforms and size.
	 */
	[System.Serializable]
	public class PointExtraInfo {
		[SerializeField] Transform  point            ;
		[SerializeField] Transform  previousPoint    ;
		[SerializeField] GameObject previousPlatform1;
		[SerializeField] GameObject previousPlatform2;
		[SerializeField] float      previousSize     ;

		public Transform  Point             { get { return point            ; } set { point             = value; }}
		public Transform  PreviousPoint     { get { return previousPoint    ; } set { previousPoint     = value; }}
		public GameObject PreviousPlatform1 { get { return previousPlatform1; } set { previousPlatform1 = value; }}
		public GameObject PreviousPlatform2 { get { return previousPlatform2; } set { previousPlatform2 = value; }}
		public float      PreviousSize      { get { return previousSize     ; } set { previousSize      = value; }}
	}
	// Variables -------------------------------------------------------------------------------------------------
	// Imports


	// Information Fields
	[SerializeField] PointExtraInfo entryPoint;
	[SerializeField] PointExtraInfo exitPoint ;

	// Settings

	[SerializeField] bool disconnectPrefabInstance; // Only leave this on if you are editing the base prefab

	public bool Editable { get { return editable; } }
	public bool InUse { get { return roomScript.InUse; } set { roomScript.InUse = value; } }

	// Methods ---------------------------------------------------------------------------------------------------
	// Print the rooms
	public void PrintRooms() { roomScript.Rooms.PrintArray (); }

	// Reset the room array and spawn in a new room
	public void Reset() {
		roomScript.Rooms.Reset ();
		CheckIfRoomHasAValidID ();
		RoomSegment newRoomSegment = CreateRoomSegment (new Vector2(0, 0), Direction.None);
		AddRoomSegment (new Vector2(0, 0), Direction.None, newRoomSegment);
	}



	void Start() {
		if(disconnectPrefabInstance)
			PrefabUtility.DisconnectPrefabInstance(gameObject);
	}

	/**
	 * Create a room object based on the side parameter.
	 * If it is "None", the room will be created at the position given.
	 * Otherwise, it will be created to one of the four sides.
	 * @return: the script on the generated room
	 */
	public RoomSegment CreateRoomSegment (Vector2 position, Direction side) {
		Vector2 spawnPosition = position;
		spawnPosition += UsefulMethods.vectorProduct(RoomCache.Instance.RoomSize, DirectionHelper.GetDirectionVector(side));

		GameObject roomObj = Instantiate (RoomCache.Instance.RoomPrefab, spawnPosition, Quaternion.identity) as GameObject;
		roomObj.transform.SetParent (transform, false);
		roomObj.transform.SetAsLastSibling ();

		RoomSegment roomSegment = roomObj.GetComponent<RoomSegment> ();
		roomSegment.HelperScript.roomGenerator = this;

		return roomSegment;
	}

	/**
	 * Adds a room to the room array and makes sure everything goes well.
	 * It also checks nearby rooms and corners to disable the walls
	 */
	public void AddRoomSegment (Vector2 originalPosition, Direction direction, RoomSegment newRoom) {
		SegmentArray rooms = roomScript.Rooms;

		if (rooms.IsNull()) Reset ();

		Vector2 desiredPosition = originalPosition + DirectionHelper.GetDirectionVector (direction);
		if      (desiredPosition.y >= rooms.Rows) rooms.AddRowToTop    ();
		else if (desiredPosition.y <           0) rooms.AddRowToBottom ();
		if      (desiredPosition.x >= rooms.Cols) rooms.AddRowToRight  ();
		else if (desiredPosition.x <           0) rooms.AddRowToLeft   ();

		// If we are adding a row to the left or bottom, the new position will be 0
		Vector2 newPosition = new Vector2(0, 0);
		if (desiredPosition.x < 0) { desiredPosition.x = 0; newPosition.x = 1; }
		if (desiredPosition.y < 0) { desiredPosition.y = 0; newPosition.y = 1; }

		newRoom.Index = desiredPosition;
		newRoom.IncreasePosition (newPosition);
		newRoom.SetName ();

		rooms.SetRoom(desiredPosition, newRoom);

		CheckForNearbyRooms (desiredPosition, Direction.Top   );
		CheckForNearbyRooms (desiredPosition, Direction.Bottom);
		CheckForNearbyRooms (desiredPosition, Direction.Left  );
		CheckForNearbyRooms (desiredPosition, Direction.Right );

		CheckCorner (desiredPosition, Direction.Top   , Direction.Right, 0f);
		CheckCorner (desiredPosition, Direction.Top   , Direction.Left , 0f);
		CheckCorner (desiredPosition, Direction.Bottom, Direction.Right, 0f);
		CheckCorner (desiredPosition, Direction.Bottom, Direction.Left , 0f);

		// Make the editor select the new room
		Selection.objects = new Object[] { newRoom.gameObject };
	}

	/**
	 * Deletes a room from the array and makes sure everything goes well.
	 * It also checks nearby rooms and corners to re-enable them if required.
	 */
	public bool DeleteRoomSegment (Vector2 position) {
		SegmentArray rooms = roomScript.Rooms;

		if (rooms.IsNull()) Reset ();

		if (rooms.ThereIsOnlyOneRoom()) { Debug.LogError ("Cannot destroy the only room"); return false;}

		rooms.SetRoom (position, null);

		// Check corners first so that the nearby room check sets the right dimensions
		CheckCorner (position, Direction.Top   , Direction.Right, RoomCache.Instance.MinimumPlatformSize.x);
		CheckCorner (position, Direction.Top   , Direction.Left , RoomCache.Instance.MinimumPlatformSize.x);
		CheckCorner (position, Direction.Bottom, Direction.Right, RoomCache.Instance.MinimumPlatformSize.x);
		CheckCorner (position, Direction.Bottom, Direction.Left , RoomCache.Instance.MinimumPlatformSize.x);

		CheckForNearbyRoomsForDelete (position, Direction.Top   );
		CheckForNearbyRoomsForDelete (position, Direction.Bottom);
		CheckForNearbyRoomsForDelete (position, Direction.Left  );
		CheckForNearbyRoomsForDelete (position, Direction.Right );

		rooms.DeleteUselessSpots ();

		return true;
	}

	/**
	 * Checks if there is a neighbour in a given direction.
	 * If there is, it will disable its points and set the right scales.
	 */
	public void CheckForNearbyRooms(Vector2 index, Direction side) {
		SegmentArray rooms = roomScript.Rooms;

		Vector2 desiredPosition = index + DirectionHelper.GetDirectionVector (side);

		Debug.Log (index);
		Debug.Log (desiredPosition);
		Debug.Log (rooms.IsNotAValidPosition (desiredPosition));
		if (rooms.IsNotAValidPosition (desiredPosition)) return;

		RoomSegment neighbour = rooms.RoomAt(desiredPosition);
		RoomSegment actual    = rooms.RoomAt(index          );

		if (neighbour != null) {
			actual.HelperScript.pointRefs   .TurnOff   (side);
			actual.HelperScript.platformRefs.SetXScale (side, RoomCache.Instance.MinimumPlatformSize.x);

			neighbour.HelperScript.pointRefs   .TurnOff   (DirectionHelper.GetOpposite(side));
			neighbour.HelperScript.platformRefs.SetXScale (DirectionHelper.GetOpposite(side), RoomCache.Instance.MinimumPlatformSize.x);

			actual   .HelperScript.SetNeighbour (side, neighbour.HelperScript.roomSegment);
			neighbour.HelperScript.SetNeighbour (DirectionHelper.GetOpposite(side), actual.HelperScript.roomSegment);
		}
	}

	/**
	 * Checks if there is a neighbour in a given direction.
	 * If there is, it will enable its points and set the right scales.
	 */
	public void CheckForNearbyRoomsForDelete(Vector2 index, Direction side) {
		SegmentArray rooms = roomScript.Rooms;

		Vector2 desiredPosition = index + DirectionHelper.GetDirectionVector (side);

		if (rooms.IsNotAValidPosition (desiredPosition)) return;

		RoomSegment neighbour = rooms.RoomAt (desiredPosition);

		if (neighbour != null) {
			float size = (side == Direction.Top || side == Direction.Bottom) ? RoomCache.Instance.RoomSize.x / 2 : RoomCache.Instance.RoomSize.y / 2;

			neighbour.HelperScript.pointRefs   .TurnOn    (DirectionHelper.GetOpposite(side));
			neighbour.HelperScript.platformRefs.SetXScale (DirectionHelper.GetOpposite(side), size);

			neighbour.HelperScript.ResetNeighbour (DirectionHelper.GetOpposite(side));
		}	
	}

	/**
	 * A bit of weird logic, we basically check a corner to see if we should do something to it.
	 * The conditions for either removing a corner or setting it are:
	 * 		There is a valid room in all three directions (example: top, right and topright)
	 * dir1 should always be top or bottom and dir2 should always be left or right
	 * index1 is used for dir1
	 * index2 is used for dir2
	 * index3 and 4 are used for the actual corner room.
	 * @param roomIndex : index of the room we check from
	 * @param size      : what size the corner platforms will be set to
	 */
	void CheckCorner(Vector2 roomIndex, Direction dir1, Direction dir2, float size) {
		SegmentArray rooms = roomScript.Rooms;

		if (dir1 != Direction.Top && dir1 != Direction.Bottom) {
			Debug.LogError ("Direction 1 cannot be left or right"); return;
		}

		if (dir2 != Direction.Left && dir2 != Direction.Right) {
			Debug.LogError ("Direction 2 cannot be top or bottom"); return;
		}
		int index1, index2, index3, index4;
		if      (dir1 == Direction.Top    && dir2 == Direction.Right)  { index1 = 2; index2 = 2; index3 = 1; index4 = 1; }
		else if (dir1 == Direction.Top    && dir2 == Direction.Left )  { index1 = 1; index2 = 2; index3 = 2; index4 = 1; }
		else if (dir1 == Direction.Bottom && dir2 == Direction.Left )  { index1 = 1; index2 = 1; index3 = 2; index4 = 2; }
		else  /*(dir1 == Direction.Bottom && dir2 == Direction.Right)*/{ index1 = 2; index2 = 1; index3 = 1; index4 = 2; }

		Vector2 direction1 = DirectionHelper.GetDirectionVector (dir1);
		Vector2 direction2 = DirectionHelper.GetDirectionVector (dir2);

		Vector2 position1 = roomIndex + direction1;
		Vector2 position2 = roomIndex + direction2;
		Vector2 position3 = roomIndex + direction1 + direction2;
		if (rooms.IsValidPosition (position1) && rooms.IsValidPosition (position2) && rooms.IsValidPosition (position3)) {
			if (rooms.RoomAt (position1) != null && rooms.RoomAt (position2) != null && rooms.RoomAt (position3) != null) {
				rooms.RoomAt (position1).HelperScript.platformRefs.SetXScale (DirectionHelper.GetOpposite(dir1), index1, size);
				rooms.RoomAt (position1).HelperScript.platformRefs.SetXScale (dir2, dir1 == Direction.Top   ? 1 : 2, size);

				rooms.RoomAt (position2).HelperScript.platformRefs.SetXScale (DirectionHelper.GetOpposite(dir2), index2, size);
				rooms.RoomAt (position2).HelperScript.platformRefs.SetXScale (dir1, dir2 == Direction.Right ? 1 : 2, size);

				rooms.RoomAt (position3).HelperScript.platformRefs.SetXScale (DirectionHelper.GetOpposite(dir1), index3, size);
				rooms.RoomAt (position3).HelperScript.platformRefs.SetXScale (DirectionHelper.GetOpposite(dir2), index4, size);

				// Also check the room itself
				if (rooms.IsValidPosition (roomIndex) && rooms.RoomAt (roomIndex) != null) {
					rooms.RoomAt (roomIndex).HelperScript.platformRefs.SetXScale (dir1, index1, size);
					rooms.RoomAt (roomIndex).HelperScript.platformRefs.SetXScale (dir2, index2, size);
				}
			}
		}
	}

	// Adds the room to the cache to allow it to be used ingame
	public void AddRoomToCache() {
		CheckIfRoomHasAValidID ();
		
		RoomCache.Instance.AddNewRoom (roomScript);
	}

	// Checks if the room has a valid ID set
	public void CheckIfRoomHasAValidID() {
		if (roomScript.Id == 0 || !name.Contains (roomScript.Id.ToString())) {
			long newId = gameObject.GetInstanceID ();
			gameObject.name = "Room " + newId;
			roomScript.Id = newId;
		}
	}

	// Handling Exit and Enter arrows ----------------------------------------------------------------------------
	/**
	 * Handle what happens when we found a new spot for our arrow (entry or exit).
	 * Change the roomScript accordingly (main, secondary and index) and make sure the platforms have the right size.
	 * Update the previous point info for the next update cycle.
	 */
	void HandlePlatformSize(PointRefs newPoint, PointExtraInfo previousInfo, PointDTO roomPoint) {
		RoomSegmentHelper.PlatformRefs platformRefs = newPoint.roomEditor.platformRefs;

		string mainString      = newPoint.name.Substring (0, newPoint.name.Length - 2);
		string secondaryString = newPoint.name.Substring (newPoint.name.Length - 1);

		roomPoint.Main      = DirectionHelper.GetByName (mainString);
		roomPoint.Secondary = platformRefs.GetFromIndex(roomPoint.Main, int.Parse(secondaryString));
		roomPoint.RoomIndex = newPoint.roomEditor.roomSegment.Index;

		if (RoomCache.Instance == null)
			return;
			
		float newSize = (roomPoint.Main == Direction.Top || roomPoint.Main == Direction.Bottom ) ? RoomCache.Instance.RoomSize.x / 2 : RoomCache.Instance.RoomSize.y / 2;

		// Get which platforms to modify
		int mainIndex, secondaryIndex;
		if   (secondaryString == "1") { mainIndex = 1; secondaryIndex = 2; } 
		else/*secondaryString == "2"*/{ mainIndex = 2; secondaryIndex = 1; }
		GameObject platform1 = platformRefs.Get (roomPoint.Main, mainIndex     );
		GameObject platform2 = platformRefs.Get (roomPoint.Main, secondaryIndex);

		// If there is a previous point and its active
		if (previousInfo.PreviousPoint != null && previousInfo.PreviousPoint.gameObject.activeSelf) {
			previousInfo.PreviousPlatform1.transform.localScale = new Vector2 (previousInfo.PreviousSize, previousInfo.PreviousPlatform1.transform.localScale.y);
			previousInfo.PreviousPlatform2.transform.localScale = new Vector2 (previousInfo.PreviousSize, previousInfo.PreviousPlatform2.transform.localScale.y);
		}

		platform1.transform.localScale = new Vector2 (RoomCache.Instance.MinimumPlatformSize.x , platform1.transform.localScale.y);
		platform2.transform.localScale = new Vector2 (newSize * 2 - RoomCache.Instance.PointGap, platform2.transform.localScale.y);

		previousInfo.PreviousSize      = newSize;
		previousInfo.PreviousPoint     = newPoint.transform;
		previousInfo.PreviousPlatform1 = platform1;
		previousInfo.PreviousPlatform2 = platform2;
	}
	/**
	 * Check if we deleted at objects or if we added any new ones.
	 * These are held in the room object. 
	 * This is inneficient, but is only ran during edit mode.
	 */
	void HandleObjectsChange(List<RoomObject> objects, Transform parent) {
		// Check if a room was deleted in the editor
		foreach(RoomObject obj in objects) {
			if (obj == null) {
				objects.Remove (obj);
				break;
			}
		}
		// Check if a room was added in the editor
		foreach (Transform child in parent) {
			RoomObject obj = child.GetComponent<RoomObject> ();
			if (obj != null && !objects.Contains (obj)) {
				objects.Add (obj);
			}
		}
	}

	void Update () {
		if (editable) {
			SegmentArray rooms = roomScript.Rooms;

			CheckIfRoomHasAValidID ();

			if (rooms.IsNull ()) {
				Reset ();
			}

			roomScript.Size = new Vector2 (rooms.Cols, rooms.Rows);

			if (!roomScript.InUse)
				this.roomScript.PreviousPosition = transform.localPosition;
		
			// Gather all points from all rooms
			List<RoomSegment> validSegments = rooms.GetValidElements ();
			List<PointRefs> validPoints = new List<PointRefs> ();
			for (int i = 0; i < validSegments.Count; i++) {
				validPoints.AddRange (validSegments [i].HelperScript.pointRefs.points);
			}

			// Handle exit point
			if (exitPoint.Point != null && exitPoint.Point.gameObject.activeSelf) {
				float minDistance = float.MaxValue;
				PointRefs minPoint = null;
				foreach (PointRefs point in validPoints) {
					if (point.gameObject.activeSelf) {
						float pointDistance = Vector2.Distance (exitPoint.Point.transform.position, point.transform.position);
						if (pointDistance < minDistance) {
							minDistance = pointDistance;
							minPoint = point;
						}
					}
				}
				if (minPoint != null) {
					exitPoint.Point.transform.position = minPoint.transform.position;
					exitPoint.Point.transform.rotation = minPoint.transform.rotation;

					HandlePlatformSize (minPoint, exitPoint, roomScript.Exit);
				}

				validPoints.Remove (minPoint);
			}

			// Handle entry point
			if (entryPoint.Point != null && entryPoint.Point.gameObject.activeSelf) {
				float minDistance = float.MaxValue;
				PointRefs minPoint = null;
				foreach (PointRefs point in validPoints) {
					if (point.gameObject.activeSelf) {
						float pointDistance = Vector2.Distance (entryPoint.Point.transform.position, point.transform.position);
						if (pointDistance < minDistance) {
							minDistance = pointDistance;
							minPoint = point;
						}
					}
				}
				if (minPoint != null) {
					entryPoint.Point.transform.position = minPoint.transform.position;
					entryPoint.Point.transform.rotation = minPoint.transform.rotation;

					HandlePlatformSize (minPoint, entryPoint, roomScript.Entry);
				}
			}

			HandleObjectsChange (roomScript.Objects  , roomScript.ObjectParent   );
			HandleObjectsChange (roomScript.Hazards  , roomScript.HazardParent   );
			HandleObjectsChange (roomScript.Treasure , roomScript.TreasureParent );
			HandleObjectsChange (roomScript.Platforms, roomScript.PlatformsParent);
		}
	}
	// -----------------------------------------------------------------------------------------------------------
	#endif
}
