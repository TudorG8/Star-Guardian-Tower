using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {
	public Transform tower;
    public int roomsToGenerate;
	public int maxColumns;
    public List<Room> rooms;
	public Room currentRoom;
	public int currentColumn;
	public int currentHeight;

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
		currentColumn = maxColumns - 1;
		currentHeight = 0;
		CalculateDictionary ();
        int generatedRooms = 0;
        while(generatedRooms < roomsToGenerate) {
			Room.DirectionNames exit = currentRoom.exitPoint;
			Vector2 newPositionDirection = Room.GetDirectionVector (exit);
			currentColumn += (int)newPositionDirection.x;
			currentHeight += (int)newPositionDirection.y;
			List<Room> possibleRooms = entryPoints [Room.GetOpposite(exit)];
			List<Room> validRooms = new List<Room> ();
			foreach (Room room in possibleRooms) {
				if (currentColumn == 0) {
					if (room.exitPoint == Room.DirectionNames.LeftTop || room.exitPoint == Room.DirectionNames.LeftBottom) {
						continue;
					}
				} 
				if (currentColumn == maxColumns - 1) {
					if (room.exitPoint == Room.DirectionNames.RightTop || room.exitPoint == Room.DirectionNames.RightBottom) {
						continue;
					}
				}
				validRooms.Add (room);

			}
			int randomRoomIndex = Random.Range (0, validRooms.Count);

			Room newRoom = validRooms [randomRoomIndex];
			Vector2 location = 
				(Vector2)currentRoom.transform.position + 
				vectorProduct(Room.GetDirectionVector (exit), roomSize);
			;
			GameObject roomObj = Instantiate (newRoom.gameObject, location, Quaternion.identity) as GameObject;
			currentRoom = roomObj.GetComponent<Room>();
			generatedRooms++;
        }
		tower.localScale = new Vector2 (80, 15 * (currentHeight + 1));
    }
}
