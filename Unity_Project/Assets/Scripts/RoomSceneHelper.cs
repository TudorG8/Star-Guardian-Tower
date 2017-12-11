using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class RoomSceneHelper : MonoBehaviour {
	[System.Serializable]
	public class PointExtraInfo {
		public Transform  point;
		public Transform  previousPoint;
		public GameObject previousPlatform1;
		public GameObject previousPlatform2;
		public float      previousSize;
	}	
	public GameObject roomPrefab;
	public List<RoomSceneEditor> rooms;

	public PointExtraInfo entryPoint;
	public PointExtraInfo exitPoint ;

	public float pointGap;

	void HandlePlatformSize(string minPointName, PointExtraInfo pointExtraInfo, PointDTO pointToModify, PointHelper pointHelper, RoomSceneEditor.PlatformRefs platformRefs) {
		float newSize;
		GameObject platform1;
		GameObject platform2;
		if (minPointName.Contains ("Top")) {
			pointToModify.main = PointDTO.Direction.Top;
			if(minPointName.Contains("1")) {
				platform1 = platformRefs.top1;
				platform2 = platformRefs.top2;
				pointToModify.secondary = PointDTO.Direction.Left;
			}
			else {
				platform1 = platformRefs.top2;
				platform2 = platformRefs.top1;
				pointToModify.secondary = PointDTO.Direction.Right;
			}
			newSize = 10f;
		}
		else if (minPointName.Contains ("Bottom")) {
			pointToModify.main = PointDTO.Direction.Bottom;
			if(minPointName.Contains("1")) {
				platform1 = platformRefs.bottom1;
				platform2 = platformRefs.bottom2;
				pointToModify.secondary = PointDTO.Direction.Left;
			}
			else {
				platform1 = platformRefs.bottom2;
				platform2 = platformRefs.bottom1;
				pointToModify.secondary = PointDTO.Direction.Right;
			}
			newSize = 10f;
		}
		else if (minPointName.Contains ("Left")) {
			pointToModify.main = PointDTO.Direction.Left;
			if(minPointName.Contains("1")) {
				platform1 = platformRefs.left1;
				platform2 = platformRefs.left2;
				pointToModify.secondary = PointDTO.Direction.Bottom;
			}
			else {
				platform1 = platformRefs.left2;
				platform2 = platformRefs.left1;
				pointToModify.secondary = PointDTO.Direction.Top;
			}
			newSize = 7.5f;
		}
		else { //minPointName.Contains ("Right")
			pointToModify.main = PointDTO.Direction.Right;
			if(minPointName.Contains("1")) {
				platform1 = platformRefs.right1;
				platform2 = platformRefs.right2;
				pointToModify.secondary = PointDTO.Direction.Bottom;
			}
			else {
				platform1 = platformRefs.right2;
				platform2 = platformRefs.right1;
				pointToModify.secondary = PointDTO.Direction.Top;
			}
			newSize = 7.5f;
		}
		if (pointExtraInfo.previousPoint.gameObject.activeSelf) {
			pointExtraInfo.previousPlatform1.transform.localScale = new Vector2 (pointExtraInfo.previousSize, pointExtraInfo.previousPlatform1.transform.localScale.y);
			pointExtraInfo.previousPlatform2.transform.localScale = new Vector2 (pointExtraInfo.previousSize, pointExtraInfo.previousPlatform2.transform.localScale.y);
		}

		platform1.transform.localScale = new Vector2 (0.5f, platform1.transform.localScale.y);
		platform2.transform.localScale = new Vector2 (newSize * 2 - pointGap, platform2.transform.localScale.y);

		pointExtraInfo.previousSize = newSize;
		pointExtraInfo.previousPoint = pointHelper.transform;
		pointExtraInfo.previousPlatform1 = platform1;
		pointExtraInfo.previousPlatform2 = platform2;
	}

	void Update () {
		// Gather all points from all rooms
		List<PointHelper> validPoints = new List<PointHelper>();
		foreach (RoomSceneEditor roomSceneEditor in rooms) {
			validPoints.AddRange (roomSceneEditor.pointRefs.points);
		}

		// Handle exit point
		float minDistance = float.MaxValue;
		PointHelper minPoint = null;
		foreach (PointHelper point in validPoints) {
			if (point.gameObject.activeSelf) {
				float pointDistance = Vector2.Distance (exitPoint.point.transform.position, point.transform.position);
				if (pointDistance < minDistance) {
					minDistance = pointDistance;
					minPoint = point;
				}
			}
		}
		exitPoint.point.transform.position = minPoint.transform.position;
		exitPoint.point.transform.rotation = minPoint.transform.rotation;
		HandlePlatformSize (minPoint.name, exitPoint, minPoint.roomEditor.roomScript.exit, minPoint, minPoint.roomEditor.platformRefs);


		minDistance = float.MaxValue;
		minPoint = null;
		foreach (PointHelper point in validPoints) {
			if (point.gameObject.activeSelf) {
				float pointDistance = Vector2.Distance (entryPoint.point.transform.position, point.transform.position);
				if (pointDistance < minDistance) {
					minDistance = pointDistance;
					minPoint = point;
				}
			}
		}
		entryPoint.point.transform.position = minPoint.transform.position;
		entryPoint.point.transform.rotation = minPoint.transform.rotation;
		HandlePlatformSize (minPoint.name, entryPoint, minPoint.roomEditor.roomScript.entry, minPoint, minPoint.roomEditor.platformRefs);

	}
}
#endif