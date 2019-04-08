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
	[SerializeField] FollowPlayer cameraScript          ;
	[SerializeField] Room         startingRoom          ;
	[SerializeField] Transform    startingPlayerLocation;
	[SerializeField] Room         tutorialRoom          ;
	[SerializeField] Transform    tutorialPlayerLocation;
	  
	// Settings
	[SerializeField] int  maxColumns        ; // Max columns the tower will have
	[SerializeField] bool returnRoomsToCache; // Should rooms be returned to the cache after they have been used?

	// Readonly
	[SerializeField][ReadOnly] int currentColumn;
	[SerializeField][ReadOnly] int currentHeight;

	// Information
	[SerializeField] Room previousRoom;
	[SerializeField] Room currentRoom ;
	[SerializeField] Room nextRoom    ;

	void Awake() { InitiateSingleton (); }

	// Generate the first room after a small delay
	void Start () { 
		if (DataSaver.Instance.FinishedTutorial) { StartCoroutine (NormalSetUp   (true, 0.25f)); } 
		else /*Not Finished Tutorial*/           { StartCoroutine (TutorialSetUp ()); }
	}

	/**
	 * For the tutorial, we just move the player and set the camera to the right room.
	 */
	IEnumerator TutorialSetUp () {
		currentRoom = tutorialRoom;

		// Camera stuff
		cameraScript.transform.position = currentRoom.Rooms.RoomAt (currentRoom.Entry.RoomIndex).Middle.position;
		cameraScript.SetNewRoom (currentRoom);

		// Player stuff
		PlayerController.Instance.transform.position = tutorialPlayerLocation.position;
		ScoreSystem.Instance.ShowTutorialUI ();

		yield return new WaitForEndOfFrame ();
	}

	/**
	 * For normal gameplay, we must generate the next room and be ready for when the player enters it.
	 */
	IEnumerator NormalSetUp(bool movePlayer, float initialDelay) {	
		// Close the tutorial door
		currentRoom.Entry.Door.SetTrigger("close");

		// Delay a little bit so everything gets loaded?
		yield return new WaitForSeconds (initialDelay);
		currentHeight = 0;
		currentColumn = maxColumns - 1;
		currentRoom   = startingRoom;
		nextRoom = GenerateRandomRoom(currentRoom);

		// Camera stuff
		cameraScript.SetNewRoom (currentRoom);

		ScoreSystem.Instance.HideTutorialUI ();

		// Player stuff
		if (movePlayer) {
			cameraScript.transform.position = currentRoom.Rooms.RoomAt (currentRoom.Entry.RoomIndex).Middle.position;
			PlayerController.Instance.transform.position = startingPlayerLocation.position;
		}
	}

	// Whenever the player finished the tutorial
	public void FinishTutorial() {
		PlayerController.Instance.EnterRoom (startingRoom, currentRoom.Exit.Main);
		DataSaver.Instance.FinishedTutorial = true;
		StartCoroutine (NormalSetUp (false, 0f));
	}

	// Whenever the player starts a game session
	public void StartGame() {
		SessionData.Instance.StartGame  ();
		ScoreSystem.Instance.ShowGameUI ();
		ScoreSystem.Instance.LoadHP ();
		ScoreSystem.Instance.Reset (); 
	}

	// Whenever the player ends a game session
	public void FinishGame() {
		SessionData.Instance.SaveStats      ();
		PlayerController.Instance.gameObject.SetActive (true);
		PlayerController.Instance.Reset ();
		ScoreSystem.Instance.HideGameoverUI ();
		ScoreSystem.Instance.HideGameUI     ();
		ShopKeeper.Instance.LoadDeathQuote ();
		if (previousRoom != null && previousRoom != startingRoom) {
			//previousRoom.FinishRoom ();
			RoomCache.Instance.ReturnRoomToCache (previousRoom);
		}
		if (currentRoom != null && currentRoom != startingRoom) {
			//currentRoom.FinishRoom ();
			RoomCache.Instance.ReturnRoomToCache (currentRoom);
		}
		if (nextRoom != null && nextRoom != startingRoom) {
			//nextRoom.FinishRoom ();
			RoomCache.Instance.ReturnRoomToCache (nextRoom);
		}
		cameraScript.transform.position = currentRoom.Rooms.RoomAt (startingRoom.Entry.RoomIndex).Middle.position;
		startingRoom.Entry.Door.SetTrigger ("close");
		StartCoroutine (NormalSetUp   (true, 0.1f));
	}
    
	// When the player enters a new room, delete the previous and generate a new one
	public void WhenPlayerEntersNewRoom(Room room) {
		PlayerController.Instance.EnterRoom (room, currentRoom.Exit.Main);
		if (DataSaver.Instance.FinishedTutorial) {
			// Return the room to the cache, but dont do it the first time (since its the starting room)
			if (previousRoom != null && previousRoom != startingRoom && returnRoomsToCache) {
				RoomCache.Instance.ReturnRoomToCache (previousRoom);
			}

			previousRoom = currentRoom;
			currentRoom = nextRoom;
			nextRoom = GenerateRandomRoom (currentRoom);
		}

		cameraScript.SetNewRoom (room);
	}

	// Whenever the player finishes a room, he gets a reward.
	public void WhenPlayerFinishesARoom(Room room) {
		ScoreSystem.Instance.GainScore (1000);
	}
		
	// Generate a new room and move it to the right spot.
	Room GenerateRandomRoom(Room currentRoom) {
		Vector2 newRoomDirection = DirectionHelper.GetDirectionVector (currentRoom.Exit.Main);
		int leftDistance  = currentColumn + 1 + (int)newRoomDirection.x;
		int rightDistance = maxColumns - currentColumn - (int)newRoomDirection.x;

		Room room = RoomCache.Instance.GetRandomRoom (currentRoom.Exit.Main, currentRoom.Exit.Secondary, leftDistance, rightDistance);

		// Uhhh, don't question this but it works
		int offset = (int)room.Entry.RoomIndex.x;
		Vector2 currentIndex = new Vector2 (currentColumn, currentHeight) + DirectionHelper.GetDirectionVector (currentRoom.Exit.Main);
		currentIndex.x -= offset;
		room.transform.position = UsefulMethods.vectorProduct (currentIndex, RoomCache.Instance.RoomSize);
		currentIndex.x += offset;

		// This too
		Vector2 newIndexOffset = currentIndex + room.Exit.RoomIndex - room.Entry.RoomIndex;
		currentHeight = (int)newIndexOffset.y;
		currentColumn = (int)newIndexOffset.x;

		return room;
	}
}
