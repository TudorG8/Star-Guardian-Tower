using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSaver : Singleton<DataSaver> {
	[SerializeField] SerializableInt currentWeapon;
	[SerializeField] SerializableInt currentArmour;
	[SerializeField] SerializableInt totalGold    ;
	[SerializeField] SerializableInt highestScore ;
	[SerializeField] bool finishedTutorial;

	public SerializableInt CurrentWeapon { get { return currentWeapon   ; } }
	public SerializableInt CurrentArmour { get { return currentArmour   ; } }
	public SerializableInt TotalGold     { get { return totalGold       ; } set { totalGold        = value; } }
	public SerializableInt HighestScore  { get { return highestScore    ; } set { highestScore     = value; } }
	public bool FinishedTutorial { get { return finishedTutorial; } set { finishedTutorial = value; } }

	void Awake() {
		InitiateSingleton (); 
		totalGold.Value = 3000;
	}
}
