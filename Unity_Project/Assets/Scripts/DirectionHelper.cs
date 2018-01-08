using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionHelper {
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

	public static Direction GetByName(string name) {
		if (name == "None"  ) return Direction.None  ;
		if (name == "Top"   ) return Direction.Bottom;
		if (name == "Bottom") return Direction.Top   ;
		if (name == "Left"  ) return Direction.Right ;
		if (name == "Right" ) return Direction.Left  ;

		Debug.LogError ("Wrong Input: " + name);
		return Direction.None;
	}

}