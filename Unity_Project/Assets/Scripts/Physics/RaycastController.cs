using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPropertyDrawers;

public class RaycastController : MonoBehaviour {
	public GameObject player;
	public LayerMask collisionMask;
	public GameObject phantom;
	public GameObject phantomObj;
	[SerializeField] BoxCollider2D boxCollider;

	[SerializeField][Range(2, 10)] int horizontalRayCount = 2; // Amount of horizontal rays to fire
	[SerializeField][Range(2, 10)] int verticalRayCount   = 2; // Amount of vertical   rays to fire
	[SerializeField][ReadOnly] float horizontalSpacing;
	[SerializeField][ReadOnly] float verticalSpacing  ;

	public int maxSlopeAngle = 60;
	public int maxSlopeDescentAngle = 60;

	[SerializeField] ColliderCorners boxCorners;
	public CollisionInfo collisionInfo;

	Vector2 velocity;
	Vector2 previousVelocity;

	void CalculateSpacing () {
		horizontalSpacing = boxCorners.Size.x / (horizontalRayCount - 1);
		verticalSpacing   = boxCorners.Size.y / (verticalRayCount   - 1);
	}

	void DrawAllRays (float distance) {
		for (int i = 0; i < horizontalRayCount; i++) {
			Debug.DrawRay (boxCorners.TopLeft    + Vector2.right * horizontalSpacing * i, Vector2.up   * distance, Color.red);
			Debug.DrawRay (boxCorners.BottomLeft + Vector2.right * horizontalSpacing * i, Vector2.down * distance, Color.red);
		}

		for (int i = 0; i < verticalRayCount; i++) {
			Debug.DrawRay (boxCorners.TopLeft  + Vector2.down * verticalSpacing * i, Vector2.left  * distance, Color.red);
			Debug.DrawRay (boxCorners.TopRight + Vector2.down * verticalSpacing * i, Vector2.right * distance, Color.red);
		}
	}

	void Awake () {
		boxCorners.setBoxCollider (boxCollider);
		CalculateSpacing ();
	}
	void SpawnPhantom() {
		Vector2 phantomPos = transform.position;
		if (phantomObj == null) {
			phantomObj = Instantiate (phantom, phantomPos, Quaternion.identity) as GameObject;
		} 
		else {
			GameObject.Destroy (phantomObj);
			phantomObj = Instantiate (phantom, phantomPos, Quaternion.identity) as GameObject;
		}
	}
	void HorizontalCollisions(ref Vector2 velocity) {
		float direction = Mathf.Sign(velocity.x);
		float rayLength = Mathf.Abs (velocity.x) + boxCorners.Inset;

		for (int i = 0; i < horizontalRayCount; i++) {
			Vector2 raycastOrigin = direction == 1 ? boxCorners.BottomRight : boxCorners.BottomLeft;
			raycastOrigin += Vector2.up * horizontalSpacing * i;

			RaycastHit2D hit = Physics2D.Raycast (raycastOrigin, Vector2.right * direction, rayLength, collisionMask);

			if (hit) {
				float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);

				// We check the bottommost ray to see if we are climbing a slope
				if (i == 0 && slopeAngle < maxSlopeAngle) {
					float distanceToSlopeStart = 0;
					if (slopeAngle != collisionInfo.slopeAngleOld) {
						distanceToSlopeStart = hit.distance-boxCorners.Inset;
						velocity.x -= distanceToSlopeStart * direction;
					}

					collisionInfo.slopeAngle = slopeAngle;
					// Going from an descending slope to an ascending slope
					if (collisionInfo.descendingSlope) {
						collisionInfo.descendingSlope = false;
						velocity = previousVelocity;
					}

					ClimbSlope (ref velocity, slopeAngle);
					velocity.x += distanceToSlopeStart * direction;
					Debug.Log ("no");
					continue;
				} 
				if (!collisionInfo.ascendingSlope || slopeAngle > maxSlopeAngle) {
					Debug.Log ("yes");
					velocity.x = direction * (hit.distance - boxCorners.Inset);
					rayLength  = hit.distance;

					if (collisionInfo.ascendingSlope) {
						float targetYVelocity = Mathf.Tan (collisionInfo.slopeAngle  * Mathf.Deg2Rad) * Mathf.Abs (velocity.x);
						velocity.y = targetYVelocity;
					}

					collisionInfo.left  = direction == -1;
					collisionInfo.right = direction ==  1;
				}
			}

			Debug.DrawRay (raycastOrigin, Vector2.right * direction * (rayLength), Color.red);
		}
	}

	void VerticalCollisions(ref Vector2 velocity) {
		float direction = Mathf.Sign(velocity.y);
		float rayLength = Mathf.Abs (velocity.y) + boxCorners.Inset;

		for (int i = 0; i < verticalRayCount; i++) {
			Vector2 raycastOrigin = direction == 1 ? boxCorners.TopLeft : boxCorners.BottomLeft;
			raycastOrigin += Vector2.right * (verticalSpacing * i + velocity.x);

			RaycastHit2D hit = Physics2D.Raycast (raycastOrigin, Vector2.up * direction, rayLength, collisionMask);

			if (hit) {
				velocity.y = direction * (hit.distance - boxCorners.Inset);
				rayLength  = hit.distance;

				if (collisionInfo.ascendingSlope) {
					velocity.x = velocity.y / Mathf.Tan (collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign (velocity.x);
				}

				collisionInfo.below = direction == -1;
				collisionInfo.above = direction ==  1;
			}

			Debug.DrawRay (raycastOrigin, Vector2.up * direction * (rayLength), Color.red);
		}

		if (collisionInfo.ascendingSlope) {
			direction = Mathf.Sign(velocity.x);
			rayLength = Mathf.Abs (velocity.x) + boxCorners.Inset;
			Vector2 rayOrigin = direction == -1 ? boxCorners.BottomLeft : boxCorners.BottomRight;
			rayOrigin += Vector2.up * velocity.y;

			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right * direction, rayLength, collisionMask);
			if (hit) {
				float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);
				if (slopeAngle != collisionInfo.slopeAngle) {
					velocity.x = (hit.distance - boxCorners.Inset) * direction;
					collisionInfo.slopeAngle = slopeAngle;
				}
			}
		}
	}

	void ClimbSlope(ref Vector2 velocity, float slopeAngle) {
		float moveDistance = Mathf.Abs (velocity.x);
		float targetYVelocity = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
		// Allow jumping on a slope
		if(velocity.y > targetYVelocity) {
			//Debug.Log ("jumping");
		}
		if (velocity.y <= targetYVelocity) {
			velocity.y = targetYVelocity;
			velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);
			collisionInfo.below = true;
			collisionInfo.ascendingSlope = true;
		}
	}

	void DescendSlope(ref Vector2 velocity) {
		float direction = Mathf.Sign (velocity.x);
		Vector2 rayOrigin = direction == -1 ? boxCorners.BottomRight : boxCorners.BottomLeft;
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

		if (hit) {
			float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);

			if (slopeAngle != 0 && 
				slopeAngle <= maxSlopeDescentAngle && 
				Mathf.Sign (hit.normal.x) == direction && 
				(hit.distance - boxCorners.Inset) <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x) 
			){
				float moveDistance = Mathf.Abs (velocity.x);
				float targetYVelocity = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
				velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);
				velocity.y -= targetYVelocity;
				collisionInfo.descendingSlope = true;
			}
		}
	}

	public void Move(Vector2 velocity) {
		boxCorners.UpdateCorners ();
		CalculateSpacing         ();
		collisionInfo.Reset      ();

		previousVelocity = velocity;
		SpawnPhantom ();
		if (velocity.y <  0) {
			DescendSlope (ref velocity);
		}

		if (velocity.x != 0) {
			HorizontalCollisions (ref velocity);
		}

		if (velocity.y != 0) {
			VerticalCollisions   (ref velocity);
		}

		player.transform.Translate (velocity);
	}
}

