using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionData : Singleton<SessionData> {
	[SerializeField] bool gameStarted ;
	[SerializeField] Stat lives       ;
	[SerializeField] SerializableInt currentGold ;
	[SerializeField] SerializableInt currentScore;

	public int GameStarted  { get { return currentScore; } set { gameStarted  = value; } }
	public Stat Lives  { get { return lives; } set { lives  = value; } }
	public SerializableInt CurrentGold  { get { return currentGold ; } set { currentGold  = value; } }
	public SerializableInt CurrentScore { get { return currentScore; } set { currentScore = value; } }

	public void Reset () {
		gameStarted  = false;
		currentGold  = 0;
		currentScore = 0;
	}

	public void SaveStats() {
		DataSaver.Instance.TotalGold += currentGold;
		if (currentScore > DataSaver.Instance.HighestScore)
			DataSaver.Instance.HighestScore = currentScore;
	}

	public void UpdateLives(float lives) {
		this.lives.Max   = lives;
		this.lives.Value = lives;
	}
}