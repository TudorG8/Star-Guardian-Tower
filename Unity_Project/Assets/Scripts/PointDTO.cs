using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PointDTO {
	public enum Direction {
		None, Top, Bottom, Left, Right
	}
	public Direction main     ;
	public Direction secondary;
	public Vector2   roomIndex;

	public static Direction GetOpposite(Direction direction) {
		if (direction == Direction.None  ) return Direction.None  ;
		if (direction == Direction.Top   ) return Direction.Bottom;
		if (direction == Direction.Bottom) return Direction.Top   ;
		if (direction == Direction.Left  ) return Direction.Right ;
	  /*if  direction == Direction.Right*/ return Direction.Left  ;
	}

	public static Vector2 GetDirectionVector(Direction direction) {
		if (direction == Direction.None  ) return Vector2.zero ;
		if (direction == Direction.Top   ) return Vector2.up   ;
		if (direction == Direction.Bottom) return Vector2.down ;
		if (direction == Direction.Left  ) return Vector2.left ;
	  /*if  direction == Direction.Right*/ return Vector2.right;
	}
}
