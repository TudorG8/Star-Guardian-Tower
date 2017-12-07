using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {
    public int roomsToGenerate;
    public List<Room> rooms;
	public Room currentRoom;

	public Vector2 roomSize = new Vector2 (20, 15);

	Dictionary<Room.DirectionNames, List<Room>> entryPoints;

	void CalculateDictionary() {
		entryPoints = new Dictionary<Room.DirectionNames, List<Room>> ();
		foreach (Room room in rooms) {
			if (!entryPoints.ContainsKey(room.entryPoint))
				entryPoints [room.entryPoint] = new List<Room> ();
			entryPoints [room.entryPoint].Add (room);
		}
	}
	Vector2 vectorProduct(Vector2 a, Vector2 b) {
		return new Vector2 (a.x * b.x, a.y * b.y);
	}
    void Awake() {
		CalculateDictionary ();
        int generatedRooms = 0;
        while(generatedRooms < roomsToGenerate) {
			Debug.Log (currentRoom.exitPoint);
			Room.DirectionNames exit = currentRoom.exitPoint;
			List<Room> possibleRooms = entryPoints [exit];
			int randomRoomIndex = Random.Range (0, possibleRooms.Count);

			Room newRoom = possibleRooms [randomRoomIndex];
			Vector2 location = 
				(Vector2)currentRoom.transform.position + 
				vectorProduct(Room.GetDirectionVector (exit), roomSize);
			;
			GameObject roomObj = Instantiate (newRoom.gameObject, location, Quaternion.identity) as GameObject;
			currentRoom = roomObj.GetComponent<Room>();
			generatedRooms++;
        }
    }
}
