using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomPropertyDrawers;

public class ScoreSystem : Singleton<ScoreSystem> {
	[SerializeField] Text     scoreText    ;
	[SerializeField] Animator scoreAnimator;

	[SerializeField] int fastThreshold;
	[SerializeField] int slowSpeed    ;
	[SerializeField] int fastSpeed    ;

	[SerializeField][ReadOnly] int currentScore;

	void Awake () { InitiateSingleton (); }

	void Update() {
		//scoreText.text = SessionData.Instance.CurrentGold.ToString ();
	}

	public void GainScore(int amount) {
		SessionData.Instance.CurrentScore += amount;
		int difference = SessionData.Instance.CurrentScore - currentScore;

		int instancesRequired = (difference >= fastThreshold) ? fastSpeed : slowSpeed;

		StopAllCoroutines ();
		for (int i = 0; i < instancesRequired; i++) {
			StartCoroutine(ScoreRoutine(0.1f));
		}
		scoreAnimator.SetTrigger ("scoreGained");
	}

	public void GainGold(int amount) {
		SessionData.Instance.CurrentGold += amount;
	}

	IEnumerator ScoreRoutine(float speed) {
		while (currentScore < SessionData.Instance.CurrentScore) {
			currentScore += 1;
			yield return new WaitForSeconds (speed);
		}
	}
}