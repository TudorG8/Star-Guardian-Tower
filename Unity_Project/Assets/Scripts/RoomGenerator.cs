#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CustomPropertyDrawers;

/**
 * This class is used to make an editor friendly room generator.
 * It is mostly used to handle the internal logic such as spawning/deleting rooms.
 * It offers two buttons:
 * 		Reset : deletes all rooms and spawns a new one
 * 		Print : prints the internal array of rooms
 * Check <RoomGeneratorRoomHelper> for the room options.
 */
[ExecuteInEditMode]
public class RoomGenerator : MonoBehaviour {
	// Extra Classes ---------------------------------------------------------------------------------------------
	/**
	 * We use this to hold information about the previous point, platforms and size.
	 */
	[System.Serializable]
	public class PointExtraInfo {
		public Transform  point;
		public Transform  previousPoint;
		public GameObject previousPlatform1;
		public GameObject previousPlatform2;
		public float      previousSize;
	}
	// Variables -------------------------------------------------------------------------------------------------
	// Imports
	[SerializeField] RoomCache roomCache ;
	[SerializeField] Room      roomScript;

	// Prefabs
	[SerializeField] public GameObject roomPrefab;

	// Information Fields
	[SerializeField] PointExtraInfo entryPoint;
	[SerializeField] PointExtraInfo exitPoint ;
	[SerializeField] RoomArray      rooms     ;

	// Settings
	[SerializeField] bool disconnectPrefabInstance;
	[SerializeField] float   pointGap;
	[SerializeField] Vector2 roomSize;
	[SerializeField] Vector2 minimumPlatformSize;

	// Properties
	public Vector2 MinimumPlatformSize { get { return minimumPlatformSize;} }
	public Vector2 RoomSize            { get { return roomSize           ;} }

	// Methods ---------------------------------------------------------------------------------------------------
	// Print the rooms
	public void PrintRooms() { rooms.PrintArray (); }

	// Reset the room array and spawn in a new room
	public void Reset() {
		Debug.Log ("resetting");
		rooms.Reset ();
		GenerateUniqueId ();
		RoomGeneratorRoomHelper newRoom = CreateRoom (new Vector2(0, 0), PointDTO.Direction.None);
		AddRoom (new Vector2(0, 0), PointDTO.Direction.None, newRoom);
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
	public RoomGeneratorRoomHelper CreateRoom(Vector2 position, PointDTO.Direction side) {
		Debug.Log ("create");
		Vector2 spawnPosition = position;
		spawnPosition += UsefulMethods.vectorProduct(RoomSize, PointDTO.GetDirectionVector(side));

		GameObject roomObj = Instantiate (roomPrefab, spawnPosition, Quaternion.identity) as GameObject;
		roomObj.transform.SetParent (transform, false);
		roomObj.transform.SetAsLastSibling ();

		RoomGeneratorRoomHelper roomHelper = roomObj.GetComponent<RoomGeneratorRoomHelper> ();
		roomHelper.roomGenerator = this;

		return roomHelper;
	}

	/**
	 * Adds a room to the room array and makes sure everything goes well.
	 * It also checks nearby rooms and corners to disable the walls
	 */
	public void AddRoom(Vector2 originalPosition, PointDTO.Direction direction, RoomGeneratorRoomHelper newRoom) {
		if (rooms.IsNull()) Reset ();

		Vector2 desiredPosition = originalPosition + PointDTO.GetDirectionVector (direction);
		if      (desiredPosition.y >= rooms.Rows) rooms.AddRowToTop    ();
		else if (desiredPosition.y <           0) rooms.AddRowToBottom ();
		if      (desiredPosition.x >= rooms.Cols) rooms.AddRowToRight  ();
		else if (desiredPosition.x <           0) rooms.AddRowToLeft   ();

		// If we are adding a row to the left or bottom, the new position will be 0
		Vector2 newPosition = new Vector2(0, 0);
		if (desiredPosition.x < 0) { desiredPosition.x = 0; newPosition.x = 1; }
		if (desiredPosition.y < 0) { desiredPosition.y = 0; newPosition.y = 1; }

		Vector2 position = newRoom.transform.position;
		position += UsefulMethods.vectorProduct (newPosition, RoomSize);
		newRoom.transform.position = position;

		newRoom.index = desiredPosition;
		newRoom.SetName ();

		rooms.SetRoom(desiredPosition, newRoom);

		CheckForNearbyRooms (desiredPosition, PointDTO.Direction.Top   );
		CheckForNearbyRooms (desiredPosition, PointDTO.Direction.Bottom);
		CheckForNearbyRooms (desiredPosition, PointDTO.Direction.Left  );
		CheckForNearbyRooms (desiredPosition, PointDTO.Direction.Right );

		CheckCorner (desiredPosition, PointDTO.Direction.Top   , PointDTO.Direction.Right, 0f);
		CheckCorner (desiredPosition, PointDTO.Direction.Top   , PointDTO.Direction.Left , 0f);
		CheckCorner (desiredPosition, PointDTO.Direction.Bottom, PointDTO.Direction.Right, 0f);
		CheckCorner (desiredPosition, PointDTO.Direction.Bottom, PointDTO.Direction.Left , 0f);

		// Make the editor select the new room
		Selection.objects = new Object[] { newRoom.gameObject };
	}

	/**
	 * Deletes a room from the array and makes sure everything goes well.
	 * It also checks nearby rooms and corners to re-enable them if required.
	 */
	public bool DeleteRoom (Vector2 position) {
		if (rooms.IsNull()) Reset ();

		if (rooms.ThereIsOnlyOneRoom()) { Debug.LogError ("Cannot destroy the only room"); return false;}

		rooms.SetRoom (position, null);

		// Check corners first so that the nearby room check sets the right dimensions
		CheckCorner (position, PointDTO.Direction.Top   , PointDTO.Direction.Right, MinimumPlatformSize.x);
		CheckCorner (position, PointDTO.Direction.Top   , PointDTO.Direction.Left , MinimumPlatformSize.x);
		CheckCorner (position, PointDTO.Direction.Bottom, PointDTO.Direction.Right, MinimumPlatformSize.x);
		CheckCorner (position, PointDTO.Direction.Bottom, PointDTO.Direction.Left , MinimumPlatformSize.x);

		CheckForNearbyRoomsForDelete (position, PointDTO.Direction.Top   );
		CheckForNearbyRoomsForDelete (position, PointDTO.Direction.Bottom);
		CheckForNearbyRoomsForDelete (position, PointDTO.Direction.Left  );
		CheckForNearbyRoomsForDelete (position, PointDTO.Direction.Right );

		rooms.DeleteUselessSpots ();

		return true;
	}

	/**
	 * Checks if there is a neighbour in a given direction.
	 * If there is, it will disable its points and set the right scales.
	 */
	public void CheckForNearbyRooms(Vector2 index, PointDTO.Direction side) {
		Vector2 desiredPosition = index + PointDTO.GetDirectionVector (side);

		if (rooms.isNotAValidPosition (desiredPosition)) return;

		RoomGeneratorRoomHelper neighbour = rooms.RoomAt(desiredPosition);
		RoomGeneratorRoomHelper actual    = rooms.RoomAt(index          );

		if (neighbour != null) {
			actual.pointRefs   .TurnOff   (side);
			actual.platformRefs.SetXScale (side, MinimumPlatformSize.x);

			neighbour.pointRefs   .TurnOff   (PointDTO.GetOpposite(side));
			neighbour.platformRefs.SetXScale (PointDTO.GetOpposite(side), MinimumPlatformSize.x);
		}
	}

	/**
	 * Checks if there is a neighbour in a given direction.
	 * If there is, it will enable its points and set the right scales.
	 */
	public void CheckForNearbyRoomsForDelete(Vector2 index, PointDTO.Direction side) {
		Vector2 desiredPosition = index + PointDTO.GetDirectionVector (side);

		if (rooms.isNotAValidPosition (desiredPosition)) return;

		RoomGeneratorRoomHelper neighbour = rooms.RoomAt (desiredPosition);

		if (neighbour != null) {
			float size = (side == PointDTO.Direction.Top || side == PointDTO.Direction.Bottom) ? RoomSize.x / 2 : RoomSize.y / 2;

			neighbour.pointRefs   .TurnOn    (PointDTO.GetOpposite(side));
			neighbour.platformRefs.SetXScale (PointDTO.GetOpposite(side), size);

			neighbour.ResetNeighbour (PointDTO.GetOpposite(side));
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
	void CheckCorner(Vector2 roomIndex, PointDTO.Direction dir1, PointDTO.Direction dir2, float size) {
		if (dir1 != PointDTO.Direction.Top && dir1 != PointDTO.Direction.Bottom) {
			Debug.LogError ("Direction 1 cannot be left or right"); return;
		}

		if (dir2 != PointDTO.Direction.Left && dir2 != PointDTO.Direction.Right) {
			Debug.LogError ("Direction 2 cannot be top or bottom"); return;
		}
		int index1, index2, index3, index4;
		if      (dir1 == PointDTO.Direction.Top    && dir2 == PointDTO.Direction.Right)  { index1 = 2; index2 = 2; index3 = 1; index4 = 1; }
		else if (dir1 == PointDTO.Direction.Top    && dir2 == PointDTO.Direction.Left )  { index1 = 1; index2 = 2; index3 = 2; index4 = 1; }
		else if (dir1 == PointDTO.Direction.Bottom && dir2 == PointDTO.Direction.Left )  { index1 = 1; index2 = 1; index3 = 2; index4 = 2; }
		else  /*(dir1 == PointDTO.Direction.Bottom && dir2 == PointDTO.Direction.Right)*/{ index1 = 2; index2 = 1; index3 = 1; index4 = 2; }

		Vector2 direction1 = PointDTO.GetDirectionVector (dir1);
		Vector2 direction2 = PointDTO.GetDirectionVector (dir2);

		Vector2 position1 = roomIndex + direction1;
		Vector2 position2 = roomIndex + direction2;
		Vector2 position3 = roomIndex + direction1 + direction2;
		if (rooms.isValidPosition (position1) && rooms.isValidPosition (position2) && rooms.isValidPosition (position3)) {
			if (rooms.RoomAt (position1) != null && rooms.RoomAt (position2) != null && rooms.RoomAt (position3) != null) {
				rooms.RoomAt (position1).platformRefs.SetXScale (PointDTO.GetOpposite(dir1), index1, size);
				rooms.RoomAt (position1).platformRefs.SetXScale (dir2, dir1 == PointDTO.Direction.Top   ? 1 : 2, size);

				rooms.RoomAt (position2).platformRefs.SetXScale (PointDTO.GetOpposite(dir2), index2, size);
				rooms.RoomAt (position2).platformRefs.SetXScale (dir1, dir2 == PointDTO.Direction.Right ? 1 : 2, size);

				rooms.RoomAt (position3).platformRefs.SetXScale (PointDTO.GetOpposite(dir1), index3, size);
				rooms.RoomAt (position3).platformRefs.SetXScale (PointDTO.GetOpposite(dir2), index4, size);

				// Also check the room itself
				if (rooms.isValidPosition (roomIndex) && rooms.RoomAt (roomIndex) != null) {
					rooms.RoomAt (roomIndex).platformRefs.SetXScale (dir1, index1, size);
					rooms.RoomAt (roomIndex).platformRefs.SetXScale (dir2, index2, size);
				}
			}
		}
	}

	public void AddRoomToCache() {
		if (roomScript.id == 0) 
			GenerateUniqueId ();

		if(roomScript.levelGenerator == null)
			roomScript.levelGenerator = FindObjectOfType<LevelGenerator> ();
		
		roomCache.AddNewRoom (roomScript);
	}

	public void GenerateUniqueId() {
		gameObject.name = "Room " + gameObject.GetInstanceID ();
		roomScript.id = gameObject.GetInstanceID ();
	}

	// Handling Exit and Enter arrows ----------------------------------------------------------------------------
	/**
	 * Most likely modify this
	 */
	void HandlePlatformSize(string minPointName, PointExtraInfo pointExtraInfo, PointDTO pointToModify, PointHelper pointHelper, RoomGeneratorRoomHelper.PlatformRefs platformRefs) {
		float newSize;
		GameObject platform1;
		GameObject platform2;
		if (minPointName.Contains ("Top")) {
			pointToModify.main = PointDTO.Direction.Top;
			if(minPointName.Contains("1")) {
				platform1 = platformRefs.top1;
				platform2 = platformRefs.top2;
				pointToModify.secondary = PointDTO.Direction.Left;
			}
			else {
				platform1 = platformRefs.top2;
				platform2 = platformRefs.top1;
				pointToModify.secondary = PointDTO.Direction.Right;
			}
			newSize = RoomSize.x / 2;
		}
		else if (minPointName.Contains ("Bottom")) {
			pointToModify.main = PointDTO.Direction.Bottom;
			if(minPointName.Contains("1")) {
				platform1 = platformRefs.bottom1;
				platform2 = platformRefs.bottom2;
				pointToModify.secondary = PointDTO.Direction.Left;
			}
			else {
				platform1 = platformRefs.bottom2;
				platform2 = platformRefs.bottom1;
				pointToModify.secondary = PointDTO.Direction.Right;
			}
			newSize = RoomSize.x / 2;
		}
		else if (minPointName.Contains ("Left")) {
			pointToModify.main = PointDTO.Direction.Left;
			if(minPointName.Contains("1")) {
				platform1 = platformRefs.left1;
				platform2 = platformRefs.left2;
				pointToModify.secondary = PointDTO.Direction.Bottom;
			}
			else {
				platform1 = platformRefs.left2;
				platform2 = platformRefs.left1;
				pointToModify.secondary = PointDTO.Direction.Top;
			}
			newSize = RoomSize.y / 2;
		}
		else { //minPointName.Contains ("Right")
			pointToModify.main = PointDTO.Direction.Right;
			if(minPointName.Contains("1")) {
				platform1 = platformRefs.right1;
				platform2 = platformRefs.right2;
				pointToModify.secondary = PointDTO.Direction.Bottom;
			}
			else {
				platform1 = platformRefs.right2;
				platform2 = platformRefs.right1;
				pointToModify.secondary = PointDTO.Direction.Top;
			}
			newSize = RoomSize.y / 2;
		}
		if (pointExtraInfo.previousPoint != null && pointExtraInfo.previousPoint.gameObject.activeSelf) {
			pointExtraInfo.previousPlatform1.transform.localScale = new Vector2 (pointExtraInfo.previousSize, pointExtraInfo.previousPlatform1.transform.localScale.y);
			pointExtraInfo.previousPlatform2.transform.localScale = new Vector2 (pointExtraInfo.previousSize, pointExtraInfo.previousPlatform2.transform.localScale.y);
		}

		platform1.transform.localScale = new Vector2 (0.5f, platform1.transform.localScale.y);
		platform2.transform.localScale = new Vector2 (newSize * 2 - pointGap, platform2.transform.localScale.y);

		pointExtraInfo.previousSize = newSize;
		pointExtraInfo.previousPoint = pointHelper.transform;
		pointExtraInfo.previousPlatform1 = platform1;
		pointExtraInfo.previousPlatform2 = platform2;
	}

	void Update () {
		if (roomScript.id == 0) 
			GenerateUniqueId ();
		
		if (rooms.IsNull ()) {
			Debug.Log ("yes");
			Reset ();
		}

		roomScript.size.x = rooms.Cols;
		roomScript.size.y = rooms.Rows;

		if(!roomScript.inUse)
			this.roomScript.previousPosition = transform.localPosition;
		
		// Gather all points from all rooms
		List<PointHelper> validPoints = new List<PointHelper>();
		for (int i = 0; i < rooms.Rows; i++) {
			for (int j = 0; j < rooms.Cols; j++) {
				RoomGeneratorRoomHelper room = rooms.RoomAt (new Vector2 (j, i));
				if(room != null)
					validPoints.AddRange (room.pointRefs.points);
			}
		}

		// Handle exit point
		float minDistance = float.MaxValue;
		PointHelper minPoint = null;
		foreach (PointHelper point in validPoints) {
			if (point.gameObject.activeSelf) {
				float pointDistance = Vector2.Distance (exitPoint.point.transform.position, point.transform.position);
				if (pointDistance < minDistance) {
					minDistance = pointDistance;
					minPoint = point;
				}
			}
		}
		if (minPoint != null) {
			exitPoint.point.transform.position = minPoint.transform.position;
			exitPoint.point.transform.rotation = minPoint.transform.rotation;
			roomScript.exit.roomIndex = minPoint.roomEditor.index;
			HandlePlatformSize (minPoint.name, exitPoint, roomScript.exit, minPoint, minPoint.roomEditor.platformRefs);
		}

		// Handle entry point
		validPoints.Remove(minPoint);
		minDistance = float.MaxValue;
		minPoint = null;
		foreach (PointHelper point in validPoints) {
			if (point.gameObject.activeSelf) {
				float pointDistance = Vector2.Distance (entryPoint.point.transform.position, point.transform.position);
				if (pointDistance < minDistance) {
					minDistance = pointDistance;
					minPoint = point;
				}
			}
		}
		if (minPoint != null) {
			entryPoint.point.transform.position = minPoint.transform.position;
			entryPoint.point.transform.rotation = minPoint.transform.rotation;
			roomScript.entry.roomIndex = minPoint.roomEditor.index;
			HandlePlatformSize (minPoint.name, entryPoint, roomScript.entry, minPoint, minPoint.roomEditor.platformRefs);
		}
	}
	// -----------------------------------------------------------------------------------------------------------
}
#endif