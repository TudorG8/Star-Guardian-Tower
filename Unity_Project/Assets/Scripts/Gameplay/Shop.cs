using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : Singleton <Shop> {
	[SerializeField] List<ShopItem> weapons;
	[SerializeField] List<ShopItem> armours;

	public float GetRange() {
		ShopItem item = weapons [DataSaver.Instance.CurrentWeapon];
		if (item.HasAttribute (ShopItem.ItemAttribute.Range)) {
			return item.GetAttribute (ShopItem.ItemAttribute.Range);
		} 
		else {
			Debug.LogError ("Weapon has no range attribute");
			return null;
		}
	}

	public int GetLives() {
		ShopItem item = armours [DataSaver.Instance.CurrentArmour];
		if (item.HasAttribute (ShopItem.ItemAttribute.Range)) {
			return item.GetAttribute (ShopItem.ItemAttribute.Lives);
		} 
		else {
			Debug.LogError ("Armour has no lives attribute");
			return null;
		}
	}

	public void BuyWeapon() {
		BuyItem (DataSaver.Instance.CurrentWeapon, weapons);
	}

	public void BuyArmour () {
		BuyItem (DataSaver.Instance.CurrentArmour, armours);
	}

	public void BuyItem (SerializableInt index, List<ShopItem> itemList) {
		if (index < itemList.Count - 1) {
			ShopItem item = itemList [index];
			bool purchased = PurchaseItem (item);
			if (purchased) {
				index.Value++;
				if (index == itemList.Count - 1) {
					// ...
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

