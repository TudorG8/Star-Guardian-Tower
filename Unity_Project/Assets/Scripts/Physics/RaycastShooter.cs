using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPropertyDrawers;

public class RaycastShooter : MonoBehaviour {
	// Imports
	[SerializeField] BoxCollider2D boxCollider  ;

	// If this is turned on, the spacing will be calculated based on the auto spacing fields below
	[SerializeField] bool  autoDetermineSpacing ;
	[SerializeField] float autoHorizontalSpacing;
	[SerializeField] float autoVerticalSpacing  ; 

	// Otherwise, it will be calculated based on the ray count
	[SerializeField][Range(2, 50)] int horizontalRayCount = 2; // Amount of horizontal rays to fire
	[SerializeField][Range(2, 50)] int verticalRayCount   = 2; // Amount of vertical   rays to fire

	// Read Only
	[SerializeField][ReadOnly] float horizontalSpacing;
	[SerializeField][ReadOnly] float verticalSpacing  ;
	[SerializeField] ColliderCorners    colliderCorners;
	[SerializeField] CollisionInfo      collisionInfo  ;
	[SerializeField] HashSet<Transform> targetsHit     ;

	public CollisionInfo   GetCollisionInfo   { get { return collisionInfo  ; } }
	public ColliderCorners GetColliderCorners { get { return colliderCorners; } }
	public int HorizontalRayCount { get { return horizontalRayCount; } }
	public int VerticalRayCount   { get { return verticalRayCount  ; } }

	// Delegates
	public delegate StatementInfo RayFunction(RayInfo rayInfo);

	void Start() {
		colliderCorners.SetBoxCollider (boxCollider);
		targetsHit = new HashSet<Transform> ();
		Reset ();
	}

	// Resets all information ready for the next cycle of ray shooting
	public void Reset () {
		colliderCorners.UpdateCorners ();
		collisionInfo  .Reset ();
		CalculateSpacing ();
	}

	/**
	 * Calculate the space between each fired ray.
	 * If autoDetermineSpacing is turned on, it will actually calculate the number of rays
	 * Otherwise, the spacing is calculated from the number of rays
	 */
	void CalculateSpacing () {
		if (autoDetermineSpacing) {
			horizontalSpacing  = autoHorizontalSpacing;
			verticalSpacing    = autoVerticalSpacing  ;
			horizontalRayCount = (int) (colliderCorners.Size.x / horizontalSpacing);
			verticalRayCount   = (int) (colliderCorners.Size.y / verticalSpacing  );

			if (verticalRayCount < 2) {
				verticalRayCount = 2;
				verticalSpacing  = colliderCorners.Size.x / 2;
			}
			if (horizontalRayCount < 2) {
				horizontalRayCount = 2;
				horizontalSpacing  = colliderCorners.Size.y / 2;
			}
		} 
		else {
			horizontalSpacing = colliderCorners.Size.y / (horizontalRayCount - 1);
			verticalSpacing   = colliderCorners.Size.x / (verticalRayCount   - 1);
		}
	}

	/**
	 * Shoots rays either to the left or right.
	 * Each hit will trigger an event to be called, defined in rayFunction (you can specifiy what happens whenever a ray hits).
	 * Targets hit are held in a hashset (targetsHit) which should be populated by you (the programmer) in the custom rayFunction you make.
	 * @param velocity       : used to determine the length of the rays
	 * @param rayColor       : color of the ray
	 * @param givenDirection : sometimes you want the direction to be different than the velocity
	 * @param applyYVelocity : whether to apply the y velocity before shooting the rays
	 * @param newTargetSet   : should the targetsHit be reset before shooting the rays?
	 */
	public void ShootHorrizontalRays(ref Vector2 velocity, Color rayColor, float givenDirection, bool applyYVelocity, bool newTargetSet, LayerMask collisionMask, RayFunction rayFunction) {
		int direction = (int)Mathf.Sign(givenDirection);
		Vector2 raycastOrigin = direction == 1 ? colliderCorners.BottomRight : colliderCorners.BottomLeft;

		// If the ray length is smaller than the inset, we force it to be the Inset
		float rayLength = Mathf.Abs (velocity.x) + colliderCorners.Inset;
		if (rayLength <= colliderCorners.Inset) {
			rayLength = colliderCorners.Inset * 2;
		}

		if(applyYVelocity)
			raycastOrigin += Vector2.up * velocity.y;

		if(newTargetSet)
			targetsHit = new HashSet<Transform> ();

		for (int rayIndex = 0; rayIndex < horizontalRayCount; rayIndex++) {
			RaycastHit2D hit = Physics2D.Raycast (raycastOrigin, Vector2.right * direction, rayLength, collisionMask);

			StatementInfo statementInfo = rayFunction (new RayInfo(rayIndex, hit, direction, rayLength, targetsHit));

			Debug.DrawRay (raycastOrigin, Vector2.right * direction * (rayLength), rayColor);

			raycastOrigin += Vector2.up * horizontalSpacing;

			if (statementInfo == StatementInfo.Break   ) { break   ;}
			if (statementInfo == StatementInfo.Continue) { continue;}
		}
	}

	/**
	 * Shoots rays either to the top or bottom.
	 * Each hit will trigger an event to be called, defined in rayFunction (you can specifiy what happens whenever a ray hits).
	 * Targets hit are held in a hashset (targetsHit) which should be populated by you (the programmer) in the custom rayFunction you make.
	 * @param velocity       : used to determine the length of the rays
	 * @param rayColor       : color of the ray
	 * @param givenDirection : sometimes you want the direction to be different than the velocity
	 * @param applyXVelocity : whether to apply the x velocity before shooting the rays
	 * @param newTargetSet   : should the targetsHit be reset before shooting the rays?
	 */
	public void ShootVerticalRays(ref Vector2 velocity, Color rayColor, float givenDirection, bool applyXVelocity, bool newTargetSet, LayerMask collisionMask, RayFunction rayFunction) {
		int direction = (int)Mathf.Sign(givenDirection);
		Vector2 raycastOrigin = direction == 1 ? colliderCorners.TopLeft : colliderCorners.BottomLeft;

		float rayLength = Mathf.Abs (velocity.y) + colliderCorners.Inset;
		if (rayLength <= colliderCorners.Inset) {
			rayLength = colliderCorners.Inset * 2;
		}

		if(applyXVelocity)
			raycastOrigin += Vector2.right * velocity.x;

		if(newTargetSet)
			targetsHit = new HashSet<Transform> ();

		for (int rayIndex = 0; rayIndex < verticalRayCount; rayIndex++) {
			RaycastHit2D hit = Physics2D.Raycast (raycastOrigin, Vector2.up * direction, rayLength, collisionMask);

			StatementInfo statementInfo = rayFunction (new RayInfo(rayIndex, hit, direction, rayLength, targetsHit));

			Debug.DrawRay (raycastOrigin, Vector2.up * direction * (rayLength), rayColor);

			raycastOrigin += Vector2.right * verticalSpacing;

			if (statementInfo == StatementInfo.Break   ) { break   ;}
			if (statementInfo == StatementInfo.Continue) { continue;}
		}
	}
}