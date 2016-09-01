using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PlayerManager
{
    public Color playerColor;
    public Material helmMat, shieldMat;

    [HideInInspector]
    public string coloredPlayerText;    // A string that represents the player with their number colored to match their character.
    public int playerNumber;
    [HideInInspector]
    public GameObject instance;         // A reference to the instance of the character when it is created.
    [HideInInspector]
    public int wins;                    // The number of wins this player has so far.

    private PlayerControllerBen playerController;
    private PlayerHealthAmmo playerHealthAmmo;

    public void Setup()
    {
        // Get references to the components.
        playerController = instance.GetComponent<PlayerControllerBen>();
        playerHealthAmmo = instance.GetComponent<PlayerHealthAmmo>();

        // Set the player numbers to be consistent across the scripts.
        playerController.playerNumber = playerNumber;

        // Create a string using the correct color that says 'PLAYER 1' etc based on the character's color and the player's number.
        coloredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(playerColor) + ">PLAYER" + "</color>";

        playerHealthAmmo.playerArrowUI.color = playerColor;
        
        // Get all of the renderers of the character.
        MeshRenderer[] renderers = instance.GetComponentsInChildren<MeshRenderer>();

        // Go through all the renderers...
        for (int i = 0; i < renderers.Length; i++)
        {
            // ... set the shield color to the default color...
            if (renderers[i].gameObject.name == "Shield")
            {
                renderers[i].material = shieldMat;
            }
            else if (renderers[i].gameObject.tag == "ThimbleHelm")
            {
                renderers[i].material.color = helmMat.color;
            }
            // ... set the eye color to black...
            else if (renderers[i].gameObject.tag == "Eye")
            {
                renderers[i].material.color = Color.black;
            }
            else
                // ... and set their material color to the color specific to this character.
                renderers[i].material.color = playerColor;
        }
    }
    
    // Used during the phases of the game where the player shouldn't be able to control their character.
    public void DisableControl()
    {
        playerController.enabled = false;
        playerController.GetComponent<AudioSource>().enabled = false;
    }

    // Used during the phases of the game where the player should be able to control their character.
    public void EnableControl()
    {
        playerController.enabled = true;
        playerController.GetComponent<AudioSource>().enabled = true;
    }

    // Used at the start of each round to put the players into it's default state.
    public void Reset()
    {
        instance.SetActive(false);
        instance.SetActive(true);

        // Set the power-ups to be non-active
        playerController.Shield.SetActive(false);
        playerController.Helm.SetActive(false);
    }
}
