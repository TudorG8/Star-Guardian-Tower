using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPropertyDrawers;

/**
 * Camera script to follow the player but be restricted by the rooms.
 * To be efficient, we check each neighbour around the current segment.
 * Once a segment becomes close than the current, that is the new point to follow.
 * You can only move the camera towards a neighbour.
 * 	Example: 
 * 		A - (B)
 *  	Player is in B and has A as a neighbour. That means he can move the camera left.
 * 		
 * 		A - (B)
 *  		 |
 *  		 C
 * 		Player is in B and has A and C as a neighbour. That means he can move the camera left and down.
 * 
 * If the player tries to move besides the current point with no neighbours in that direction, the camera will stop.
 */

public class FollowPlayer : MonoBehaviour {
	// Imports
	[SerializeField] Transform player; // The player

	// Settings
	[SerializeField] Vector3 offset   ; // The camera offset
	[SerializeField] float   smoothing; // How smooth following is

	// Read Only
	[SerializeField][ReadOnly] Room        currentRoom   ; // The room the player is in
	[SerializeField][ReadOnly] RoomSegment currentSegment; // The segment the camera is following
	[SerializeField][ReadOnly] Vector3     velocity      ; // Current velocity of the camera
	[SerializeField][ReadOnly] bool canMoveLeft, canMoveRight, canMoveTop, canMoveBottom;
	[SerializeField][ReadOnly] bool checkVerticalChange, checkHorizontalChange;

	// Set the new room to follow when a player enters it.
	public void SetNewRoom(Room room) {
		currentRoom    = room;
		currentSegment = currentRoom.Rooms.RoomAt (room.Entry.RoomIndex);
	}

	// Checks if the given segment is closer to the player as compared to the current point
	bool CheckIfSegmentCloserToPlayer (RoomSegment segment, float distanceToCurrentPoint) {
		float distanceToNeighbour = Vector3.Distance (player.position, segment.Middle.position);
		if (distanceToNeighbour < distanceToCurrentPoint) {
			currentSegment = segment;
			return true;
		}
		return false;
	}

	// Here we check for the neighbour logic
	void LateUpdate () {
		if (currentRoom == null) return;
		float distanceToCurrentPoint = Vector3.Distance (player.position, currentSegment.Middle.position);

		// Check in what directions the camera can move
		canMoveTop = canMoveBottom = canMoveLeft = canMoveRight = false;
		checkHorizontalChange = checkVerticalChange = true;
		RoomSegment top    = currentSegment.SegmentNeighbours.Top   ;
		if (top != null) {
			canMoveTop = true;
			if (CheckIfSegmentCloserToPlayer (top, distanceToCurrentPoint)) {
				checkVerticalChange = false;
			}
		} 
		RoomSegment bottom = currentSegment.SegmentNeighbours.Bottom;
		if (bottom != null) {
			canMoveBottom = true;
			if (CheckIfSegmentCloserToPlayer (bottom, distanceToCurrentPoint)) {
				checkVerticalChange = false;
			}
		}
		RoomSegment left   = currentSegment.SegmentNeighbours.Left  ;
		if (left != null) {
			canMoveLeft = true;
			if (CheckIfSegmentCloserToPlayer (left, distanceToCurrentPoint)) {
				checkHorizontalChange = false;
			}
		}
		RoomSegment right  = currentSegment.SegmentNeighbours.Right ;
		if (right != null) {
			canMoveRight = true;
			if (CheckIfSegmentCloserToPlayer (right, distanceToCurrentPoint)) {
				checkHorizontalChange = false;
			}
		}

		// Restrain position based on the players position and current segment
		Transform objectToFollow = currentSegment.Middle;
		Vector3 targetPosition = player.position + offset;
		if (checkHorizontalChange) {
			if (!canMoveLeft && player.position.x < objectToFollow.position.x) {
				targetPosition.x = objectToFollow.position.x;
			}
			if (!canMoveRight && player.position.x > objectToFollow.position.x) {
				targetPosition.x = objectToFollow.position.x;
			}
		}
		if (checkVerticalChange) {
			if (!canMoveBottom && player.position.y < objectToFollow.position.y) {
				targetPosition.y = objectToFollow.position.y;
			}
			if (!canMoveTop && player.position.y > objectToFollow.position.y) {
				targetPosition.y = objectToFollow.position.y;
			}
		}
		// Move the camera
		transform.position = Vector3.SmoothDamp (transform.position, targetPosition, ref velocity, smoothing);
	}
} 
