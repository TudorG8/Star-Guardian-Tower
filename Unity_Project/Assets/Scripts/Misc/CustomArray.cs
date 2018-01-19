using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomPropertyDrawers;

/**
 * Custom array that supports indexable pieces.
 * It also holds the coordinates in revers order:
 * 	X -> cols
 *  Y -> rows
 *  So doing a SetRoom(0, 2) will set the room at column(x) 0 and row(y) 2
 *  Why do it like this? It just makes more sense in practice than explanation...
 * 
 * Generic Explanation:
 * 	In order to implement this, you need to create a non generic class that extends it and specifies its parameters.
 *  U is a single element 
 *  T is row/list of U
 *  Example:
 * 		SegmentArray : CustomArray<RoomSegment, SegmentArrayList> { }
 * 		This will now properly serialize, even though it is a generic class
 */

[System.Serializable]
public class CustomArray <U, T> 
	where U : IndexableArrayPiece<U>, new()
	where T : CustomArrayList    <U>, new() 
{
	[SerializeField][ReadOnly] int rows, cols;
	[SerializeField] Transform attachedTo;
	[SerializeField] List<T> array;

	public int Rows { get { return rows; } }
	public int Cols { get { return cols; } }

	public bool IsNull() {
		return array == null; 
	}

	public bool ThereIsOnlyOneRoom() {
		return Rows == 1 && Cols == 1; 
	}

	// Returns a list with all valid elements
	public List<U> GetValidElements() {
		List<U> validRooms = new List<U> ();
		for (int i = 0; i < Rows; i++) {
			for (int j = 0; j < Cols; j++) {
				if (array [i].list [j] != null)
					validRooms.Add (array [i].list [j]);
			}
		}
		return validRooms;
	}

	public void AddRowToTop   () {
		array.Add(new T());
		for (int i = 0; i < cols; i++) {
			array [rows].list.Add (default(U));
		}

		rows++;
	}
	public void AddRowToBottom() {
		IncreaseIndexes(Vector2.up);
		array.Insert (0, new T());
		for (int i = 0; i < cols; i++) {
			array [0].list [i] = default(U);
		}
		
		rows++;
	}
	public void AddRowToLeft() {
		IncreaseIndexes(Vector2.right);
		for (int i = 0; i < rows; i++) 
			array [i].list.Insert (0, default(U));
		
		cols++;
	}
	public void AddRowToRight() {
		for (int i = 0; i < rows; i++) 
			array [i].list.Add(default(U));
		
		cols++;
	}

	// Increases the index of all the rooms by the vector given.
	public void IncreaseIndexes(Vector2 increase) {
		for (int i = 0; i < rows; i++) {
			for (int j = 0; j < cols; j++){
				if (array [i].list [j] != null) {
					array [i].list [j].IncreaseIndex    (increase);
					array [i].list [j].IncreasePosition (UsefulMethods.vectorProduct (increase, new Vector2 (26.7f, 15f)));
					array [i].list [j].SetName ();
				}
			}
		}
	}

	// Returns true if the given position would be a valid position
	public bool IsValidPosition(Vector2 position) {
		if (position.x < 0 || position.x == cols) return false;
		if (position.y < 0 || position.y == rows) return false;
		return true;
	}

	// Returns true if the given position would be an invalid position
	public bool IsNotAValidPosition(Vector2 position) {
		return !IsValidPosition (position);
	}

	public U RoomAt(Vector2 position) {
		if (IsNotAValidPosition (position)) { Debug.LogError ("Bad position"); }
		return array [(int)position.y].list [(int)position.x];
	}

	public void SetRoom(Vector2 position, U newRoom) {
		if (IsNotAValidPosition (position)) { Debug.LogError ("Bad position"); }
		array [(int)position.y].list [(int)position.x] = newRoom;
	}

	public void Reset() {  
		array = new List<T> (); 
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
					message += array [i].list [j].GetGameObject().name + " ";
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
