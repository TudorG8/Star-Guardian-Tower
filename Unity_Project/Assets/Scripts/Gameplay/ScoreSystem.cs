using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomPropertyDrawers;

public class ScoreSystem : Singleton<ScoreSystem> {
	[SerializeField] Text     scoreText    ;
	[SerializeField] Animator scoreAnimator;

	[SerializeField] int totalScore;
	[SerializeField] int totalGold ;

	[SerializeField] int fastThreshold;
	[SerializeField] int slowSpeed    ;
	[SerializeField] int fastSpeed    ;

	[SerializeField][ReadOnly] int currentScore;

	void Awake () { InitiateSingleton (); }

	void Update() {
		scoreText.text = totalScore.ToString ();
	}

	public void GainScore(int amount) {
		totalScore += amount;
		int difference = totalScore - currentScore;

		int instancesRequired = (difference >= fastThreshold) ? fastSpeed : slowSpeed;

		StopAllCoroutines ();
		for (int i = 0; i < instancesRequired; i++) {
			StartCoroutine(ScoreRoutine(0.1f));
		}
		scoreAnimator.SetTrigger ("scoreGained");
	}

	public void GainGold(int amount) {
		totalGold += amount;
	}

	IEnumerator ScoreRoutine(float speed) {
		while (currentScore < totalScore) {
			currentScore += 1;
			yield return new WaitForSeconds (speed);
		}
	}
}