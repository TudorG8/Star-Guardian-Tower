using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
[ExecuteInEditMode]
public class RoomSceneEditor : MonoBehaviour {
	public enum Direction {
		None, Left, Right, Top, Bottom
	}

	[System.Serializable]
	public class Point {
		public Direction main     ;
		public Direction secondary;
		public Vector2   roomIndex;
	}

	[System.Serializable]
	public class PlatformRefs {
		public GameObject top1;
		public GameObject top2;

		public GameObject bottom1;
		public GameObject bottom2;

		public GameObject left1;
		public GameObject left2;

		public GameObject right1;
		public GameObject right2;
	}

	[System.Serializable]
	public class PointRefs {
		public GameObject top1;
		public GameObject top2;

		public GameObject bottom1;
		public GameObject bottom2;

		public GameObject left1;
		public GameObject left2;

		public GameObject right1;
		public GameObject right2;

		public List<GameObject> points;
	}
	[System.Serializable]
	public class PointExtraInfo {
		public Transform  point;
		public GameObject previousPlatform1;
		public GameObject previousPlatform2;
		public float previousSize;
	}
	public Room roomScript;

	public PlatformRefs platformRefs;
	public PointRefs    pointRefs   ;

	public PointExtraInfo  entryPoint;
	public PointExtraInfo  exitPoint ;

	public GameObject room    ;
	public GameObject platform;



	public float pointGap;

	public Point desiredExitPoint;

	public void AddRoom() {
		
	}

	void HandlePlatformSize(string minPointName, PointExtraInfo pointExtraInfo, Room.Point pointToModify) {
		float newSize;
		GameObject platform1;
		GameObject platform2;
		if (minPointName.Contains ("Top")) {
			pointToModify.main = Room.Direction.Top;
			if(minPointName.Contains("1")) {
				platform1 = platformRefs.top1;
				platform2 = platformRefs.top2;
				pointToModify.secondary = Room.Direction.Left;
			}
			else {
				platform1 = platformRefs.top2;
				platform2 = platformRefs.top1;
				pointToModify.secondary = Room.Direction.Right;
			}
			newSize = 10f;
		}
		else if (minPointName.Contains ("Bottom")) {
			pointToModify.main = Room.Direction.Bottom;
			if(minPointName.Contains("1")) {
				platform1 = platformRefs.bottom1;
				platform2 = platformRefs.bottom2;
				pointToModify.secondary = Room.Direction.Left;
			}
			else {
				platform1 = platformRefs.bottom2;
				platform2 = platformRefs.bottom1;
				pointToModify.secondary = Room.Direction.Right;
			}
			newSize = 10f;
		}
		else if (minPointName.Contains ("Left")) {
			pointToModify.main = Room.Direction.Left;
			if(minPointName.Contains("1")) {
				platform1 = platformRefs.left1;
				platform2 = platformRefs.left2;
				pointToModify.secondary = Room.Direction.Bottom;
			}
			else {
				platform1 = platformRefs.left2;
				platform2 = platformRefs.left1;
				pointToModify.secondary = Room.Direction.Top;
			}
			newSize = 7.5f;
		}
		else { //minPointName.Contains ("Right")
			pointToModify.main = Room.Direction.Right;
			if(minPointName.Contains("1")) {
				platform1 = platformRefs.right1;
				platform2 = platformRefs.right2;
				pointToModify.secondary = Room.Direction.Bottom;
			}
			else {
				platform1 = platformRefs.right2;
				platform2 = platformRefs.right1;
				pointToModify.secondary = Room.Direction.Top;
			}
			newSize = 7.5f;
		}
		pointExtraInfo.previousPlatform1.transform.localScale = new Vector2 (pointExtraInfo.previousSize, pointExtraInfo.previousPlatform1.transform.localScale.y);
		pointExtraInfo.previousPlatform2.transform.localScale = new Vector2 (pointExtraInfo.previousSize, pointExtraInfo.previousPlatform2.transform.localScale.y);

		platform1.transform.localScale = new Vector2 (0.5f, platform1.transform.localScale.y);
		platform2.transform.localScale = new Vector2 (newSize * 2 - pointGap, platform2.transform.localScale.y);

		pointExtraInfo.previousSize = newSize;
		pointExtraInfo.previousPlatform1 = platform1;
		pointExtraInfo.previousPlatform2 = platform2;
	}

	void Update () {
		float minDistance = float.MaxValue;
		GameObject minPoint = room;
		foreach (GameObject point in pointRefs.points) {
			float pointDistance = Vector2.Distance (exitPoint.point.transform.position, point.transform.position);
			if (pointDistance < minDistance) {
				minDistance = pointDistance;
				minPoint = point;
			}
		}
		exitPoint.point.transform.position = minPoint.transform.position;
		exitPoint.point.transform.rotation = minPoint.transform.rotation;
		HandlePlatformSize (minPoint.name, exitPoint, roomScript.exit);


		minDistance = float.MaxValue;
		minPoint = room;
		foreach (GameObject point in pointRefs.points) {
			float pointDistance = Vector2.Distance (entryPoint.point.transform.position, point.transform.position);
			if (pointDistance < minDistance) {
				minDistance = pointDistance;
				minPoint = point;
			}
		}
		entryPoint.point.transform.position = minPoint.transform.position;
		entryPoint.point.transform.rotation = minPoint.transform.rotation;
		HandlePlatformSize (minPoint.name, entryPoint, roomScript.entry);
	}
}
#endif