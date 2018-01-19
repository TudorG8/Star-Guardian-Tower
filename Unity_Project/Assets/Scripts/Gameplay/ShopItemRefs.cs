using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Quick class to hold references to an upgrade in the shop.
 * Hold references to the current level of an upgrade and the next level of it.
 * Also has a reference to the cost, arrow and button.
 * 
 */
public class ShopItemRefs : MonoBehaviour {
	/**
	 * Quick class to hold references to name, description, art and parent of an upgrade
	 */
	[System.Serializable]
	public class ItemRefs {
		[SerializeField] Text      name       ;
		[SerializeField] Text      description;
		[SerializeField] Image     art        ;
		[SerializeField] Transform parent     ; // Holds everything

		public void Update (ShopItem item) {
			name       .text   = item.Name        ;
			art        .sprite = item.ShopImage   ;
			description.text   = item.Description ;
		}

		public Transform Parent { get { return parent; } }
	}
	[SerializeField] ItemRefs current;
	[SerializeField] ItemRefs next   ;
	[SerializeField] Image    arrow  ;
	[SerializeField] Text     cost   ;
	[SerializeField] Button   button ;

	/**
	 * Disable the additional UI stuff from a shop upgrade.
	 * Should be called when the final item is bought.
	 */
	public void DisableBuying() {
		this.arrow                  .gameObject.SetActive (false);
		this.cost  .transform.parent.gameObject.SetActive (false);
		this.button                 .gameObject.SetActive (false);

		this.current.Parent.gameObject.SetActive (false);
	}

	/**
	 * Update the name, sprite of the current item and next item.
	 * Also the cost to upgrade to the next level.
	 * Should only be called when there are still upgrades to get.
	 */
	public void UpdateRefs (ShopItem current, ShopItem next) {
		this.current.Update (current);
		this.next   .Update (next   );

		this.cost.text = next.Cost.ToString();
	}

	/**
	 * Similar to the above version, but should be called when the final item is bought.
	 */
	public void UpdateRefs (ShopItem current) {
		this.next.Update (current);
	}
}