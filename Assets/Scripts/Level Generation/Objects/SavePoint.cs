using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Whenever the player triggers a collision, this will save its position for when they die.
 */

public class SavePoint : RoomObject {
	public void OnTouch() {
		PlayerController.Instance.UpdateSavePoint (transform);
	}
}