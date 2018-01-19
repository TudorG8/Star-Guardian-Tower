using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Anima2D;

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

	public class AvailableItems {
		
	}

	[System.Serializable]
	public class ObjectItems {
		[SerializeField] string     itemName ;
		[SerializeField] SpriteMesh meshToUse;
		[SerializeField] List<Transform> additionalItems;

		public string ItemName { get { return itemName; } }
		public SpriteMesh MeshToUse { get { return meshToUse; } }
		public List<Transform> AdditionalItems { get { return additionalItems; } }
	}

	[SerializeField] string      itemName   ;
	[SerializeField] string      description;
	[SerializeField] int         cost       ;
	[SerializeField] Sprite      shopImage  ;
	[SerializeField] List<ObjectItems> items;


	[SerializeField] Dictionary<ItemAttribute, float> stats;

	public string     Name        { get { return itemName   ; } }
	public string     Description { get { return description; } }
	public int        Cost        { get { return cost       ; } }
	public Sprite     ShopImage   { get { return shopImage  ; } } 

	public int HasItem(string name) {
		for (int i = 0; i < items.Count; i++) {
			if (items [i].ItemName == name) {
				return i;
			}
		}
		return -1;
	}

	public SpriteMesh GetItemSpriteMesh (int index) {
		return items [index].MeshToUse;
	}

	public Dictionary<string, ObjectItems> GetSprites() {
		Dictionary<string, ObjectItems> dict = new Dictionary<string, ObjectItems> ();
		for (int i = 0; i < items.Count; i++) {
			dict [items [i].ItemName] = items [i];
		}
		return dict;
	}

	public List<Transform> GetItemAdditional (int index) {
		return items [index].AdditionalItems;
	}

	public bool  HasAttribute(ItemAttribute attribute) {
		return stats.ContainsKey (attribute);
	}
	public float GetAttribute(ItemAttribute attribute) {
		return stats [attribute];
	}
}