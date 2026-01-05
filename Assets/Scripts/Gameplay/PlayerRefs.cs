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
	[System.Serializable]
	public class FaceEmotion {
		[SerializeField] SpriteMesh rightEye;
		[SerializeField] SpriteMesh leftEye ;
		[SerializeField] SpriteMesh mouth   ;

		public SpriteMesh RightEye { get { return rightEye; } }
		public SpriteMesh LeftEye  { get { return leftEye ; } }
		public SpriteMesh Mouth    { get { return mouth   ; } }
	}

	public enum FaceEmotionType {
		Normal, Sad
	}

	[SerializeField] SpriteMeshInstance rightEye;
	[SerializeField] SpriteMeshInstance leftEye ;
	[SerializeField] SpriteMeshInstance mouth   ;

	[SerializeField] FaceEmotion normal;
	[SerializeField] FaceEmotion sad   ;
	[SerializeField] Transform weapon;
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

	// Load the weapon and upgrade the weapons range
	public void LoadWeapon(ShopItem item) {
		Vector2 scale = weapon.localScale;
		int index = item.HasAttribute ("Range");
		if (index == -1) {
			Debug.LogError ("Weapon missing range attribute");
			return;
		}
		scale.x = item.GetAttribute (index).Value;
		weapon.localScale = scale;
	}

	public void LoadEmotion(FaceEmotionType emotion) {
		switch (emotion) {
			case FaceEmotionType.Normal: {
				UpdateEmotion (normal);
				break;
			}
			case FaceEmotionType.Sad: {
				UpdateEmotion (sad);
				break;
			}
		}
	}
	void UpdateEmotion(FaceEmotion emotion) {
		rightEye.spriteMesh = emotion.RightEye;
		leftEye .spriteMesh = emotion.LeftEye ;
		mouth   .spriteMesh = emotion.Mouth   ;
	}
}
