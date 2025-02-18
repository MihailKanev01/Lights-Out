using UnityEngine;
using UnityEngine.UI;

public class FlashlightPickup : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider batterySlider;
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject pickUpText;
    [SerializeField] private AudioClip batteryPickupSound;

    [Header("Flashlight Settings")]
    [SerializeField] private GameObject flashlightObject;
    [SerializeField] private Light flashlightLight;
    [SerializeField] private Renderer flashlightRenderer;
    [SerializeField] private float maxBattery = 100f;
    [SerializeField, Range(0f, 5f)] private float drainRate = 1f;
    [SerializeField] private float lowBatteryThreshold = 20f;
    [SerializeField] private float rechargeAmount = 20f;
    private float currentBattery;

    [Header("Sound FX")]
    [SerializeField] private AudioClip turnOnSound;
    [SerializeField] private AudioClip turnOffSound;
    [SerializeField] private AudioClip lowBatterySound;

    private bool isFlashlightOn = false;
    private bool isLowBatteryWarningPlayed = false;
    private bool inReach = false;

    private void Start()
    {
        currentBattery = maxBattery;
        UpdateBatteryUI();
        pickUpText?.SetActive(false);
        flashlightObject?.SetActive(true);
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

        if (Input.GetButtonDown("F"))
        {
            ToggleFlashlight();
        }

        if (isFlashlightOn)
        {
            DrainBattery();
        }
    }

    private void ToggleFlashlight()
    {
        if (currentBattery <= 0) return;

        isFlashlightOn = !isFlashlightOn;
        flashlightObject?.SetActive(isFlashlightOn);
        flashlightLight.enabled = isFlashlightOn;

        SoundFXManager.Instance?.PlaySoundFXClip(
            isFlashlightOn ? turnOnSound : turnOffSound, transform, 0.5f);
    }

    private void DrainBattery()
    {
        currentBattery -= drainRate * Time.deltaTime;
        UpdateBatteryUI();

        if (currentBattery <= 0)
        {
            currentBattery = 0;
            TurnOffFlashlight();
        }

        if (currentBattery <= lowBatteryThreshold && !isLowBatteryWarningPlayed)
        {
            SoundFXManager.Instance?.PlaySoundFXClip(lowBatterySound, transform, 0.5f);
            isLowBatteryWarningPlayed = true;
        }
        else if (currentBattery > lowBatteryThreshold)
        {
            isLowBatteryWarningPlayed = false;
        }
    }

    private void TurnOffFlashlight()
    {
        isFlashlightOn = false;
        flashlightObject?.SetActive(false);
        flashlightLight.enabled = false;
        SoundFXManager.Instance?.PlaySoundFXClip(turnOffSound, transform, 0.5f);
    }

    public void RechargeBattery(float amount)
    {
        currentBattery = Mathf.Clamp(currentBattery + amount, 0, maxBattery);
        UpdateBatteryUI();
    }

    private void UpdateBatteryUI()
    {
        if (batterySlider != null)
        {
            batterySlider.value = currentBattery / maxBattery;
        }
        UpdateSliderColor();
    }

    private void UpdateSliderColor()
    {
        if (fillImage != null)
        {
            fillImage.color = currentBattery > 50 ? Color.green :
            currentBattery > 20 ? Color.yellow :
            Color.red;
        }
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
}
