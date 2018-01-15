using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Rotate : MonoBehaviour {
	public enum Direction {
		Clockwise, Anticlockwise
	}
	[SerializeField] Transform obj;

	[SerializeField] float     timeToSpinOnce;
	[SerializeField] float degrees;
	[SerializeField] Direction direction     ;

	void Update() {
		transform.Rotate (0, 0, degrees * Time.deltaTime);
	}
}