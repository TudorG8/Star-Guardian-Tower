using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsefulMethods : MonoBehaviour {
	public static Vector2 vectorProduct(Vector2 a, Vector2 b) {
		return new Vector2 (a.x * b.x, a.y * b.y);
	}
	public static Vector2 ReverseCoordinates(Vector2 input) {
		return new Vector2 (input.y, input.x);
	}
}
