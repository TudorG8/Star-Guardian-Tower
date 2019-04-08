using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : Singleton <Shop> {
	// Scriptable Objects
	[SerializeField] List<ShopItem> weapons;
	[SerializeField] List<ShopItem> armours;

	// Imports
	[SerializeField] ValueChanger valueChanger      ;
	[SerializeField] ShopItemRefs weaponRefs        ;
	[SerializeField] ShopItemRefs armourRefs        ;
	[SerializeField] Text         goldText          ;
	[SerializeField] Text         goldDifferenceText;
	[SerializeField] Text         scoreText         ;

	public List<ShopItem> Weapons    { get { return weapons; } }
	public List<ShopItem> Armours    { get { return armours; } }
	public ShopItemRefs   WeaponRefs { get { return weaponRefs; } }
	public ShopItemRefs   ArmourRefs { get { return armourRefs; } }

	public void SaveStats() {
		valueChanger.SetUp (DataSaver.Instance.TotalGold);
	}

	void Awake() { InitiateSingleton ();}

	void Start() {
		valueChanger.SetUp (DataSaver.Instance.TotalGold);
	}


	void Update() {
		scoreText.text = DataSaver.Instance.HighestScore.Value.ToString();
		goldText .text = valueChanger.CurrentAmount.ToString ();
		if   (valueChanger.Difference != 0)  { goldDifferenceText.text = valueChanger.Difference.ToString (); } 
		else/*valueChanger.Difference == 0*/ { goldDifferenceText.text = ""; }
	}

	public ShopItem GetCurrentWeapon() { return weapons [DataSaver.Instance.CurrentWeapon.Value]; }
	public ShopItem GetCurrentArmour() { return armours [DataSaver.Instance.CurrentArmour.Value]; }
		
	// Purchasing
	/**
	 * Purchase the next level of the weapons.
	 */
	public void BuyWeapon() {
		bool purchased = BuyItem (DataSaver.Instance.CurrentWeapon, weapons, weaponRefs);
		if (purchased) {
			PlayerLoader.Instance.LoadCurrentWeapon ();
		}
	}

	/**
	 * Purchase the next level of the armours.
	 */
	public void BuyArmour () {
		bool purchased = BuyItem (DataSaver.Instance.CurrentArmour, armours, armourRefs);
		if (purchased) {
			PlayerLoader.Instance.LoadCurrentArmour ();
		}
	}

	/**
	 * Generic version to buy an item.
	 * First checks if the player has enough resources.
	 * Then it will update the shop UI, depending on whether the player purchased the last item or not.
	 */
	bool BuyItem (SerializableInt index, List<ShopItem> itemList, ShopItemRefs itemRefs) {
		if (index.Value < itemList.Count - 1) {
			ShopItem item = itemList [index.Value + 1];
			bool purchased = PurchaseItem (item);
			if (purchased) {
				index.Value++;
				// Bought the last item
				if (index.Value == itemList.Count - 1) {
					itemRefs.DisableBuying ();
					itemRefs.UpdateRefs (item);
				} 
				else {
					ShopItem nextItem = itemList [index.Value + 1];
					itemRefs.UpdateRefs (item, nextItem);
				}
				ShopKeeper.Instance.LoadPurchaseQuote ();
				return true;
			}
		}
		return false;
	}

	/**
	 * Used to check whether the player has enough resources to buy an item.
	 */
	bool PurchaseItem(ShopItem item) {
		if (DataSaver.Instance.TotalGold.Value >= item.Cost) {
			valueChanger.GainAmount(DataSaver.Instance.TotalGold, -item.Cost);
			return true;
		}
		return false;
	}
}

