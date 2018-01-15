using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPropertyDrawers;

public class RaycastShooter : MonoBehaviour {
	[SerializeField] public LayerMask     collisionMask;
	[SerializeField] BoxCollider2D boxCollider  ;

	[SerializeField] bool drawRaysAfter = true;
	[SerializeField][Range(2, 10)] int horizontalRayCount = 2; // Amount of horizontal rays to fire
	[SerializeField][Range(2, 10)] int verticalRayCount   = 2; // Amount of vertical   rays to fire

	[SerializeField][ReadOnly] float horizontalSpacing;
	[SerializeField][ReadOnly] float verticalSpacing  ;

	[SerializeField] public ColliderCorners boxCorners   ;
	[SerializeField] public CollisionInfo   collisionInfo;

	HashSet<Transform> targetsHit = new HashSet<Transform> ();

	public delegate StatementInfo RayFunction(RayInfo rayInfo);

	public int VerticalRayCount { get { return verticalRayCount; } }

	void CalculateSpacing () {
		horizontalSpacing = boxCorners.Size.y / (horizontalRayCount - 1);
		verticalSpacing   = boxCorners.Size.x / (verticalRayCount   - 1);
	}
	void Start() {
		boxCorners.setBoxCollider (boxCollider);
	}
	public void Reset () {
		boxCorners.UpdateCorners ();
		CalculateSpacing         ();
		collisionInfo.Reset      ();
	}

	public void ShootHorrizontalRays(ref Vector2 velocity, Color rayColor, float givenDirection, bool newTargetSet, RayFunction rayFunction) {
		int direction = (int)Mathf.Sign(givenDirection);
		float rayLength = Mathf.Abs (velocity.x) + boxCorners.Inset;
		Vector2 raycastOrigin = direction == 1 ? boxCorners.BottomRight : boxCorners.BottomLeft;
		raycastOrigin -= Vector2.up * horizontalSpacing;

		if (rayLength <= boxCorners.Inset) {
			rayLength = boxCorners.Inset * 2;
		}
			
		if(newTargetSet)
			targetsHit = new HashSet<Transform> ();

		for (int rayIndex = 0; rayIndex < horizontalRayCount; rayIndex++) {
			raycastOrigin += Vector2.up * horizontalSpacing;
			RaycastHit2D hit = Physics2D.Raycast (raycastOrigin, Vector2.right * direction, rayLength, collisionMask);

			StatementInfo statementInfo = rayFunction (new RayInfo(rayIndex, hit, direction, rayLength, targetsHit));

			Debug.DrawRay (raycastOrigin, Vector2.right * direction * (rayLength), rayColor);

			if (statementInfo == StatementInfo.Break   ) { break   ;}
			if (statementInfo == StatementInfo.Continue) { continue;}
		}
	}

	public void ShootVerticalRays(ref Vector2 velocity, Color rayColor, float givenDirection, bool applyXVelocity, bool newTargetSet, RayFunction rayFunction) {
		int direction = (int)Mathf.Sign(givenDirection);
		float rayLength = Mathf.Abs (velocity.y) + boxCorners.Inset;
		Vector2 raycastOrigin = direction == 1 ? boxCorners.TopLeft : boxCorners.BottomLeft;
		raycastOrigin -= Vector2.right * verticalSpacing;

		if(applyXVelocity)
			raycastOrigin += Vector2.right * velocity.x;

		if(newTargetSet)
			targetsHit = new HashSet<Transform> ();

		for (int rayIndex = 0; rayIndex < verticalRayCount; rayIndex++) {
			raycastOrigin += Vector2.right * verticalSpacing;
			RaycastHit2D hit = Physics2D.Raycast (raycastOrigin, Vector2.up * direction, rayLength, collisionMask);

			StatementInfo statementInfo = rayFunction (new RayInfo(rayIndex, hit, direction, rayLength, targetsHit));

			Debug.DrawRay (raycastOrigin, Vector2.up * direction * (rayLength), rayColor);

			if (statementInfo == StatementInfo.Break   ) { break   ;}
			if (statementInfo == StatementInfo.Continue) { continue;}
		}
	}
}
