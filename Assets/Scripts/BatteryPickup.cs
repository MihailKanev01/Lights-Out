using UnityEngine;

public class BatteryPickup : MonoBehaviour
{
    [SerializeField] private float rechargeAmount = 20f;
    [SerializeField] private GameObject pickUpText;
    [SerializeField] private AudioClip batteryPickupSound;

    private bool inReach = false;

    private void Start()
    {
        if (pickUpText != null)
            pickUpText.SetActive(false);
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

    private void Update()
    {
        if (inReach && Input.GetButtonDown("Interact"))
        {
            Flashlight flashlight = FindAnyObjectByType<Flashlight>();

            if (flashlight != null)
            {
                // Only allow battery pickup if player has the flashlight
                if (flashlight.HasFlashlight())
                {
                    flashlight.RechargeBattery(rechargeAmount);

                    if (batteryPickupSound != null)
                    {
                        SoundFXManager.Instance?.PlaySoundFXClip(batteryPickupSound, transform, 0.5f);
                    }

                    if (pickUpText != null)
                        pickUpText.SetActive(false);

                    Destroy(gameObject);
                }
                else
                {
                    Debug.Log("You need a flashlight first!");
                    // Optional: Show UI message "Find a flashlight first!"
                }
            }
        }
    }
}