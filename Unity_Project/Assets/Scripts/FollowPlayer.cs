using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {
	public Transform objToFollow;
	public Vector3 offset;
	public float smoothing = 0.2f;

	Vector3 velocity;

	void LateUpdate () {
		Vector3 targetPosition = offset + objToFollow.position;
		transform.position = Vector3.SmoothDamp (transform.position, targetPosition, ref velocity, smoothing);
	}
} 
