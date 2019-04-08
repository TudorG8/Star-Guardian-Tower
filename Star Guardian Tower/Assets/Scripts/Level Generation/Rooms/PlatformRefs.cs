using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Holds references to the outer platforms of a room segment
 * Allows settings their size or getting information about them.
 */

[System.Serializable]
public class PlatformRefs {
	[SerializeField] List<GameObject> platforms;

	// Set the x scale of both platforms of a whole side
	public void SetXScale(Direction side, float xScale) {
		foreach (GameObject platform in platforms) {
			if(platform.name.Contains(side.ToString())) {
				platform.transform.localScale = new Vector2 (xScale, platform.transform.localScale.y);
			}
		}
	}

	// Set the x scale of a single platform
	public void SetXScale(Direction side, int index, float xScale) {
		foreach (GameObject platform in platforms) {
			if(platform.name.Contains(side.ToString()) && platform.name.Contains(index.ToString())) {
				platform.transform.localScale = new Vector2 (xScale, platform.transform.localScale.y);
				break;
			}
		}
	}

	// Get a single platform
	public GameObject Get(Direction side, int index) {
		for (int i = 0; i < platforms.Count; i++) {
			if (platforms [i].name.Contains (side.ToString ()) && platforms [i].name.Contains (index.ToString ()))
				return platforms [i];
		}

		Debug.LogError ("There was no platform of type " + side.ToString () + " " + index);
		return null;
	}

	// Gets the secondary direction based on the index given
	public Direction GetFromIndex(Direction main, int index) {
		if (main == Direction.Top  || main == Direction.Bottom) {
			if (index == 1) return Direction.Left ;
			if (index == 2) return Direction.Right;
		}
		if (main == Direction.Left || main == Direction.Right ) {
			if (index == 1) return Direction.Bottom;
			if (index == 2) return Direction.Top   ;
		}
		Debug.LogError ("Bad Input");
		return Direction.None;
	}
}