using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : RoomObject {
	public void OnTouch() {
		PlayerController.Instance.UpdateSavePoint (transform);
	}
}