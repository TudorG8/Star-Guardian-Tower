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

	public void SaveStats() {
		valueChanger.SetUp (DataSaver.Instance.TotalGold);
	}

	void Awake() {
		InitiateSingleton ();
	}

	void Start() {
		DataSaver.Instance.TotalGold.Value = 3000;
		LoadInitialData ();
		valueChanger.SetUp (DataSaver.Instance.TotalGold);
	}


	void Update() {
		scoreText.text = DataSaver.Instance.HighestScore.Value.ToString();
		goldText .text = valueChanger.CurrentAmount.ToString ();
		if   (valueChanger.Difference != 0)  { goldDifferenceText.text = valueChanger.Difference.ToString (); } 
		else/*valueChanger.Difference == 0*/ { goldDifferenceText.text = ""; }
	}

	public ShopItem GetWeapon() { return weapons [DataSaver.Instance.CurrentWeapon.Value]; }
	public ShopItem GetArmour() { return armours [DataSaver.Instance.CurrentArmour.Value]; }

	// Loading
	/**
	 * To be used to load the current weapon and armour into the shop and player.
	 */
	void LoadInitialData() {
		LoadItem (DataSaver.Instance.CurrentWeapon, weapons, weaponRefs);
		PlayerController.Instance.GetPlayerRefs.UpdateRefs (GetWeapon ().GetSprites ());

		LoadItem (DataSaver.Instance.CurrentArmour, armours, armourRefs);
		PlayerController.Instance.GetPlayerRefs.UpdateRefs (GetArmour ().GetSprites ());
		int index = GetArmour ().HasAttribute ("Hit Points");
		if (index == -1) {
			Debug.LogError ("Armour does not have hit points");
			return;
		}
		SessionData.Instance.Lives.Max = (int) GetArmour ().GetAttribute (index).Value;
	}

	/**
	 * Loads an item onto the shop.
	 * @param index    : index of the item
	 * @param itemList : list item belongs in
	 * @param itemRefs : references to the UI elements
	 */
	void LoadItem(SerializableInt index, List<ShopItem> itemList, ShopItemRefs itemRefs) {
		int currentIndex = index.Value < itemList.Count - 1? index.Value + 1 : itemList.Count - 1;
		bool final = (index.Value == itemList.Count - 1);

		if (!final) {
			ShopItem currentItem = itemList [currentIndex - 1];
			ShopItem nextItem    = itemList [currentIndex];
			itemRefs.UpdateRefs (currentItem, nextItem);
		} 
		else {
			ShopItem currentItem = itemList [currentIndex];
			itemRefs.UpdateRefs (currentItem);
			itemRefs.DisableBuying ();
		}
	}
		
	// Purchasing
	/**
	 * Purchase the next level of the weapons.
	 */
	public void BuyWeapon() {
		bool purchased = BuyItem (DataSaver.Instance.CurrentWeapon, weapons, weaponRefs);
		if (purchased) {
			PlayerController.Instance.GetPlayerRefs.UpdateRefs (GetWeapon ().GetSprites ());
			PlayerController.Instance.GetAnimator.SetTrigger ("pickWeapon");
		}
	}

	/**
	 * Purchase the next level of the armours.
	 */
	public void BuyArmour () {
		bool purchased = BuyItem (DataSaver.Instance.CurrentArmour, armours, armourRefs);
		if (purchased) {
			PlayerController.Instance.GetPlayerRefs.UpdateRefs (GetArmour ().GetSprites ());
			PlayerController.Instance.GetAnimator.SetTrigger ("pickArmour");
			int index = GetArmour ().HasAttribute ("Hit Points");
			if (index == -1) {
				Debug.LogError ("Armour does not have hit points");
				return;
			}
			SessionData.Instance.Lives.Max = (int) GetArmour ().GetAttribute (index).Value;
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

