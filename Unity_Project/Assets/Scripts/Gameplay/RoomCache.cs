using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Cache for the rooms that allows the level generator to select a random room on need.
 * Rooms are not spawned in but rather already exist and are just moved to the right position.
 * Also some editor stuff to make it easier.
 */

[ExecuteInEditMode]
public class RoomCache : Singleton<RoomCache> {
	[SerializeField] Vector2    roomSize           ;
	[SerializeField] float      pointGap           ;
	[SerializeField] Vector2    minimumPlatformSize;
	[SerializeField] GameObject roomPrefab         ;

	[SerializeField] List<Room> instantiatedRooms;

	// Properties
	public Vector2    RoomSize            { get { return roomSize           ; } }
	public float      PointGap            { get { return pointGap           ; } }
	public Vector2    MinimumPlatformSize { get { return minimumPlatformSize; } }
	public GameObject RoomPrefab          { get { return roomPrefab         ; } }

	void Awake() { InitiateSingleton (); }

	/**
	 * Get a random room from the cache that meets the right criteria:
	 * 		Its entries main must be the opposite of the previous room exit.
	 * 		Its entries secondary must be the same as the previous room exit.
	 * 		The distance to either left or right must not go outside the limit.
	 * Marks the chosen room as in use so it is not selected again.
	 */
	public Room GetRandomRoom(Direction main, Direction secondary, int leftDistance, int rightDistance) {
		List<Room> rooms = new List<Room> ();
		for (int i = 0; i < instantiatedRooms.Count; i++) {
			Room room = instantiatedRooms [i];
			if (room.InUse)
				continue;

			if (room.Entry.Main == DirectionHelper.GetOpposite (main) && room.Entry.Secondary == secondary) {
				Vector2 entryIndex = room.Entry.RoomIndex;
				int leftSize  = (int)entryIndex.x + 1;
				int rightSize = (int)(room.Size.x - entryIndex.x);

				// If its a room to the left and the exit is on the left, size is bigger by one
				if (room.Exit.RoomIndex.x == 0 && room.Exit.Main == Direction.Left)
					leftSize++;
				// If its a room to the right and the exit is on the right, size is bigger by one
				else if (room.Exit.RoomIndex.x == room.Size.x - 1 && room.Exit.Main == Direction.Right)
					rightSize++;

				if (leftSize > leftDistance || rightSize > rightDistance)
					continue;
			} 
			else
				continue;

			rooms.Add (room);
		}

		if (rooms.Count == 0) {
			Debug.LogError ("No Available Rooms");
			return null;
		}
		int random = Random.Range (0, rooms.Count);
		Room chosenRoom = rooms [random];

		chosenRoom.InUse = true;

		return chosenRoom;
	}

	// Once a room is no longer needed, it can be returned to the cache, which resets its position.
	public void ReturnRoomToCache(Room room) {
		room.Reset ();
	}

	// Editor Stuff -------------------------------------------------------------------------------------------
	#if UNITY_EDITOR
	public void Update () {
		if (Instance == null) InitiateSingleton (); 
		// Check if the room list is null by any chance
		if (instantiatedRooms == null) {
			instantiatedRooms = new List<Room> ();
			Debug.LogError ("Empty Cache");
		}
		// Check if a room was deleted in the editor
		for (int i = 0; i < instantiatedRooms.Count; i++) {
			if (instantiatedRooms [i] == null) {
				instantiatedRooms.RemoveAt (i);
				break;
			}
		}
		// Check if a room was added in the editor
		foreach (Transform child in transform) {
			Room room = child.GetComponent<Room> ();
			AddNewRoom (room);
		}
	}

	// Get the index of a room based on its id
	int GetRoomIndex(long id) {
		for (int i = 0; i < instantiatedRooms.Count; i++) {
			if (instantiatedRooms [i].Id == id) {
				return i;
			}
		}
		return -1;  
	}
		
	/**
	 * Adds a new room to the cache.
	 * If it already is in the cache, it will replace it.
	 */
	public void AddNewRoom(Room room) {
		int roomIndex = GetRoomIndex (room.Id);

		if (roomIndex == -1) {
			instantiatedRooms.Add (room);
		} 
		else {
			instantiatedRooms [roomIndex] = room;
		}
		room.transform.SetParent (this.transform, true);  
	}

	/**
	 * Removes a room from the cache.
	 * Also destroys its associated gameobject.
	 */
	public void RemoveRoom(Room room) {
		if (room == null) {
			Debug.LogError ("Cannot delete a null room.");
			return;
		}
			
		int roomIndex = GetRoomIndex (room.Id);

		if (roomIndex == -1)
			Debug.LogError ("Room does not exist in list.");
		else {
			instantiatedRooms.RemoveAt (roomIndex);
			DestroyImmediate (room.gameObject);
		}
	}

	// Print the contents of the cache
	public void PrintCache() {
		Debug.Log ("Super Cache with " + instantiatedRooms.Count + " rooms");
		for (int i = 0; i < instantiatedRooms.Count; i++) {
			Debug.Log (instantiatedRooms [i].name);
		}
	}

	// Reset the cache and delete all children
	public void Reset() {
		while (transform.childCount != 0) {
			DestroyImmediate (transform.GetChild (0).gameObject);
		}
		instantiatedRooms = new List<Room> ();
	}

	public void SetRoomsAs(bool active) {
		for (int i = 0; i < instantiatedRooms.Count; i++) {
			Room room = instantiatedRooms [i];
			room.RoomGeneratorScript.SetActive (active);
		}
	}

	public void ActivateAllRooms(bool active) {
		for (int i = 0; i < instantiatedRooms.Count; i++) {
			Room room = instantiatedRooms [i];
			room.InUse = active;
		}
	}
	#endif
}
// --------------------------------------------------------------------------------------------------------