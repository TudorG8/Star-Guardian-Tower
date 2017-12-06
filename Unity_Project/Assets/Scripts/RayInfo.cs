using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayInfo {
	public int   rayIndex ;
	public float direction;
	public float rayLength;
	public RaycastHit2D hit;
	public HashSet<Transform> targetsHit;

	public RayInfo(int rayIndex, RaycastHit2D hit, float direction, float rayLength, HashSet<Transform> targetsHit) {
		this.hit = hit;
		this.rayIndex   = rayIndex  ;
		this.direction  = direction ;
		this.rayLength  = rayLength ;
		this.targetsHit = targetsHit;
	}
}