#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPropertyDrawers;

[ExecuteInEditMode]
public class RoomGenerator : MonoBehaviour {
	[System.Serializable]
	public class PointExtraInfo {
		public Transform  point;
		public Transform  previousPoint;
		public GameObject previousPlatform1;
		public GameObject previousPlatform2;
		public float      previousSize;
	}
	// Imports
	[SerializeField] public GameObject roomPrefab;

	public RoomGeneratorRoomHelper originalRoom;

	// Fields
	[SerializeField] PointExtraInfo entryPoint;
	[SerializeField] PointExtraInfo exitPoint ;
	[SerializeField] float pointGap;
	[SerializeField] Vector2 minimumPlatformSize;
	[SerializeField] Vector2 roomSize;

	[SerializeField][ReadOnly] int rows, cols;
	List<List<RoomGeneratorRoomHelper>> arr;

	// Properties
	public Vector2 MinimumPlatformSize { get { return minimumPlatformSize;} }
	public Vector2 RoomSize            { get { return roomSize           ;} }

	public void AddRowToTop   () {
		arr.Add(new List<RoomGeneratorRoomHelper>());
		for (int i = 0; i < cols; i++) {
			arr [rows].Add(null);
		}
		rows++;
	}
	public void AddRowToBottom() {
		for (int i = 0; i < rows; i++) {
			for (int j = 0; j < cols; j++){
				if (arr [i] [j] != null) {
					arr [i] [j].index.y++;
					arr [i] [j].SetName ();
				}
			}
		}
		arr.Insert (0, new List<RoomGeneratorRoomHelper> ());
		for (int i = 0; i < cols; i++) {
			arr [0].Add(null);
		}
		rows++;
	}
	public void AddRowToLeft() {
		for (int i = 0; i < rows; i++) {
			for (int j = 0; j < cols; j++){
				if (arr [i] [j] != null) {
					arr [i] [j].index.x++;
					arr [i] [j].SetName ();
				}
			}
		}
		for (int i = 0; i < rows; i++) {
			arr [i].Insert (0, null);
		}
		cols++;
	}
	public void AddRowToRight() {
		for (int i = 0; i < rows; i++) {
			arr [i].Add(null);
		}
		cols++;
	}

	public void CheckForNearbyRooms(Vector2 index, PointDTO.Direction side) {
		Vector2 desiredPosition = index + PointDTO.GetDirectionVector (side);
		if (desiredPosition.x < 0 || desiredPosition.x == cols)
			return;
		if (desiredPosition.y < 0 || desiredPosition.y == rows)
			return;
		RoomGeneratorRoomHelper neighbour = arr [(int)desiredPosition.y] [(int)desiredPosition.x];
		RoomGeneratorRoomHelper actual    = arr [(int)index.y] [(int)index.x];
		if (neighbour != null) {
			actual.pointRefs   .TurnOff   (side);
			actual.platformRefs.SetXScale (side, 0);

			neighbour.pointRefs   .TurnOff   (PointDTO.GetOpposite(side));
			neighbour.platformRefs.SetXScale (PointDTO.GetOpposite(side), 0);
		}
	}

	public void DestroyRoom(Vector2 position) {
		if (arr == null) {
			ResetArray ();
		}
		if (rows == 1 && cols == 1) {
			Debug.LogError ("Cannot destroy the only room");
			return;
		}
	}

	public void AddRoom(Vector2 originalPosition, PointDTO.Direction direction, RoomGeneratorRoomHelper newRoom) {
		if (arr == null) {
			ResetArray ();
		}
		Vector2 desiredPosition = originalPosition + PointDTO.GetDirectionVector (direction);
		if      (desiredPosition.x >= cols) AddRowToRight  ();
		else if (desiredPosition.x <     0) AddRowToLeft   ();
		else if (desiredPosition.y >= rows) AddRowToTop    ();
		else if (desiredPosition.y <     0) AddRowToBottom ();

		if (desiredPosition.x < 0) desiredPosition.x = 0;
		if (desiredPosition.y < 0) desiredPosition.y = 0;
		newRoom.index = desiredPosition;
		Debug.Log (desiredPosition);
		arr [(int)desiredPosition.y] [(int)desiredPosition.x] = newRoom;
		newRoom.SetName ();
		CheckForNearbyRooms (desiredPosition, PointDTO.Direction.Top   );
		CheckForNearbyRooms (desiredPosition, PointDTO.Direction.Bottom);
		CheckForNearbyRooms (desiredPosition, PointDTO.Direction.Left  );
		CheckForNearbyRooms (desiredPosition, PointDTO.Direction.Right );
	}

	public void ResetArray() {
		arr = new List<List<RoomGeneratorRoomHelper>> ();
		arr.Add (new List<RoomGeneratorRoomHelper> ());
		arr [0].Add (originalRoom);
		cols = rows = 1;
	}

	public void PrintArray() {
		Debug.Log ("Rows: " + rows + " Cols: " + cols);

		for (int i = rows - 1; i >= 0; i--) {
			string message = "";
			for (int j = 0; j < cols; j++) {
				if (arr [i] [j] == null)
					message += "null ";
				else
					message += arr [i] [j].gameObject.name + " ";
			}
			Debug.Log (message);
		}
	}

	void HandlePlatformSize(string minPointName, PointExtraInfo pointExtraInfo, PointDTO pointToModify, PointHelper pointHelper, RoomGeneratorRoomHelper.PlatformRefs platformRefs) {
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

	void Start() {
		rows = cols = 1;
	}

	void Update () {
		// Gather all points from all rooms
		List<PointHelper> validPoints = new List<PointHelper>();
		for (int i = 0; i < rows; i++) {
			for (int j = 0; j < cols; j++) {
				validPoints.Add (arr[i][j]);
			}
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

		// Handle entry point
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