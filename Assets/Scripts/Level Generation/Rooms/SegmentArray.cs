using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This marks a victory over Unity's serialization.
 * Holds an array of rooms.
 */
[System.Serializable]
public class SegmentArray : CustomArray<RoomSegment, SegmentArrayList> { }