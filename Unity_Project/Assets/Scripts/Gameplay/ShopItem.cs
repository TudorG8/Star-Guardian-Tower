using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ShopItem : ScriptableObject {
	[MenuItem("Assets/Create/Shop Item")]
	public static void CreateMyAsset() {
		ShopItem asset = ScriptableObject.CreateInstance<ShopItem>();

		AssetDatabase.CreateAsset(asset, "Assets/Prefabs/item.asset");
		AssetDatabase.SaveAssets();

		EditorUtility.FocusProjectWindow();

		Selection.activeObject = asset;
	}

	public enum ItemAttribute {
		Range, Lives
	}

	[SerializeField] string itemName   ;
	[SerializeField] string description;
	[SerializeField] int    cost       ;
	[SerializeField] Sprite image      ;
	[SerializeField] Dictionary<ItemAttribute, float> stats;

	public string Name        { get { return itemName   ; } }
	public string Description { get { return description; } }
	public int    Cost        { get { return cost       ; } }
	public Sprite Image       { get { return image      ; } } 

	public bool  HasAttribute(ItemAttribute attribute) {
		return stats.ContainsKey (attribute);
	}
	public float GetAttribute(ItemAttribute attribute) {
		return stats [attribute];
	}
}