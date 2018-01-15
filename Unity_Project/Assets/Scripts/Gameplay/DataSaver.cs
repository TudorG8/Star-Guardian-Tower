using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSaver : Singleton<DataSaver> {
	[SerializeField] SerializableInt currentWeapon;
	[SerializeField] SerializableInt currentArmour;
	[SerializeField] float totalGold       ;
	[SerializeField] float highestScore    ;
	[SerializeField] bool  finishedTutorial;

	public float TotalGold        { get { return totalGold       ; } }
	public bool  HighestScore     { get { return highestScore    ; } }
	public bool  FinishedTutorial { get { return finishedTutorial; } }
	public SerializableInt CurrentWeapon { get { return currentWeapon   ; } }
	public SerializableInt CurrentArmour { get { return currentArmour   ; } }

	void Awake() { InitiateSingleton (); }
}
