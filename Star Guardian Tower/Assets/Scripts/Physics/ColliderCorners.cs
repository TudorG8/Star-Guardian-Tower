using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPropertyDrawers;

/**
 * Quick access information about a collision box.
 */

[System.Serializable]
public class ColliderCorners {
	BoxCollider2D boxCollider; // Collider used to determine from where to shoot the rays

	[SerializeField] float inset = 0.02f; // Inset inside the collider before firing the rays

	[SerializeField][ReadOnly] Vector2 size       ;
	[SerializeField][ReadOnly] Vector2 topLeft    ;
	[SerializeField][ReadOnly] Vector2 topRight   ;
	[SerializeField][ReadOnly] Vector2 bottomLeft ;
	[SerializeField][ReadOnly] Vector2 bottomRight;

	private ColliderCorners() {}

	public void SetBoxCollider(BoxCollider2D boxCollider) {
		this.boxCollider = boxCollider;
		UpdateCorners ();
	}
	public void UpdateCorners () {
		Bounds bounds = boxCollider.bounds;
		bounds.Expand (-inset*2);

		size = bounds.size;
		bottomLeft  = new Vector2 (bounds.min.x, bounds.min.y);
		bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
		topLeft     = new Vector2 (bounds.min.x, bounds.max.y);
		topRight    = new Vector2 (bounds.max.x, bounds.max.y);
	}

	public float Inset { get { return inset; } }
	public Vector2 Size 	   { get { return size       ; } }
	public Vector2 TopLeft     { get { return topLeft    ; } }
	public Vector2 TopRight    { get { return topRight   ; } }
	public Vector2 BottomLeft  { get { return bottomLeft ; } }
	public Vector2 BottomRight { get { return bottomRight; } }
}