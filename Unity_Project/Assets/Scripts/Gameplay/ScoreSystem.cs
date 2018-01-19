using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomPropertyDrawers;

public class ScoreSystem : Singleton<ScoreSystem> {
	[SerializeField] Text         currentScoreText    ;
	[SerializeField] Animator     currentScoreAnimator;
	[SerializeField] Text         differenceText      ;

	[SerializeField] ValueChanger valueChanger ;

	void Awake () { InitiateSingleton (); }

	void Update() {
		if (SessionData.Instance.GameStarted) {
			currentScoreText.text = valueChanger.CurrentAmount.ToString();
			if (valueChanger.Difference != 0) {
				char sign = valueChanger.Difference > 0 ? '+' : '-';
				differenceText  .text = sign + valueChanger.Difference.ToString();
			}
		}
	}

	public void GainScore(int amount) {
		valueChanger.GainAmount (SessionData.Instance.CurrentScore, amount);
		currentScoreAnimator.SetTrigger ("scoreGained");
	}

	public void GainGold(int amount) {
		SessionData.Instance.CurrentGold.Value += amount;
	}
}