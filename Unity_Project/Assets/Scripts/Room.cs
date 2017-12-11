using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {
	public PointDTO entry;
	public PointDTO exit ;

	public Vector2 size;

	Vector2 vectorProduct(Vector2 a, Vector2 b) {
		return new Vector2 (a.x * b.x, a.y * b.y);
	}
}
