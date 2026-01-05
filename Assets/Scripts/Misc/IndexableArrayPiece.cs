using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This interface is used for arrays that have indexable elements.
 * Example: Used by RoomArray to hold all the room segments, which have indexes.
 */

public interface IndexableArrayPiece<T> {
	void       IncreaseIndex    (Vector2 increase); // Increase the index field of the element
	void       IncreasePosition (Vector2 increase); // Increase the world position of the element
	GameObject GetGameObject ();                    // Get the elements gameobject
	void       SetName ();                          // Set the name of the elements gameobject
}