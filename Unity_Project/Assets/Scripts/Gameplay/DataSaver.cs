using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSaver : Singleton<DataSaver> {
	[SerializeField] SerializableInt currentWeapon;
	[SerializeField] SerializableInt currentArmour;
	[SerializeField] int  totalGold       ;
	[SerializeField] int  highestScore    ;
	[SerializeField] bool finishedTutorial;

	public SerializableInt CurrentWeapon { get { return currentWeapon   ; } }
	public SerializableInt CurrentArmour { get { return currentArmour   ; } }
	public int  TotalGold        { get { return totalGold       ; } set { totalGold        = value; } }
	public int  HighestScore     { get { return highestScore    ; } set { highestScore     = value; } }
	public bool FinishedTutorial { get { return finishedTutorial; } set { finishedTutorial = value; } }

	void Awake() {
		InitiateSingleton (); 
		totalGold = 2000;
	}

}
