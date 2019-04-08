using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectInterfaces;

/**
 * Dummy class for platforms until more logic is needed.
 */

public class Platform : RoomObject, IStartable, IResetable  {
	public void OnStart () { StartEffects (); }
	public void OnReset() { ResetEffects (); }
}
