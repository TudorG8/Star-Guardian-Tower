using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShopItem))]
public class ShopItemEditor : Editor {
	
	public override void OnInspectorGUI () {
		ShopItem script = (ShopItem)target;

		DrawDefaultInspector ();

		if (GUILayout.Button ("Set Armour Sprites")) {
			List<ShopItem.EquipableSprite> sprites = script.Sprites;
			sprites = new List<ShopItem.EquipableSprite> ();
			sprites.Add (new ShopItem.EquipableSprite ("Chest"));
			sprites.Add (new ShopItem.EquipableSprite ("Right Arm"));
			sprites.Add (new ShopItem.EquipableSprite ("Right Hand"));
			sprites.Add (new ShopItem.EquipableSprite ("Left Arm"));
			sprites.Add (new ShopItem.EquipableSprite ("Left Hand"));
			sprites.Add (new ShopItem.EquipableSprite ("Right Leg"));
			sprites.Add (new ShopItem.EquipableSprite ("Right Foot"));
			sprites.Add (new ShopItem.EquipableSprite ("Left Leg"));
			sprites.Add (new ShopItem.EquipableSprite ("Left Foot"));
			script.Sprites = sprites;
		}

		if (GUILayout.Button ("Set Weapon Sprites")) {
			List<ShopItem.EquipableSprite> sprites = script.Sprites;
			sprites = new List<ShopItem.EquipableSprite> ();
			sprites.Add (new ShopItem.EquipableSprite ("Weapon"));
			script.Sprites = sprites;
		}
	}
}
