using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Anima2D;

public class ShopItem : ScriptableObject {
	/**
	 * Menu Item to create a shop item.
	 */
	[MenuItem("Assets/Level Design/Create/Shop Item")]
	public static void CreateMyAsset() {
		ShopItem asset = ScriptableObject.CreateInstance<ShopItem>();

		AssetDatabase.CreateAsset(asset, "Assets/Prefabs/item.asset");
		AssetDatabase.SaveAssets();

		EditorUtility.FocusProjectWindow();

		Selection.activeObject = asset;
	}

	/**
	 * Quick class to hold the mesh and additional items related to a sprite piece.
	 */
	[System.Serializable]
	public class EquipableSprite {
		[SerializeField] string          itemName       ;
		[SerializeField] SpriteMesh      meshToUse      ;
		[SerializeField] List<Transform> additionalItems;

		public string          ItemName        { get { return itemName       ; } }
		public SpriteMesh      MeshToUse       { get { return meshToUse      ; } }
		public List<Transform> AdditionalItems { get { return additionalItems; } }
	}

	/**
	 * Quick class to hold an name-value attribute pair.
	 */
	public class Attribute {
		[SerializeField] string attributeName;
		[SerializeField] float  value        ;

		public string Name  { get { return attributeName; } }
		public float  Value { get { return value        ; } }
	}

	// Settings
	[SerializeField] string itemName   ;
	[SerializeField] string description;
	[SerializeField] int    cost       ;
	[SerializeField] Sprite shopImage  ;
	[SerializeField] List<EquipableSprite> sprites   ;
	[SerializeField] List<Attribute>       attributes;

	// Properties
	public string     Name        { get { return itemName   ; } }
	public string     Description { get { return description; } }
	public int        Cost        { get { return cost       ; } }
	public Sprite     ShopImage   { get { return shopImage  ; } } 

	/**
	 * Returns the index of a sprite if it is contained in the sprite list.
	 * Or -1 if it isn't.
	 */
	public int HasSprite (string name) {
		for (int i = 0; i < sprites.Count; i++) {
			if (sprites [i].ItemName == name) { return i; }
		}
		return -1;
	}

	public EquipableSprite GetSprite (int index) {
		return sprites [index];
	}

	/**
	 * Returns the index of an attribute if it is contained in the attribute list.
	 * Or -1 if it isn't.
	 */
	public int HasAttribute (string name) {
		for (int i = 0; i < attributes.Count; i++) {
			if (attributes [i].Name == name) { return i; }
		}
		return -1;
	}

	public Attribute GetAttribute (int index) {
		return attributes [index];
	}

	/**
	 * Returns a dictionary of all the equipable sprite objects.
	 */
	public Dictionary<string, EquipableSprite> GetSprites() {
		Dictionary<string, EquipableSprite> dict = new Dictionary<string, EquipableSprite> ();
		for (int i = 0; i < sprites.Count; i++) {
			dict [sprites [i].ItemName] = sprites [i];
		}
		return dict;
	}

	/**
	 * Returns a dictionary of all the attributes.
	 */
	public Dictionary<string, Attribute> GetAttributes() {
		Dictionary<string, Attribute> dict = new Dictionary<string, Attribute> ();
		for (int i = 0; i < attributes.Count; i++) {
			dict [attributes [i].Name] = attributes [i];
		}
		return dict;
	}
}