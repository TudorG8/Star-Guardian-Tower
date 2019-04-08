using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectInterfaces;

/**
 * Dummy class for hazards until more logic is needed.
 */

public class Hazard : RoomObject, IStartable, IResetable {
	public void OnStart() { StartEffects (); }
	public void OnReset() { ResetEffects (); }
}