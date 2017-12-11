using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
[ExecuteInEditMode]
public class RoomSceneEditor : MonoBehaviour {
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

		public List<PointHelper> points;
	}
	public RoomSceneHelper roomSceneHelper;
	public Room roomScript;

	public PlatformRefs platformRefs;
	public PointRefs    pointRefs   ;

	public GameObject room    ;

	public RoomSceneEditor top   ;
	public RoomSceneEditor bottom;
	public RoomSceneEditor left  ;
	public RoomSceneEditor right ;

	public void AddRoomToTheTop() {
		if (top != null)
			return;
		
		Vector2 spawnPosition = transform.position;
		spawnPosition += new Vector2 (0f, 15f);

		GameObject roomObj = Instantiate (roomSceneHelper.roomPrefab, spawnPosition, Quaternion.identity) as GameObject;
		roomObj.transform.SetParent (roomSceneHelper.transform);
		roomObj.transform.SetAsLastSibling ();

		RoomSceneEditor roomEditorScript = roomObj.GetComponent<RoomSceneEditor> ();
		roomEditorScript.roomSceneHelper = roomSceneHelper;
		roomEditorScript.bottom = this;

		roomSceneHelper.rooms.Add (roomEditorScript);

		top = roomEditorScript;

		pointRefs.top1.SetActive (false);
		pointRefs.top2.SetActive (false);
		platformRefs.top1.transform.localScale = new Vector2 (0.5f, platformRefs.top1.transform.localScale.y);
		platformRefs.top2.transform.localScale = new Vector2 (0.5f, platformRefs.top1.transform.localScale.y);

		roomEditorScript.pointRefs.bottom1.SetActive (false);
		roomEditorScript.pointRefs.bottom2.SetActive (false);
		roomEditorScript.platformRefs.bottom1.transform.localScale = new Vector2 (0.5f, platformRefs.top1.transform.localScale.y);
		roomEditorScript.platformRefs.bottom2.transform.localScale = new Vector2 (0.5f, platformRefs.top1.transform.localScale.y);
	}
	public void AddRoomToTheBottom() {
		if (bottom != null)
			return;

		Vector2 spawnPosition = transform.position;
		spawnPosition += new Vector2 (0f, -15f);

		GameObject roomObj = Instantiate (roomSceneHelper.roomPrefab, spawnPosition, Quaternion.identity) as GameObject;
		roomObj.transform.SetParent (roomSceneHelper.transform);
		roomObj.transform.SetAsLastSibling ();

		RoomSceneEditor roomEditorScript = roomObj.GetComponent<RoomSceneEditor> ();
		roomEditorScript.roomSceneHelper = roomSceneHelper;
		roomEditorScript.top = this;

		roomSceneHelper.rooms.Add (roomEditorScript);

		bottom = roomEditorScript;

		pointRefs.bottom1.SetActive (false);
		pointRefs.bottom2.SetActive (false);
		platformRefs.bottom1.transform.localScale = new Vector2 (0.5f, platformRefs.top1.transform.localScale.y);
		platformRefs.bottom2.transform.localScale = new Vector2 (0.5f, platformRefs.top1.transform.localScale.y);

		roomEditorScript.pointRefs.top1.SetActive (false);
		roomEditorScript.pointRefs.top2.SetActive (false);
		roomEditorScript.platformRefs.top1.transform.localScale = new Vector2 (0.5f, platformRefs.top1.transform.localScale.y);
		roomEditorScript.platformRefs.top2.transform.localScale = new Vector2 (0.5f, platformRefs.top1.transform.localScale.y);
	}
	public void AddRoomToTheLeft() {
		if (left != null)
			return;

		Vector2 spawnPosition = transform.position;
		spawnPosition += new Vector2 (-20f, 0f);

		GameObject roomObj = Instantiate (roomSceneHelper.roomPrefab, spawnPosition, Quaternion.identity) as GameObject;
		roomObj.transform.SetParent (roomSceneHelper.transform);
		roomObj.transform.SetAsLastSibling ();

		RoomSceneEditor roomEditorScript = roomObj.GetComponent<RoomSceneEditor> ();
		roomEditorScript.roomSceneHelper = roomSceneHelper;
		roomEditorScript.right = this;

		roomSceneHelper.rooms.Add (roomEditorScript);

		left = roomEditorScript;

		pointRefs.left1.SetActive (false);
		pointRefs.left2.SetActive (false);
		platformRefs.left1.transform.localScale = new Vector2 (0.5f, platformRefs.top1.transform.localScale.y);
		platformRefs.left2.transform.localScale = new Vector2 (0.5f, platformRefs.top1.transform.localScale.y);

		roomEditorScript.pointRefs.right1.SetActive (false);
		roomEditorScript.pointRefs.right2.SetActive (false);
		roomEditorScript.platformRefs.right1.transform.localScale = new Vector2 (0.5f, platformRefs.top1.transform.localScale.y);
		roomEditorScript.platformRefs.right2.transform.localScale = new Vector2 (0.5f, platformRefs.top1.transform.localScale.y);
	}
	public void AddRoomToTheRight() {
		if (right != null)
			return;

		Vector2 spawnPosition = transform.position;
		spawnPosition += new Vector2 (20f, 0f);

		GameObject roomObj = Instantiate (roomSceneHelper.roomPrefab, spawnPosition, Quaternion.identity) as GameObject;
		roomObj.transform.SetParent (roomSceneHelper.transform);
		roomObj.transform.SetAsLastSibling ();

		RoomSceneEditor roomEditorScript = roomObj.GetComponent<RoomSceneEditor> ();
		roomEditorScript.roomSceneHelper = roomSceneHelper;
		roomEditorScript.left = this;

		roomSceneHelper.rooms.Add (roomEditorScript);

		right = roomEditorScript;

		pointRefs.right1.SetActive (false);
		pointRefs.right2.SetActive (false);
		platformRefs.right1.transform.localScale = new Vector2 (0.5f, platformRefs.top1.transform.localScale.y);
		platformRefs.right2.transform.localScale = new Vector2 (0.5f, platformRefs.top1.transform.localScale.y);

		roomEditorScript.pointRefs.left1.SetActive (false);
		roomEditorScript.pointRefs.left2.SetActive (false);
		roomEditorScript.platformRefs.left1.transform.localScale = new Vector2 (0.5f, platformRefs.top1.transform.localScale.y);
		roomEditorScript.platformRefs.left2.transform.localScale = new Vector2 (0.5f, platformRefs.top1.transform.localScale.y);
	}
}
#endif