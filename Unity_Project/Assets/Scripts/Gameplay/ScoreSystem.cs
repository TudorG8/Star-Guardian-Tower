using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomPropertyDrawers;

/**
 * Runtime Session Score system that holds the portrait, hitpoints, score and gold.
 */

public class ScoreSystem : Singleton<ScoreSystem> {
	[SerializeField] Text         currentScoreText    ;
	[SerializeField] Animator     currentScoreAnimator;

	[SerializeField] Text         currentGoldText     ;
	[SerializeField] Text         goldDifferenceText  ;
	[SerializeField] ValueChanger valueChanger        ;

	void Awake () { InitiateSingleton (); }

	void Update() {
		if (SessionData.Instance.GameStarted) {
			currentScoreText.text = valueChanger.CurrentAmount.ToString();
			if   (valueChanger.Difference != 0)  { goldDifferenceText.text = valueChanger.Difference.ToString (); } 
			else/*valueChanger.Difference == 0*/ { goldDifferenceText.text = ""; }
		}
	}

	/**
	 * Should be called by events to gain a specific amount of score during a runtime session.
	 */
	public void GainScore(int amount) {
		valueChanger.GainAmount (SessionData.Instance.CurrentScore, amount);
		currentScoreAnimator.SetTrigger ("scoreGained");
	}

	/**
	 * Should be called by events to gain a specific amount of gold during a runtime session.
	 */
	public void GainGold (int amount) {
		valueChanger.GainAmount (SessionData.Instance.CurrentGold , amount);
	}

	public void ShowGameUI() {
	}

	public void ShowGameoverUI () {
	}
}