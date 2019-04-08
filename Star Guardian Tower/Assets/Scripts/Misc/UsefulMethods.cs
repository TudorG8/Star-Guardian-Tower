using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Provides useful methods that are not provided by Unity.
 */
public class UsefulMethods : MonoBehaviour {
	public static Vector2 vectorProduct(Vector2 a, Vector2 b) {
		return new Vector2 (a.x * b.x, a.y * b.y);
	}

	public static Vector2 ReverseCoordinates(Vector2 input) {
		return new Vector2 (input.y, input.x);
	}

	public static bool IsRightLayer(LayerMask collisionMask, int layer) {
		return collisionMask == (collisionMask | (1 << layer));
	}

	public static int mod(int k, int n) {
		return ((k %= n) < 0) ? k+n : k;
	}
}
