using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : Singleton <Shop> {
	[SerializeField] List<ShopItem> weapons;
	[SerializeField] List<ShopItem> armours;

	[SerializeField] ShopItemRefs weaponRefs;
	[SerializeField] ShopItemRefs armourRefs;

	[SerializeField] ValueChanger valueChanger;
	[SerializeField] Text totalGold     ;
	[SerializeField] Text goldDifference;
	[SerializeField] Text highestScore  ;

	void Start() {
		LoadInitialData ();
	}

	void Update() {
		highestScore  .text = DataSaver.Instance.HighestScore.ToString();
		totalGold     .text = valueChanger.CurrentAmount.ToString ();
		if (valueChanger.Difference != 0) {
			char sign = valueChanger.Difference > 0 ? '+' : '-';
			goldDifference.text = sign + valueChanger.Difference;
		}
	}

	void LoadItem(SerializableInt index, List<ShopItem> itemList, ShopItemRefs itemRefs) {
		int currentIndex = index.Value < itemList.Count - 1? index.Value + 1 : itemList.Count - 1;
		bool final = (index.Value == itemList.Count);

		if (!final) {
			ShopItem currentItem = itemList [currentIndex    ];
			ShopItem nextItem    = itemList [currentIndex + 1];
			itemRefs.UpdateRefs (currentItem, nextItem);
		} 
		else {
			ShopItem currentItem = itemList [currentIndex];
			itemRefs.UpdateRefs (currentItem);
			itemRefs.DisableBuying ();
		}
	}
	void LoadInitialData() {
		LoadItem (DataSaver.Instance.CurrentWeapon, weapons, weaponRefs);
		LoadItem (DataSaver.Instance.CurrentArmour, armours, armourRefs);
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
				// Bought the last item
				if (index.Value == itemList.Count - 1) {
					itemRefs.DisableBuying ();
					itemRefs.UpdateRefs (item);
				} 
				else {
					ShopItem nextItem = itemList [index.Value + 1];
					itemRefs.UpdateRefs (item, nextItem);
				}
			}
		}
	}

	public bool PurchaseItem(ShopItem item) {
		if (DataSaver.Instance.TotalGold >= item.Cost) {
			valueChanger.GainAmount(DataSaver.Instance.TotalGold, -item.Cost);
			return true;
		}
		return false;
	}
}

