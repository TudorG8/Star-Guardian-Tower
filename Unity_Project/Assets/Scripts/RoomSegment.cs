using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPropertyDrawers;

public class RoomSegment : MonoBehaviour, IndexableArrayPiece<RoomSegment> {
	// Imports
	[SerializeField] RoomSegmentHelper helperScript;
	[SerializeField] Transform         middle      ;

	// Readonly
	[SerializeField]           Neighbours neighbours;
	[SerializeField][ReadOnly] Vector2    index     ;

	// Properties
	public RoomSegmentHelper HelperScript      { get { return helperScript; } }
	public Transform         Middle            { get { return middle      ; } }
	public Neighbours        SegmentNeighbours { get { return neighbours  ; } }
	public Vector2           Index             { get { return index       ; } set { index = value; }}

	public void DeactivateHelperScript() {
		
	}

	public void IncreaseIndex    (Vector2 increase) {
		index += increase;
	}

	public void IncreasePosition (Vector2 increase) {
		Vector2 position = transform.position;
		position += UsefulMethods.vectorProduct(increase, new Vector2 (26.7f, 15f));
		transform.position = position;
	}

	public GameObject GetGameObject () {
		return gameObject;
	}

	public void SetName() {
		name = "Room " + index.y + " " + index.x;
	}
}