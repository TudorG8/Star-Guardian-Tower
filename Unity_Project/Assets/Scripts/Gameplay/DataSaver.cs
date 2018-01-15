using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSaver : Singleton<DataSaver> {
	[SerializeField] bool finishedTutorial;

	public bool FinishedTutorial { get { return finishedTutorial; } }

	void Awake() { InitiateSingleton (); }
}
