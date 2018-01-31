using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Interfaces for room objects.
 * Once you have the base class (RoomObject), check if it fullfills any of the following interfaces.
 */

namespace ObjectInterfaces {
	// Denotes an object that can be destroyed (but not actually, just turned off)
	public interface IDestroyable {
		void OnDestroy();
	}
	// Denotes an object that can be picked up by walking over it
	public interface IPickable {
		void OnPickup();
	}
	// Denotes an object that will be reset when a room is returned to the cache (so almost everything)
	public interface IResetable {
		void OnReset();
	}
	// Denotes an object that will start its actions only when the player enters the room (or a trigger happens)
	public interface IStartable {
		void OnStart();
		void OnStop ();
	}
}