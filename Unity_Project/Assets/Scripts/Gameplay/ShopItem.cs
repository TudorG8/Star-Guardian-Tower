using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItem : ScriptableObject {
	public enum ItemAttribute {
		Range, Lives
	}
	[SerializeField] int   cost ;
	[SerializeField] float range;
	[SerializeField] Dictionary<ItemAttribute, float> stats;

	public int Cost { get { return cost; } }

	public bool  HasAttribute(ItemAttribute attribute) {
		return stats.ContainsKey (attribute);
	}
	public float GetAttribute(ItemAttribute attribute) {
		return stats [attribute];
	}
}