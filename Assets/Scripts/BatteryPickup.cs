using UnityEngine;

public class BatteryPickup : MonoBehaviour
{
    [SerializeField] private float rechargeAmount = 20f;
    [SerializeField] private GameObject pickUpText;
    [SerializeField] private AudioClip batteryPickupSound;

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
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = false;
            pickUpText?.SetActive(false);
        }
    }

    private void Update()
    {
        if (inReach && Input.GetButtonDown("Interact"))
        {
            Flashlight flashlight = FindAnyObjectByType<Flashlight>();
            if (flashlight != null)
            {
                flashlight.RechargeBattery(rechargeAmount);
            }

            if (batteryPickupSound != null)
            {
                SoundFXManager.Instance?.PlaySoundFXClip(batteryPickupSound, transform, 0.5f);
            }

            pickUpText?.SetActive(false);
            Destroy(gameObject);
        }
    }
}
