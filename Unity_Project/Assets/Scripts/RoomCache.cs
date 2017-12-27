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
		room.transform.SetParent (this.transform, false);  
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

	public void GetRandomRoom(PointDTO point) {
		
	}
}
