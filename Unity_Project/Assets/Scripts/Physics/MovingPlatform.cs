using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPropertyDrawers;

public class MovingPlatform : MonoBehaviour {
	[SerializeField] ControllerBase controller    ;
	[SerializeField] Transform      waypointParent;

	[SerializeField] bool  loop ;
	[SerializeField] float speed;

	[SerializeField] AnimationCurve easeAmount;

	[SerializeField] List<Transform> waypoints = new List<Transform>();
	[SerializeField][ReadOnly] int   waypointIndex     ;
	[SerializeField][ReadOnly] float percentageTraveled;
	[SerializeField][ReadOnly] int   direction         ;

	void Start () {
		foreach (Transform waypoint in waypointParent) {
			waypoints.Add (waypoint);
		}
		waypointIndex = 0;
		direction     = 1;
	}

	Vector2 GetVelocity() {
		Vector2 previousWaypoint  = waypoints [waypointIndex].position;
		Vector2 targetWaypoint    = waypoints [UsefulMethods.mod(waypointIndex + direction, waypoints.Count)].position;

		float distance = Vector2.Distance (previousWaypoint, targetWaypoint); 

		percentageTraveled = Mathf.Clamp01(percentageTraveled + (speed * Time.deltaTime) / distance);
		float easedPercentage = easeAmount.Evaluate (percentageTraveled);

		Vector2 newPosition = Vector2.Lerp (previousWaypoint, targetWaypoint, easedPercentage);

		if (percentageTraveled == 1) {
			percentageTraveled = 0;
			waypointIndex = UsefulMethods.mod (waypointIndex + direction, waypoints.Count);

			if (!loop && (waypointIndex == waypoints.Count - 1 || waypointIndex == 0)) {
				direction *= -1;
			}
		}

		return (newPosition - new Vector2(transform.position.x, transform.position.y)) ; 
	}

	void Update () {
		Vector2 velocity = GetVelocity ();
		if (controller != null) {
			controller.AddVelocity ("Platform", velocity);
		}
		else
			transform.Translate (velocity);
	}

	public void Reverse() {
		waypointIndex = UsefulMethods.mod (waypointIndex + direction, waypoints.Count);
		direction = direction * -1;
		percentageTraveled = 1 - percentageTraveled;
	}
}
