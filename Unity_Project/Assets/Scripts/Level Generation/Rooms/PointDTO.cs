using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PointDTO {
	[SerializeField] Direction     main         ;
	[SerializeField] Direction     secondary    ;
	[SerializeField] Vector2       roomIndex    ;
	[SerializeField] TriggerScript triggerScript;

	public Direction     Main          { get { return main         ; } set { main      = value; }}
	public Direction     Secondary     { get { return secondary    ; } set { secondary = value; }}
	public Vector2       RoomIndex     { get { return roomIndex    ; } set { roomIndex = value; }}
	public TriggerScript TriggerScript { get { return triggerScript; } }

	public override string ToString () {
		return main + " " + secondary + " " + roomIndex;
	}
}
