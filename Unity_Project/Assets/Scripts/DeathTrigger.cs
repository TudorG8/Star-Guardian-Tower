using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    // --------------------------------------------------------------

    // Events
    public delegate void PlayerDeath(int playerNum);
    public static event PlayerDeath OnPlayerDeath;

    // --------------------------------------------------------------

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
            if(playerController)
            {
                // Kill the player
                //playerController.Die();

                // Increase the score for the opposing player
                if(OnPlayerDeath != null)
                {
                    //OnPlayerDeath(playerController.GetPlayerNum());
                }
            }
        }
    }
}
