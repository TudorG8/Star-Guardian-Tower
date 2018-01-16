using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPropertyDrawers;

/**
 * Level generator for the game. A new room is generated each time the player advances from a room to another.
 * Rooms are chosen from the cache.
 * It holds the previous, current and next room to make sure there is always a room to transition to.
 */

public class LevelGenerator : Singleton<LevelGenerator> {
	// Imports
	[SerializeField] FollowPlayer cameraScript;
	[SerializeField] Room         startingRoom;
	[SerializeField] Transform    startingPlayerLocation;
	[SerializeField] Room         tutorialRoom;
	[SerializeField] Transform    tutorialPlayerLocation;
	[SerializeField] Transform    tower       ;

	// Settings
	[SerializeField] int maxColumns;

	// Readonly
	[SerializeField][ReadOnly] int currentColumn;
	[SerializeField][ReadOnly] int currentHeight;

	// Information
	[SerializeField] Room previousRoom;
	[SerializeField] Room currentRoom ;
	[SerializeField] Room nextRoom    ;

	public Vector2 roomSize = new Vector2 (26.7f, 15f);

	void Awake() { InitiateSingleton (); }

	// Generate the first room after a small delay
	void Start () { 
		if (DataSaver.Instance.FinishedTutorial) { StartCoroutine (NormalSetUp   ()); } 
		else /*Not Finished Tutorial*/           { StartCoroutine (TutorialSetUp ()); }
	}

	IEnumerator TutorialSetUp () {
		// Delay a little bit so everything gets loaded?
		yield return new WaitForSeconds (0.25f);
		currentRoom = tutorialRoom;

		// Camera stuff
		cameraScript.transform.position = currentRoom.Rooms.RoomAt (currentRoom.Entry.roomIndex).Middle.position;
		cameraScript.SetNewRoom (currentRoom);

		// Player stuff
		PlayerController.Instance.transform.position = tutorialPlayerLocation.position;
	}

	IEnumerator NormalSetUp() {
		// Delay a little bit so everything gets loaded?
		yield return new WaitForSeconds (0.25f);
		currentHeight = 0;
		currentColumn = maxColumns - 1;
		currentRoom   = startingRoom;
		nextRoom = GenerateRandomRoom(currentRoom);

		// Camera stuff
		cameraScript.transform.position = currentRoom.Rooms.RoomAt (currentRoom.Entry.roomIndex).Middle.position;
		cameraScript.SetNewRoom (currentRoom);

		// Player stuff
		//SessionData.Instance.Reset ();
		//PlayerController.Instance.UpdateAttackRange (Shop.Instance.GetWeapon());
		//PlayerController.Instance.UpdateLives       (Shop.Instance.GetLives ());
		PlayerController.Instance.transform.position = startingPlayerLocation.position;
	}
    
	// When the player enters a new room, delete the previous and generate a new one
	public void WhenPlayerEntersNewRoom(Room room) {
		if (DataSaver.Instance.FinishedTutorial) {
			// Return the room to the cache, but dont do it the first time (since its the starting room)
			if (previousRoom != null) {
				RoomCache.Instance.ReturnRoomToCache (previousRoom);
			}

			previousRoom = currentRoom;
			currentRoom = nextRoom;
			nextRoom = GenerateRandomRoom (currentRoom);
		}

		Debug.Log ("yes");

		cameraScript.SetNewRoom (room);
	}
		
	// Generate a new room and move it to the right spot.
	Room GenerateRandomRoom(Room currentRoom) {
		Vector2 newRoomDirection = DirectionHelper.GetDirectionVector (currentRoom.Exit.main);
		int leftDistance  = currentColumn + 1 + (int)newRoomDirection.x;
		int rightDistance = maxColumns - currentColumn - (int)newRoomDirection.x;

		Room room = RoomCache.Instance.GetRandomRoom (currentRoom.Exit.main, currentRoom.Exit.secondary, leftDistance, rightDistance);

		// Uhhh, don't question this but it works
		int offset = (int)room.Entry.roomIndex.x;
		Vector2 currentIndex = new Vector2 (currentColumn, currentHeight) + DirectionHelper.GetDirectionVector (currentRoom.Exit.main);
		currentIndex.x -= offset;
		room.transform.position = UsefulMethods.vectorProduct (currentIndex, roomSize);
		currentIndex.x += offset;

		// This too
		Vector2 newIndexOffset = currentIndex + room.Exit.roomIndex - room.Entry.roomIndex;
		currentHeight = (int)newIndexOffset.y;
		currentColumn = (int)newIndexOffset.x;

		return room;
	}
}
