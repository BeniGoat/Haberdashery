using UnityEngine;
using System.Collections;

public class PowerUpManager : MonoBehaviour
{
    PlayerHealthAmmo playerHealth;
    PlayerControllerBen playerControllerBen;
    public float healthBoost = 25f,
                 powerUpTimer = 15f;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (gameObject.name == "Heart(Clone)")
            {
                // Increase health of player
                playerHealth = other.GetComponentInParent<PlayerHealthAmmo>();
                playerHealth.PickUpHeart(healthBoost);
            }
            else if (gameObject.name == "ThimbleHelm(Clone)")
            {
                playerControllerBen = other.GetComponentInParent<PlayerControllerBen>();
                // Acivate helm on player
                playerControllerBen.helmActive = true;
                playerControllerBen.helmTimer = powerUpTimer;
            }
            else if (gameObject.name == "Shield(Clone)")
            {
                playerControllerBen = other.GetComponentInParent<PlayerControllerBen>();
                // Acivate shield on player
                playerControllerBen.shieldActive = true;
                playerControllerBen.shieldTimer = powerUpTimer;
            }

            Destroy(gameObject);
        }
    }
}
