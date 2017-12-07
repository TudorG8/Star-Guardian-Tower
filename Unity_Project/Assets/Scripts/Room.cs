using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {
	public class RoomDirections {
		public static Vector2 left  = Vector2.left ;
		public static Vector2 right = Vector2.right;
		public static Vector2 up    = Vector2.up   ;
		public  static Vector2 down  = Vector2.down ;
	}
	public enum DirectionNames {
		Left, Right, Up, Down
	}
	public DirectionNames entryPoint;
	public DirectionNames exitPoint ;

	public static Vector2 GetDirectionVector(DirectionNames name) {
		if (name == DirectionNames.Left)
			return RoomDirections.left;
		if (name == DirectionNames.Right)
			return RoomDirections.right;
		if (name == DirectionNames.Up)
			return RoomDirections.up;
		if (name == DirectionNames.Down)
			return RoomDirections.down;
		return Vector2.zero;
	}
}
