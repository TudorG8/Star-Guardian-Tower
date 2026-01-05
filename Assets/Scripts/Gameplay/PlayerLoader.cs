using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Used to load items purchased from the shop onto the player.
 * Is also used to load the initial items when the player starts the game.
 */
public class PlayerLoader : Singleton<PlayerLoader> {
	void Awake() {
		InitiateSingleton ();
	}

	void Start() {
		LoadInitialData ();
	}
		
	void LoadInitialData() {
		LoadCurrentWeapon ();
		LoadCurrentArmour (); 
	}

	public void LoadCurrentWeapon() {
		LoadItem (DataSaver.Instance.CurrentWeapon, Shop.Instance.Weapons, Shop.Instance.WeaponRefs);
		PlayerController.Instance.GetPlayerRefs.UpdateRefs (Shop.Instance.GetCurrentWeapon ().GetSprites ());
		PlayerController.Instance.GetPlayerRefs.LoadWeapon (Shop.Instance.GetCurrentWeapon ());
	}

	public void LoadCurrentArmour() {
		LoadItem (DataSaver.Instance.CurrentArmour, Shop.Instance.Armours, Shop.Instance.ArmourRefs);
		PlayerController.Instance.GetPlayerRefs.UpdateRefs (Shop.Instance.GetCurrentArmour ().GetSprites ());
		int index = Shop.Instance.GetCurrentArmour ().HasAttribute ("Hit Points");
		if (index == -1) {
			Debug.LogError ("Armour does not have hit points");
			return;
		}
		SessionData.Instance.Lives.Max = (int) Shop.Instance.GetCurrentArmour ().GetAttribute (index).Value;
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
}