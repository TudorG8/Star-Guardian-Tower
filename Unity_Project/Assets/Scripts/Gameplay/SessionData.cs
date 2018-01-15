using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionData : Singleton<SessionData> {
	[SerializeField] int currentGold ;
	[SerializeField] int currentScore;

	public int CurrentGold  { get { return currentGold ; } set { currentGold  = value; } }
	public int CurrentScore { get { return currentScore; } set { currentScore = value; } }

	public void Reset () {
		currentGold  = 0;
		currentScore = 0;
	}

	public void SaveStats() {
		DataSaver.Instance.TotalGold += currentGold;
		if (currentScore > DataSaver.Instance.HighestScore)
			DataSaver.Instance.HighestScore = currentScore;
	}
}