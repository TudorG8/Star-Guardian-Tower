using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anima2D;
using System.Linq;

/**
 * Quick class to hold references to the sprites currently equiped on the player.
 * Besides just changing the mesh sprite, you can also add small static sprites on top of bones under their
 * "Runtime Sprites". These will get deleted when a new item is equiped in that place.
 */

public class PlayerRefs : MonoBehaviour {
	/**
	 * Quick class for sprite references to name, parent and spriteMeshInstance.
	 */
	[System.Serializable]
	public class SpriteRef {
		[SerializeField] string             name         ;
		[SerializeField] Transform          runtimeParent;
		[SerializeField] SpriteMeshInstance mesh         ;

		public string             Name          { get { return name         ; } }
		public SpriteMeshInstance Mesh          { get { return mesh         ; } }
		public Transform          RuntimeParent { get { return runtimeParent; } }
	}

	[SerializeField] List<SpriteRef> sprites;

	/**
	 * Remove the old static sprites and then change the mesh to the new given one.
	 * Will only update items given in the dictionary.
	 */
	public void UpdateRefs(Dictionary<string, ShopItem.EquipableSprite> items) {
		for (int i = 0; i < sprites.Count; i++) {
			SpriteRef spriteRef = sprites [i];
			if (items.ContainsKey (spriteRef.Name)) {
				spriteRef.Mesh.spriteMesh = items [spriteRef.Name].MeshToUse;
				// Destroy old stuff
				foreach (Transform child in spriteRef.RuntimeParent) { 
					Destroy (child.gameObject);
				}
				// Spawn the new stuff
				for(int j = 0; j < items [spriteRef.Name].AdditionalItems.Count; j++) {
					Transform obj = items [spriteRef.Name].AdditionalItems [j];
					GameObject newObj = Instantiate (obj.gameObject, obj.localPosition, obj.localRotation) as GameObject;
					newObj.transform.SetParent (spriteRef.RuntimeParent, false);
				}
			}
		}
	}
}
