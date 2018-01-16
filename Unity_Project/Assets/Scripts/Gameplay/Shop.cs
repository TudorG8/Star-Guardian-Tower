using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : Singleton <Shop> {
	[SerializeField] List<ShopItem> weapons;
	[SerializeField] List<ShopItem> armours;

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
		BuyItem (DataSaver.Instance.CurrentWeapon, weapons);
	}

	public void BuyArmour () {
		BuyItem (DataSaver.Instance.CurrentArmour, armours);
	}

	public void BuyItem (SerializableInt index, List<ShopItem> itemList) {
		if (index.Value < itemList.Count - 1) {
			ShopItem item = itemList [index.Value];
			bool purchased = PurchaseItem (item);
			if (purchased) {
				index.Value++;
				if (index.Value == itemList.Count - 1) {
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

