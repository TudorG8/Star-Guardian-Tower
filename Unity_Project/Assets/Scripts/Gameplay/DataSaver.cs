using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This class holds everything that will actually be saved between play sessions.
 * It is what will actually be saved whenever the player quits, eventually...
 */

public class DataSaver : Singleton<DataSaver> {
	[SerializeField] SerializableInt currentWeapon   ;
	[SerializeField] SerializableInt currentArmour   ;
	[SerializeField] SerializableInt totalGold       ;
	[SerializeField] SerializableInt highestScore    ;
	[SerializeField] bool            finishedTutorial;

	public SerializableInt CurrentWeapon    { get { return currentWeapon   ; } }
	public SerializableInt CurrentArmour    { get { return currentArmour   ; } }
	public SerializableInt TotalGold        { get { return totalGold       ; } }
	public SerializableInt HighestScore     { get { return highestScore    ; } }
	public bool            FinishedTutorial { get { return finishedTutorial; } set { finishedTutorial = value; } }

	void Awake() { InitiateSingleton (); }

	/**
	 * Insert code to save the data locally
	 */
	public void Save () {
	}
}