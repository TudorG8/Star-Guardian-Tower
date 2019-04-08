using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stuff I modified from the original code base -----------------
/*
	 * It is wastefull to use GetComponent on stuff that can just be drag and
	 * dropped from before the game starts. It is an expensive operation that
	 * can be skipped. Only time this can't be done is on runtime objects.
	 void Awake() {
		m_CharacterController = GetComponent<CharacterController>(); 
	 }
	 
     * The game I am making is single player, so no need for this.
     public int GetPlayerNum() {
     	if(m_PlayerInputString == "_P1") {
     		return 1;
     	}
     	else if (m_PlayerInputString == "_P2") {
     		return 2;
     	}
     	return 0;
     }
	 * The character won't rotate so no need for this either.
     void RotateCharacter(Vector3 movementDirection) {
        Quaternion lookRotation = Quaternion.LookRotation(movementDirection);
        if (transform.rotation != lookRotation)
        {
            transform.rotation = lookRotation;
        }
    }
    * The Unity Character controller is good for unrealistic FPS and such,
    * but we want a more accurate controller using raycasting.
    CharacterController m_CharacterController;

	Vector2 movementDirection = Vector2.zero; // The current movement direction in x & z.

    public void Die() {
        isAlive = false;
		respawnTime = maxRespawnTime;
    }

    void UpdateRespawnTime() {
		respawnTime -= Time.deltaTime;
		if (respawnTime < 0.0f) {
            Respawn();
        }
    }

    void Respawn() {
		isAlive = true;
        transform.position = spawningPosition;
        transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
    }

	[SerializeField] private float maxRespawnTime =  1.0f; // The time it takes to respawn

    void Start() {
        spawningPosition = transform.position;
		respawnTime = maxRespawnTime;
    }

	bool isAlive = true; // Whether the player is alive or not 
	float respawnTime; // The time it takes to respawn
Vector2 spawningPosition = Vector2.zero;
Vector2 currentMovementOffset = Vector2.zero; 
	public float horizontalSpeed = 0.0f; // The current movement speed
	public float verticalSpeed   = 0.0f; // The current vertical / falling speed

	[SerializeField] private string playerInputString = "_P1"; // Identifier for Input
	*/