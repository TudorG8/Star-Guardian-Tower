using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RoomCache : MonoBehaviour {
	[SerializeField] List<Room> instantiatedRooms;

	int GetRoomId(long id) {
		for (int i = 0; i < instantiatedRooms.Count; i++) {
			if (instantiatedRooms [i].id == id) {
				return i;
			}
		}
		return -1;  
	}

	public void AddNewRoom(Room room) {
		long id = long.Parse (room.name.Substring (4));
		int roomIndex = GetRoomId (id);

		if (roomIndex == -1) {
			instantiatedRooms.Add (room);
		} 
		else {
			instantiatedRooms [roomIndex] = room;
		}
		room.transform.SetParent (this.transform, true);  
	}

	public void PrintCache() {
		Debug.Log ("Super Cache with " + instantiatedRooms.Count + " rooms");
		for (int i = 0; i < instantiatedRooms.Count; i++) {
			Debug.Log (instantiatedRooms [i].name);
		}
	}
	 
	public void Update () {
		if (instantiatedRooms == null) {
			instantiatedRooms = new List<Room> ();
			Debug.LogError ("Empty Cache");
		}
		for (int i = 0; i < instantiatedRooms.Count; i++) {
			if (instantiatedRooms [i] == null) {
				instantiatedRooms.RemoveAt (i);
				break;
			}
		}
	}

	public void Reset() {
		while (transform.childCount != 0) {
			DestroyImmediate (transform.GetChild (0).gameObject);
		}
		instantiatedRooms = new List<Room> ();
	}

	public void RemoveRoom(Room room) {
		if (room == null) {
			Debug.LogError ("Cannot delete a null room.");
			return;
		}

		long id = long.Parse (room.name.Substring (4));
		int roomIndex = GetRoomId (id);

		if (roomIndex == -1)
			Debug.LogError ("Room does not exist in list.");
		else {
			instantiatedRooms.RemoveAt (roomIndex);
			DestroyImmediate (room.gameObject);
		}
	}

	public Room GetRandomRoom(PointDTO.Direction main, PointDTO.Direction secondary, int leftDistance, int rightDistance) {
		List<Room> rooms = new List<Room> ();
		for (int i = 0; i < instantiatedRooms.Count; i++) {
			Room room = instantiatedRooms [i];
			if (room.inUse)
				continue;

			if (room.entry.main == PointDTO.GetOpposite (main) && room.entry.secondary == secondary) {
				Vector2 entryIndex = room.entry.roomIndex;
				int leftSize = (int)entryIndex.x;
				int rightSize = (int)(room.size.x - entryIndex.x + 1);

				if (room.exit.roomIndex.x == 0 && room.exit.main == PointDTO.Direction.Left)
					leftSize++;
				else if (room.exit.roomIndex.x == room.size.x - 1 && room.exit.main == PointDTO.Direction.Right)
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

		chosenRoom.inUse = true;

		return chosenRoom;
	}

	public void ReturnRoomToCache(Room room) {
		room.transform.localPosition = room.previousPosition;
		room.entry.triggerScript.triggered = false;
		room.inUse = false;
	}
}
