using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthAdd : MonoBehaviour
{
    public int healthToAdd = 10;
    public AudioSource audioSource;
    public AudioClip powerUpSound;

    Health playerHealth;
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            playerHealth = other.GetComponent<Health>();
            if (playerHealth.health < playerHealth.maxHealth)
            {
                audioSource.PlayOneShot(powerUpSound);
                playerHealth.health += healthToAdd;
                if (playerHealth.health > playerHealth.maxHealth)
                {
                    playerHealth.health = playerHealth.maxHealth;
                }
                Destroy(gameObject);
            }
        }
    }
}
