using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour {
	public void OnTouch() {
		PlayerController.Instance.UpdateSavePoint (transform);
	}
}