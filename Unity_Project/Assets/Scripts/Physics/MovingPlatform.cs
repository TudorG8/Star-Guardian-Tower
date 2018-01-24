using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {
	[SerializeField] PlatformController platformController;
	[SerializeField] Transform          waypointParent;

	[SerializeField] bool loop;
	[SerializeField] float speed;

	[SerializeField] AnimationCurve easeAmount;

	List<Transform> waypoints = new List<Transform>();
	int waypointIndex;

	float percentageTraveled;
	public int direction;
	int mod(int k, int n) {
		return ((k %= n) < 0) ? k+n : k;
	}

	public void Reverse() {
		waypointIndex = mod (waypointIndex + direction, waypoints.Count);
		direction = direction * -1;
		percentageTraveled = 1 - percentageTraveled;
	}

	void Start () {
		foreach (Transform waypoint in waypointParent) {
			waypoints.Add (waypoint);
		}
		waypointIndex = 0;
		direction     = 1;
	}
	Vector2 MovePlatform() {
		Vector2 previousWaypoint  = waypoints [waypointIndex].position;
		Vector2 targetWaypoint    = waypoints [mod(waypointIndex + direction, waypoints.Count)].position;

		float distance = Vector2.Distance (previousWaypoint, targetWaypoint); 

		percentageTraveled = Mathf.Clamp01(percentageTraveled + (speed * Time.deltaTime) / distance);
		float easedPercentage = easeAmount.Evaluate (percentageTraveled);

		Vector2 newPosition = Vector2.Lerp (previousWaypoint, targetWaypoint, easedPercentage);

		if (percentageTraveled == 1) {
			percentageTraveled = 0;
			waypointIndex = mod (waypointIndex + direction, waypoints.Count);

			if (!loop && (waypointIndex == waypoints.Count - 1 || waypointIndex == 0)) {
				direction *= -1;
			}
		}

		return newPosition - new Vector2(transform.position.x, transform.position.y); 
	}
	void Update () {
		Vector2 velocity = MovePlatform ();

		platformController.Move (velocity, null);
	}
}
