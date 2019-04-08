using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectInterfaces;

public class Decoration : RoomObject, IStartable, IResetable {
	public void OnStart () { StartEffects (); }
	public void OnReset () { ResetEffects (); }
}
