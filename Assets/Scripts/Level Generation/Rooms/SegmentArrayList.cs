using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 * This marks a victory over Unity's serialization.
 * Holds an row of rooms.
 */
[System.Serializable]
public class SegmentArrayList : CustomArrayList<RoomSegment> { }