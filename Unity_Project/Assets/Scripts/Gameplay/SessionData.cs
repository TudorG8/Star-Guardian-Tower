using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionData : Singleton<SessionData> {
	[SerializeField] bool gameStarted ;
	[SerializeField] Stat lives       ;
	[SerializeField] SerializableInt currentGold ;
	[SerializeField] SerializableInt currentScore;

	public bool GameStarted  { get { return gameStarted; } set { gameStarted  = value; } }
	public Stat Lives  { get { return lives; } set { lives  = value; } }
	public SerializableInt CurrentGold  { get { return currentGold ; } set { currentGold  = value; } }
	public SerializableInt CurrentScore { get { return currentScore; } set { currentScore = value; } }

	void Awake() {
		InitiateSingleton ();
	}

	public void Reset () {
		gameStarted  = false;
		currentGold.Value  = 0;
		currentScore.Value = 0;
	}

	public void SaveStats() {
		DataSaver.Instance.TotalGold.Value += currentGold.Value;
		if (currentScore.Value > DataSaver.Instance.HighestScore.Value)
			DataSaver.Instance.HighestScore.Value = currentScore.Value;
	}

	public void UpdateLives(float lives) {
		this.lives.Max   = lives;
		this.lives.Value = lives;
	}
}