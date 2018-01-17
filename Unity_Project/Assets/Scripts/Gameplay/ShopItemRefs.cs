using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemRefs : MonoBehaviour {
	[SerializeField] Image  art   ;
	[SerializeField] Text   name  ;
	[SerializeField] Text   cost  ;
	[SerializeField] Button button;

	public Image Art  { get { return art ; } }
	public Text  Cost { get { return cost; } }

	public void UpdateRefs (Sprite art, string name, string cost, bool final) {
		this.art .sprite = art ;
		this.name.text   = name;
		if (final) {
			this.button.transform       .gameObject.SetActive (false);
			this.cost  .transform.parent.gameObject.SetActive (false);
		} 
		else {
			this.cost.text = cost;
		}
	}
}