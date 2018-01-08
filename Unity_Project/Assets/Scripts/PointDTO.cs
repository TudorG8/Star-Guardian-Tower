using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PointDTO {
	public Direction main     ;
	public Direction secondary;
	public Vector2   roomIndex;
	public TriggerScript triggerScript;

	public PointDTO GetCopy() {
		PointDTO newPoint = new PointDTO ();

		newPoint.main      = this.main;
		newPoint.secondary = this.secondary;
		newPoint.roomIndex = this.roomIndex;

		return newPoint;
	}

	public override string ToString () {
		return main + " " + secondary + " " + roomIndex;
	}
}
