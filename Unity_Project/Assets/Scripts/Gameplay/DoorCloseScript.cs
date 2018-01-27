using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCloseScript : MonoBehaviour {
	public void FullyClose() {
		
		transform.localScale = new Vector2 (4.75f, transform.localScale.y);
	}
}
