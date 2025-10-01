using UnityEngine;

public class FlashlightPickup : MonoBehaviour
{
    [SerializeField] private GameObject pickUpText;
    [SerializeField] private AudioClip flashlightPickupSound;

    private bool inReach = false;

    private void Start()
    {
        pickUpText?.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = true;
            pickUpText?.SetActive(true);
            Debug.Log("Player can pick up flashlight - inReach set to true");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = false;
            pickUpText?.SetActive(false);
            Debug.Log("Player left flashlight range - inReach set to false");
        }
    }

    private void Update()
    {
        if (inReach && Input.GetButtonDown("Interact"))
        {
            Debug.Log("Interact button pressed while in reach of flashlight");

            // Find the player's Flashlight component
            Flashlight playerFlashlight = FindAnyObjectByType<Flashlight>();

            if (playerFlashlight != null)
            {
                // Enable the player's flashlight component
                playerFlashlight.EnableFlashlight();

                // Play pickup sound
                if (flashlightPickupSound != null)
                {
                    SoundFXManager.Instance?.PlaySoundFXClip(flashlightPickupSound, transform, 0.5f);
                }

                // Remove from the world
                pickUpText?.SetActive(false);
                Destroy(gameObject);
            }
            else
            {
                Debug.LogError("No Flashlight component found on player! Make sure player has Flashlight script attached.");
            }
        }
    }
}