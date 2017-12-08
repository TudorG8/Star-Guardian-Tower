using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {
	[System.Serializable]
	public class Point {
		public Direction main     ;
		public Direction secondary;
		public Vector2   roomIndex;
	}

	public enum Direction {
		None, Left, Right, Top, Bottom
	}
	public class RoomDirections {
		public static Vector2 left  = Vector2.left ;
		public static Vector2 right = Vector2.right;
		public static Vector2 up    = Vector2.up   ;
		public  static Vector2 down = Vector2.down ;
	}
	public enum DirectionNames {
		None,
		TopLeft, TopRight,
		BottomLeft, BottomRight,
		LeftBottom, LeftTop,
		RightBottom, RightTop
	}
	public Point entry;
	public Point exit ;
	public DirectionNames entryPoint;
	public DirectionNames exitPoint ;
	public Vector2 size;

	Vector2 vectorProduct(Vector2 a, Vector2 b) {
		return new Vector2 (a.x * b.x, a.y * b.y);
	}

	/**
	 * Returns the opposite of a direction.
	 * It will change the first half of the direction.
	 * 	Example: TopLeft -> BottomLeft
	 */
	public static DirectionNames GetOpposite(DirectionNames name) {
		if (name == DirectionNames.None)
			return DirectionNames.None;
		
		string directionName = name.ToString ();
		string newName = "";
		Debug.Log (directionName);
		if (directionName.StartsWith ("Top"))
			newName = "Bottom" + directionName.Substring (3);
		if (directionName.StartsWith ("Bottom"))
			newName = "Top" + directionName.Substring (6);
		if (directionName.StartsWith ("Left"))
			newName = "Right" + directionName.Substring (4);
		if (directionName.StartsWith ("Right"))
			newName = "Left" + directionName.Substring (5);
		Debug.Log (newName);
		DirectionNames newDirectionName = (DirectionNames)System.Enum.Parse (typeof(DirectionNames), newName);

		return newDirectionName;
	}

	public static Vector2 GetDirectionVector(DirectionNames name) {
		string directionName = name.ToString ();
		if (directionName.StartsWith("Left"))
			return RoomDirections.left;
		if (directionName.StartsWith("Right"))
			return RoomDirections.right;
		if (directionName.StartsWith("Top"))
			return RoomDirections.up;
		if (directionName.StartsWith("Bottom"))
			return RoomDirections.down;
		return Vector2.zero;
	}
}
