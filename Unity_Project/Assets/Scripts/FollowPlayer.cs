using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {
	public RoomGenerator currentRoom;
	public Transform objToFollow;
	public Transform player;
	public Vector3 offset;
	public float smoothing = 0.2f;

	public Vector2 currentIndex;

	public bool canMoveLeft, canMoveRight, canMoveTop, canMoveBottom;

	public Vector3 velocity;

	void Start() {
		
	}

	void LateUpdate () {
		float distanceToCurrentPoint = Vector3.Distance (player.position, objToFollow.position);
		RoomGeneratorRoomHelper currentRoomSegment = currentRoom.rooms.RoomAt (currentIndex);
		//Debug.Log (currentRoomSegment);
		Transform newObjectToFollow = objToFollow;
		canMoveTop = canMoveBottom = canMoveLeft = canMoveRight = false;
		if (currentRoomSegment.neighbours.top != null) {
			canMoveTop = true;
			float distanceToNeighbour = Vector3.Distance (player.position, currentRoomSegment.neighbours.top.middle.position);
			if (distanceToNeighbour < distanceToCurrentPoint) {
				currentIndex = currentRoomSegment.neighbours.top.index;
				newObjectToFollow = currentRoomSegment.neighbours.top.middle;
			}
		} 
		if (currentRoomSegment.neighbours.bottom != null) {
			canMoveBottom = true;
			float distanceToNeighbour = Vector2.Distance (player.position, currentRoomSegment.neighbours.bottom.middle.position);
			if (distanceToNeighbour < distanceToCurrentPoint) {
				currentIndex = currentRoomSegment.neighbours.bottom.index;
				newObjectToFollow = currentRoomSegment.neighbours.bottom.middle;
			}
		}
		if (currentRoomSegment.neighbours.left != null) {
			canMoveLeft = true;
			float distanceToNeighbour = Vector2.Distance (player.position, currentRoomSegment.neighbours.left.middle.position);
			if (distanceToNeighbour < distanceToCurrentPoint) {
				currentIndex = currentRoomSegment.neighbours.left.index;
				newObjectToFollow = currentRoomSegment.neighbours.left.middle;
			}
		}
		if (currentRoomSegment.neighbours.right != null) {
			canMoveRight = true;
			float distanceToNeighbour = Vector2.Distance (player.position, currentRoomSegment.neighbours.right.middle.position);
			if (distanceToNeighbour < distanceToCurrentPoint) {
				currentIndex = currentRoomSegment.neighbours.right.index;
				newObjectToFollow = currentRoomSegment.neighbours.right.middle;
			}
		}

		Vector3 targetPosition = objToFollow.position + offset;
		targetPosition.x = player.position.x;
		targetPosition.y = player.position.y;
		if (player.position.x < objToFollow.position.x) {
			if (!canMoveLeft) {
				targetPosition.x = objToFollow.position.x;
			}
		}
		if (player.position.x > objToFollow.position.x) {
			if (!canMoveRight) {
				targetPosition.x = objToFollow.position.x;
			}
		}
		if (player.position.y < objToFollow.position.y) {
			if (!canMoveBottom) {
				targetPosition.y = objToFollow.position.y;
			}
		}
		if (player.position.y > objToFollow.position.y) {
			if (!canMoveTop) {
				targetPosition.y = objToFollow.position.y;
			}
		}
			
		objToFollow = newObjectToFollow;

		transform.position = Vector3.SmoothDamp (transform.position, targetPosition, ref velocity, smoothing);
	}
} 
