using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PointDTO {
	public enum Direction {
		None, Left, Right, Top, Bottom
	}
	public Direction main     ;
	public Direction secondary;
	public Vector2   roomIndex;
}
