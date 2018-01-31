using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * For loading quotes
 */

public class ShopKeeper : Singleton<ShopKeeper> {
	[SerializeField] Text text;
	[SerializeField] string welcomeQuote;
	[SerializeField] string purchaseQuote;
	[SerializeField] List<string> deathQuotes;

	void Awake() {
		InitiateSingleton ();
		LoadWelcomeQuote ();
	}
	public void LoadWelcomeQuote() {
		text.text = welcomeQuote;
	}
	public void LoadPurchaseQuote() {
		text.text = purchaseQuote;
	}
	public void LoadDeathQuote() {
		int random = Random.Range (0, deathQuotes.Count);
		text.text = deathQuotes [random];
	}
}
