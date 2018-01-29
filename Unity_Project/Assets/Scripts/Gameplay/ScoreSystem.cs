using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomPropertyDrawers;

/**
 * Runtime Session Score system that holds the portrait, hitpoints, score and gold.
 */

public class ScoreSystem : Singleton<ScoreSystem> {
	[SerializeField] Animator     parent    ;
	[SerializeField] Animator     gameOverUI;
	[SerializeField] GameObject   tutorialUI;

	[SerializeField] Text         currentScoreText    ;
	[SerializeField] Animator     currentScoreAnimator;
	[SerializeField] ValueChanger scoreChanger        ;

	[SerializeField] Text         currentGoldText     ;
	[SerializeField] Text         goldDifferenceText  ;
	[SerializeField] ValueChanger goldChanger         ;

	[SerializeField] Transform    hitPointsParent     ;
	[SerializeField] GameObject   hitPointsBar        ;
	[SerializeField] List<Image>  hitPointsInstances  ;

	[SerializeField] Color normalHP ;
	[SerializeField] Color damagedHP;

	void Awake () { 
		InitiateSingleton (); 
		Reset ();
		parent.gameObject.SetActive (false);
	}

	public void Reset () {
		currentScoreText  .text = "0";
		currentGoldText   .text = "0";
		goldDifferenceText.text =  "";
		scoreChanger.Reset ();
		goldChanger.Reset ();
	}
		
	void Update() {
		if (SessionData.Instance.GameStarted) {
			currentScoreText.text = scoreChanger.CurrentAmount.ToString();
			currentGoldText .text = goldChanger .CurrentAmount.ToString();
			if   (goldChanger.Difference != 0)  { goldDifferenceText.text = goldChanger.GetDifferenceSign() + goldChanger.Difference.ToString (); } 
			else/*goldChanger.Difference == 0*/ { goldDifferenceText.text = ""; }
		}
	}

	public void LoadHP() {
		foreach (Transform child in hitPointsParent) {
			Destroy (child.gameObject);
		}
		hitPointsInstances.Clear ();
		for (int i = 0; i < SessionData.Instance.Lives.Value; i++) {
			GameObject obj = Instantiate (hitPointsBar, new Vector2(), Quaternion.identity) as GameObject;
			obj.transform.SetParent (hitPointsParent, false);
			obj.transform.SetAsLastSibling ();
			hitPointsInstances.Add(obj.GetComponent<Image>());
		}
		Reset ();
	}

	public void TakeDamage() {
		hitPointsInstances [(int)SessionData.Instance.Lives.Value].color = damagedHP;
	}

	/**
	 * Should be called by events to gain a specific amount of score during a runtime session.
	 */
	public void GainScore(int amount) {
		scoreChanger.GainAmount (SessionData.Instance.CurrentScore, amount);
		//currentScoreAnimator.SetTrigger ("scoreGained");
	}

	/**
	 * Should be called by events to gain a specific amount of gold during a runtime session.
	 */
	public void GainGold (int amount) {
		goldChanger.GainAmount (SessionData.Instance.CurrentGold , amount);
	}

	public void ShowTutorialUI() {
		tutorialUI.SetActive (true);
	}

	public void HideTutorialUI() {
		tutorialUI.SetActive (false);
	}

	public void ShowGameUI() {
		LoadHP ();
		parent.gameObject.SetActive (true);
	}

	public void ShowGameoverUI () {
		gameOverUI.gameObject.SetActive (true);
	}
	public void HideGameoverUI () {
		gameOverUI.gameObject.SetActive (false);
	}
	public void HideGameUI() {
		parent.gameObject.SetActive (false);
	}

	public void QuitGame() {
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#endif
		Application.Quit ();
	}
}