using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemRefs : MonoBehaviour {
	[System.Serializable]
	public class ItemRefs {
		[SerializeField] Text  name       ;
		[SerializeField] Text  description;
		[SerializeField] Image art        ;

		public void Update (ShopItem item) {
			name       .text   = item.Name       ;
			art        .sprite = item.Image      ;
			description.text   = item.Description;
		}
	}
	[SerializeField] ItemRefs current;
	[SerializeField] ItemRefs next   ;

	[SerializeField] Text   finished;
	[SerializeField] Text   cost    ;
	[SerializeField] Button button  ;

	public void DisableBuying() {
		this.finished.gameObject.SetActive (true );
		this.cost    .gameObject.SetActive (false);
		this.button  .gameObject.SetActive (false);
	}

	public void UpdateRefs (ShopItem current, ShopItem next) {
		this.current.Update (current);
		this.next   .Update (next   );

		this.cost = next.Cost;
	}

	public void UpdateRefs (ShopItem current) {
		this.current.Update (current);
	}
}