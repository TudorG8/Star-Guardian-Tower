using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PointDTO {
	[SerializeField] Direction      main         ;
	[SerializeField] Direction      secondary    ;
	[SerializeField] Vector2        roomIndex    ;
	[SerializeField] TriggerScript  triggerScript;
	[SerializeField] SpriteRenderer sprite       ;
	[SerializeField] Animator       door         ;
	 
	public Direction      Main          { get { return main         ; } set { main      = value; }}
	public Direction      Secondary     { get { return secondary    ; } set { secondary = value; }}
	public Vector2        RoomIndex     { get { return roomIndex    ; } set { roomIndex = value; }}
	public TriggerScript  TriggerScript { get { return triggerScript; } }
	public SpriteRenderer Sprite        { get { return sprite       ; } }
	public Animator       Door          { get { return door         ; } set { door = value; }}

	public override string ToString () {
		return main + " " + secondary + " " + roomIndex;
	}
}
