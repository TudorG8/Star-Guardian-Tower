using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Information about a ray hit.
 */

public class RayInfo {
	[SerializeField] int   rayIndex ;
	[SerializeField] float direction;
	[SerializeField] float rayLength;
	[SerializeField] int   rayHits  ;
	[SerializeField] RaycastHit2D hit;
	[SerializeField] HashSet<Transform> targetsHit;

	public RayInfo(int rayIndex, RaycastHit2D hit, float direction, float rayLength, HashSet<Transform> targetsHit) {
		this.hit = hit;
		this.rayIndex   = rayIndex  ;
		this.direction  = direction ;
		this.rayLength  = rayLength ;
		this.targetsHit = targetsHit;
		this.rayHits = 0;
	}

	public int                RayIndex   { get { return rayIndex  ; } }
	public float              Direction  { get { return direction ; } }
	public float              RayLength  { get { return rayLength ; } set { rayLength = value; }}
	public int                RayHits    { get { return rayHits   ; } set { rayHits   = value; }}
	public RaycastHit2D       Hit        { get { return hit       ; } set { hit       = value; }}
	public HashSet<Transform> TargetsHit { get { return targetsHit; } }
}