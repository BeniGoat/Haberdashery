using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealthAmmo : MonoBehaviour
{
    [HideInInspector]
    public float startingHealth = 100f;
    public int startingAmmo = 3;
    private float currentHealth;
    public int currentAmmo;
    [HideInInspector]
    public bool isDead;
    private PlayerManager playerManager;

    public Image healthFill, ammoFill, playerArrowUI;   
    public Slider healthSlider, ammoSlider;
    public Color fullHealthColour = Color.green, zeroHealthColor = Color.red, ammoColor = Color.yellow;

    private void OnEnable()
    {
        // And reset the player's health and whether or not it's dead
        currentHealth = startingHealth;
        currentAmmo = startingAmmo;
        isDead = false;
    }

    void Update()
    {
        SetUI();
    }
    
    public void TakeDamage(float amount)
    {
        // Reduce current health by the amount of damage done
        currentHealth -= amount;

        // If the current health is at or below zero and it has not yet been registered, change death flag
        if (currentHealth <= 0f && !isDead)
        {
            isDead = true;

            // Turn the player off
            gameObject.SetActive(false);
        }
    }

    public void PickUpHeart(float amount)
    {
        // If the current health is at above max health and it has not yet been registered
        if (currentHealth + amount > startingHealth)
            currentHealth = startingHealth;
        else
            // Increase current health by the amount
            currentHealth += amount;
    }

    private void SetUI()
    {
        // Set the slider's value appropriately.
        healthSlider.value = currentHealth;

        ammoSlider.value = currentAmmo;

        // Interpolate the color of the bar between the choosen colours based on the current percentage of the starting health.
        healthFill.color = Color.Lerp(zeroHealthColor, fullHealthColour, currentHealth / startingHealth);
        ammoFill.color = ammoColor;
    }
}
