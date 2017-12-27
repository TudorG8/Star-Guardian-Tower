using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPropertyDrawers;

[System.Serializable]
public class RoomArray {
	[System.Serializable]
	public class ListWrapper {
		public List<RoomGeneratorRoomHelper> list;

		public ListWrapper() {
			list = new List<RoomGeneratorRoomHelper>();
		}
	}

	[SerializeField][ReadOnly] int rows, cols;
	[SerializeField] Transform attachedTo;
	[SerializeField] List<ListWrapper> array;

	public int Rows { get { return rows; } }
	public int Cols { get { return cols; } }

	public bool IsNull() {
		return array == null; 
	}

	public bool ThereIsOnlyOneRoom() {
		return Rows == 1 && Cols == 1; 
	}

	public List<RoomGeneratorRoomHelper> GetValidRooms() {
		List<RoomGeneratorRoomHelper> validRooms = new List<RoomGeneratorRoomHelper> ();
		for (int i = 0; i < Rows; i++) {
			for (int j = 0; j < Cols; j++) {
				if (array [i].list [j] != null)
					validRooms.Add (array [i].list [j]);
			}
		}
		return validRooms;
	}

	public void AddRowToTop   () {
		array.Add(new ListWrapper());
		for (int i = 0; i < cols; i++) 
			array [rows].list.Add(null);
		
		rows++;
	}
	public void AddRowToBottom() {
		IncreaseIndexes(Vector2.up);
		array.Insert (0, new ListWrapper());
		for (int i = 0; i < cols; i++) 
			array [0].list.Add(null);
		
		rows++;
	}
	public void AddRowToLeft() {
		IncreaseIndexes(Vector2.right);
		for (int i = 0; i < rows; i++) 
			array [i].list.Insert (0, null);
		
		cols++;
	}
	public void AddRowToRight() {
		for (int i = 0; i < rows; i++) 
			array [i].list.Add(null);
		
		cols++;
	}

	/**
	 * Increases the index of all the rooms by the vector given.
	 */
	public void IncreaseIndexes(Vector2 increase) {
		for (int i = 0; i < rows; i++) {
			for (int j = 0; j < cols; j++){
				if (array [i].list [j] != null) {
					array [i].list [j].index += increase;
					array [i].list [j].SetName ();
				}
			}
		}
	}

	public bool isValidPosition(Vector2 position) {
		if (position.x < 0 || position.x == cols) return false;
		if (position.y < 0 || position.y == rows) return false;
		return true;
	}

	public bool isNotAValidPosition(Vector2 position) {
		return !isValidPosition (position);
	}

	public RoomGeneratorRoomHelper RoomAt(Vector2 position) {
		if (!isValidPosition (position)) {
			Debug.LogError ("Bad position");
		}
		return array [(int)position.y].list [(int)position.x];
	}

	public void SetRoom(Vector2 position, RoomGeneratorRoomHelper newRoom) {
		if (!isValidPosition (position)) {
			Debug.LogError ("Bad position");
		}
		array [(int)position.y].list [(int)position.x] = newRoom;
	}

	public void Reset() {  
		array = new List<ListWrapper> ();
		//array.Add (new ListWrapper());
		cols = rows = 0;
		// Delete all rooms
		foreach (Transform room in attachedTo.transform) {
			if (room.name.Contains ("Room"))
				Object.DestroyImmediate (room.gameObject);
		}
	}

	public void PrintArray() {
		Debug.Log ("Rows: " + rows + " Cols: " + cols);

		for (int i = rows - 1; i >= 0; i--) {
			string message = "";
			for (int j = 0; j < cols; j++) {
				if (array [i].list [j] == null)
					message += "null ";
				else
					message += array [i].list [j].gameObject.name + " ";
			}
			Debug.Log (message);
		}
	}

	/**
	 * Checks for any null rows or cols and deletes them.
	 * Will check <top-to-bottom>, <bottom-to-top>, <right-to-left> and <left-to-right>
	 */
	public void DeleteUselessSpots() {
		// Top
		for (int i = rows - 1; i >= 0; i--) {
			bool foundEntity = false;
			for (int j = 0; j < cols; j++) {
				if (array [i].list [j] != null)
					foundEntity = true;
			}
			if (foundEntity) break;		
			else { //!foundEntity
				array.RemoveAt (i);
				rows--;
			}
		}

		// Bottom
		int rowsToDisplace = 0;
		for (int i = 0; i < rows; i++) {
			bool foundEntity = false;
			for (int j = 0; j < cols; j++) {
				if (array [i].list [j] != null)
					foundEntity = true;
			}
			if     (foundEntity)  break;
			else /*!foundEntity*/ rowsToDisplace++;	
		}
		for (int i = 0; i < rowsToDisplace; i++) 
			array.RemoveAt (0); 
		rows -= rowsToDisplace;
		IncreaseIndexes(new Vector2(0, -rowsToDisplace));

		// Right
		for (int j = cols - 1; j >= 0; j--) {
			bool foundEntity = false;
			for (int i = 0; i < rows; i++) {
				if (array [i].list [j] != null)
					foundEntity = true;
			}
			if (foundEntity) { break; } 
			else {
				for (int i = 0; i < rows; i++) 
					array [i].list.RemoveAt (j); 
				cols--;
			}
		}

		// Left
		int colsToDisplace = 0;
		for (int j = 0; j < cols; j++) {
			bool foundEntity = false;
			for (int i = 0; i < rows; i++) {
				if (array [i].list [j] != null)
					foundEntity = true;
			}
			if     (foundEntity)  break;
			else /*!foundEntity*/ colsToDisplace++;

		}
		for (int times = 0; times < colsToDisplace; times++) {
			for (int i = 0; i < rows; i++) 
				array [i].list.RemoveAt (0);
		}
		cols -= colsToDisplace;
		IncreaseIndexes(new Vector2(-colsToDisplace, 0));
	}
}
