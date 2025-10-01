using UnityEngine;

public class FlashlightPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private GameObject pickUpText;
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private GameObject flashlightModel; // The 3D model on the ground

    private bool inReach = false;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (pickUpText != null)
            pickUpText.SetActive(false);
    }

    private void Update()
    {
        // Check if player presses interact button while in range
        if (inReach && Input.GetButtonDown("Interact"))
        {
            PickUpFlashlight();
        }
    }

    private void PickUpFlashlight()
    {
        // Find the player's Flashlight script
        Flashlight playerFlashlight = FindAnyObjectByType<Flashlight>();

        if (playerFlashlight != null)
        {
            // Give the flashlight to the player
            playerFlashlight.GiveFlashlight();

            // Play pickup sound
            if (pickupSound != null)
            {
                SoundFXManager.Instance?.PlaySoundFXClip(pickupSound, transform, 0.5f);
            }

            Debug.Log("Flashlight picked up!");

            // Hide the pickup text
            if (pickUpText != null)
                pickUpText.SetActive(false);

            // Destroy the pickup object
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("Could not find Flashlight script on player!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = true;
            if (pickUpText != null)
                pickUpText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = false;
            if (pickUpText != null)
                pickUpText.SetActive(false);
        }
    }
}