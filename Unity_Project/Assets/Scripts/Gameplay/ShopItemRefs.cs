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
		[SerializeField] Transform parent;

		public void Update (ShopItem item) {
			name       .text   = item.Name        ;
			art        .sprite = item.ShopImage   ;
			description.text   = item.Description ;
		}

		public Transform Parent { get { return parent; } }
	}
	[SerializeField] ItemRefs current;
	[SerializeField] ItemRefs next   ;

	[SerializeField] Image  arrow   ;
	[SerializeField] Text   cost    ;
	[SerializeField] Button button  ;

	public void DisableBuying() {
		this.arrow .gameObject.SetActive (false);
		this.cost  .transform.parent.gameObject.SetActive (false);
		this.button.gameObject.SetActive (false);

		this.current.Parent.gameObject.SetActive (false);
	}

	public void UpdateRefs (ShopItem current, ShopItem next) {
		this.current.Update (current);
		this.next   .Update (next   );

		this.cost.text = next.Cost.ToString();
	}

	public void UpdateRefs (ShopItem current) {
		this.next.Update (current);
	}
}