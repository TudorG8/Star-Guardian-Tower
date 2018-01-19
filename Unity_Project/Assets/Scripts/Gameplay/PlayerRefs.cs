using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anima2D;
using System.Linq;

public class PlayerRefs : MonoBehaviour {
	[System.Serializable]
	public class SpriteRef {
		[SerializeField] string             name         ;
		[SerializeField] Transform          runtimeParent;
		[SerializeField] SpriteMeshInstance mesh         ;

		public string Name { get { return name; } }
		public SpriteMeshInstance Mesh { get { return mesh; } }
		public Transform RuntimeParent { get { return runtimeParent; } }
	}

	[SerializeField] List<SpriteRef> sprites;

	public void UpdateRefs(Dictionary<string, ShopItem.ObjectItems> items) {
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
