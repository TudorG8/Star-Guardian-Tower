using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {
	public Transform tower;
    public int roomsToGenerate;
	public int maxColumns;

	public int currentColumn;
	public int currentHeight;

	public Room previousRoom;
	public Room currentRoom ;
	public Room nextRoom    ;

	public Vector2 roomSize = new Vector2 (20, 15);

	public FollowPlayer cameraScript;

	[SerializeField] RoomCache roomCache   ;
	[SerializeField] Room      startingRoom;

	private IEnumerator StartUp() {
		yield return new WaitForSeconds (0.25f);
		currentHeight = 0;
		currentColumn = maxColumns - 1;
		currentRoom = startingRoom;

		nextRoom = GenerateRandomRoom(currentRoom);
	}
    void Start () {
		StartCoroutine (StartUp ());
    }

	public void WhenPlayerEntersNewRoom(Transform roomCenter) {
		if (previousRoom != null) {
			roomCache.ReturnRoomToCache (previousRoom);
		}
		previousRoom = currentRoom;
		currentRoom  = nextRoom   ;
		nextRoom     = GenerateRandomRoom(currentRoom);
		cameraScript.objToFollow = roomCenter;
	}

	Room GenerateRandomRoom(Room currentRoom) {
		int leftDistance  = currentColumn;
		int rightDistance = maxColumns - currentColumn + 1;
		Room room = roomCache.GetRandomRoom (currentRoom.exit.main, currentRoom.exit.secondary, leftDistance, rightDistance);

		int offset = (int)room.entry.roomIndex.x;
		Vector2 currentIndex = new Vector2 (currentColumn, currentHeight) + PointDTO.GetDirectionVector (currentRoom.exit.main);
		currentIndex.x -= offset;
		room.transform.position = UsefulMethods.vectorProduct (currentIndex, roomSize);
		currentIndex.x += offset;

		Debug.Log (currentIndex);
		Vector2 newIndexOffset = currentIndex + room.exit.roomIndex - room.entry.roomIndex;
		currentHeight = (int)newIndexOffset.y;
		currentColumn = (int)newIndexOffset.x;



		return room;
	}

	public void Test() {
		if (nextRoom != null) {
			roomCache.ReturnRoomToCache (nextRoom);
			nextRoom = null;
		} 
		else {
			nextRoom = GenerateRandomRoom (currentRoom);
		}
	}
}
