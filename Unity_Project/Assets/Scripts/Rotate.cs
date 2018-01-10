using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Rotate : MonoBehaviour {
	public enum Direction {
		Clockwise, Anticlockwise
	}
	[SerializeField] Transform obj;

	[SerializeField] float     timeToSpinOnce;
	[SerializeField] Direction direction     ;

	void Update() {
		
	}
}