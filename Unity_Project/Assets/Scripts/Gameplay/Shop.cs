using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : Singleton <Shop> {
	[SerializeField] List<ShopItem> weapons;
	[SerializeField] List<ShopItem> armours;

	[SerializeField] ShopItemRefs weaponRefs;
	[SerializeField] ShopItemRefs armourRefs;

	[SerializeField] Text totalGold   ;
	[SerializeField] Text highestScore;

	void Start() {
		LoadInitialData ();
	}

	void Update() {
		totalGold   .text = DataSaver.Instance.TotalGold   .ToString();
		highestScore.text = DataSaver.Instance.HighestScore.ToString();
	}

	void LoadInitialData() {
		int weaponToShow = DataSaver.Instance.CurrentWeapon.Value < weapons.Count - 1? DataSaver.Instance.CurrentWeapon.Value + 1 : weapons.Count - 1;
		bool final = DataSaver.Instance.CurrentWeapon.Value == weapons.Count;
		ShopItem weapon = weapons [weaponToShow];
		weaponRefs.UpdateRefs (weapon.Image, weapon.Name, weapon.Cost.ToString (), final);

		int armourToShow = DataSaver.Instance.CurrentArmour.Value < armours.Count - 1? DataSaver.Instance.CurrentArmour.Value + 1 : armours.Count - 1;
		final = DataSaver.Instance.CurrentArmour.Value == armours.Count;
		ShopItem armour = armours [armourToShow];
		armourRefs.UpdateRefs (armour.Image, armour.Name, armour.Cost.ToString (), final);
	}
		
	public float GetRange() {
		ShopItem item = weapons [DataSaver.Instance.CurrentWeapon.Value];
		if (item.HasAttribute (ShopItem.ItemAttribute.Range)) {
			return item.GetAttribute (ShopItem.ItemAttribute.Range);
		} 
		else {
			Debug.LogError ("Weapon has no range attribute");
			return 0;
		}
	}

	public ShopItem GetWeapon() {
		return weapons [DataSaver.Instance.CurrentWeapon.Value];
	}

	public ShopItem GetArmour() {
		return armours [DataSaver.Instance.CurrentArmour.Value];
	}


	public int GetLives() {
		ShopItem item = armours [DataSaver.Instance.CurrentArmour.Value];
		if (item.HasAttribute (ShopItem.ItemAttribute.Range)) {
			return (int)item.GetAttribute (ShopItem.ItemAttribute.Lives);
		} 
		else {
			Debug.LogError ("Armour has no lives attribute");
			return 0;
		}
	}

	public void BuyWeapon() {
		BuyItem (DataSaver.Instance.CurrentWeapon, weapons, weaponRefs);
	}

	public void BuyArmour () {
		BuyItem (DataSaver.Instance.CurrentArmour, armours, armourRefs);
	}

	public void BuyItem (SerializableInt index, List<ShopItem> itemList, ShopItemRefs itemRefs) {
		if (index.Value < itemList.Count - 1) {
			ShopItem item = itemList [index.Value + 1];
			bool purchased = PurchaseItem (item);
			if (purchased) {
				index.Value++;
				bool final = false;
				// Bought the last item
				if (index.Value == itemList.Count - 1) {
					Debug.Log ("Bought the last item");
					final = true;
					itemRefs.UpdateRefs (item.Image, item.Name, item.Cost.ToString (), final);
				} 
				else {
					Debug.Log ("Bought an item");
					ShopItem nextItem = itemList [index.Value + 1];
					itemRefs.UpdateRefs (nextItem.Image, nextItem.Name, nextItem.Cost.ToString (), final);
				}
			}
		}
	}

	public bool PurchaseItem(ShopItem item) {
		if (DataSaver.Instance.TotalGold >= item.Cost) {
			DataSaver.Instance.TotalGold -= item.Cost;
			return true;
		}
		return false;
	}
}

