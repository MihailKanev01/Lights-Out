using UnityEngine;
using UnityEngine.UI;

public class Flashlight : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider batterySlider;
    [SerializeField] private Image batteryImage;
    [SerializeField] private Image sanityImage;
    [SerializeField] private Slider sanitySlider;

    [Header("Flashlight Settings")]
    [SerializeField] private GameObject flashlightObject;
    [SerializeField] private Light flashlightLight;
    [SerializeField] private Renderer flashlightRenderer;
    [SerializeField] private float maxBattery = 100f;
    [SerializeField] private float drainRate = 1f;
    [SerializeField] private float lowBatteryThreshold = 20f;
    private float currentBattery;

    [Header("Sanity System")]
    [SerializeField] private float maxSanity = 100f;
    [SerializeField] private float sanityDrainRate = 5f; // Drains when flashlight is OFF
    [SerializeField] private float sanityRegenRate = 10f; // Regenerates when flashlight is ON
    [SerializeField] private float lowSanityThreshold = 20f;
    private float currentSanity;

    [Header("Sound FX")]
    [SerializeField] private AudioClip turnOnSound;
    [SerializeField] private AudioClip turnOffSound;
    [SerializeField] private AudioClip lowBatterySound;
    [SerializeField] private AudioClip lowSanitySound;

    [Header("Pickup System")]
    [SerializeField] private bool hasFlashlight = false; // Player starts WITHOUT flashlight
    [SerializeField] private GameObject flashlightUIIndicator; // Optional: UI icon showing player has flashlight

    private bool isFlashlightOn = false;
    private bool isLowBatteryWarningPlayed = false;
    private bool isLowSanityWarningPlayed = false;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        currentBattery = maxBattery;
        currentSanity = maxSanity;
        UpdateBatteryUI();
        UpdateSanityUI();

        // Start without flashlight
        isFlashlightOn = false;
        if (flashlightObject != null) flashlightObject.SetActive(false);
        if (flashlightLight != null) flashlightLight.enabled = false;
        if (flashlightRenderer != null) flashlightRenderer.enabled = false;

        // Update UI to show player doesn't have flashlight
        UpdateFlashlightUI();
    }

    private void Update()
    {
        // Only allow flashlight toggle if player has picked it up
        if (Input.GetButtonDown("F"))
        {
            if (hasFlashlight)
            {
                ToggleFlashlight();
            }
            else
            {
                Debug.Log("You don't have a flashlight!");
                // Optional: Play a "can't do that" sound or show UI message
            }
        }

        // Handle flashlight and sanity logic
        if (hasFlashlight && isFlashlightOn)
        {
            DrainBattery();
            RegenerateSanity();
        }
        else
        {
            DrainSanity(); // Always drain sanity when flashlight is off or not owned
        }

        // Check for low sanity warning
        if (currentSanity <= lowSanityThreshold && !isLowSanityWarningPlayed && lowSanitySound != null)
        {
            SoundFXManager.Instance?.PlaySoundFXClip(lowSanitySound, transform, 0.3f);
            isLowSanityWarningPlayed = true;
        }

        if (currentSanity > lowSanityThreshold)
        {
            isLowSanityWarningPlayed = false;
        }
    }

    private void ToggleFlashlight()
    {
        // Can't turn on if battery is dead
        if (!isFlashlightOn && currentBattery <= 0)
        {
            return;
        }

        isFlashlightOn = !isFlashlightOn;

        if (flashlightObject != null) flashlightObject.SetActive(isFlashlightOn);
        if (flashlightLight != null) flashlightLight.enabled = isFlashlightOn;
        if (flashlightRenderer != null) flashlightRenderer.enabled = isFlashlightOn;

        SoundFXManager.Instance?.PlaySoundFXClip(
            isFlashlightOn ? turnOnSound : turnOffSound, transform, 0.5f);
    }

    private void DrainBattery()
    {
        currentBattery -= drainRate * Time.deltaTime;

        if (currentBattery <= 0)
        {
            currentBattery = 0;
            TurnOffFlashlight();
        }

        UpdateBatteryUI();

        // Low battery warning
        if (currentBattery <= lowBatteryThreshold && !isLowBatteryWarningPlayed)
        {
            SoundFXManager.Instance?.PlaySoundFXClip(lowBatterySound, transform, 0.2f);
            isLowBatteryWarningPlayed = true;
        }

        if (currentBattery > lowBatteryThreshold)
        {
            isLowBatteryWarningPlayed = false;
        }
    }

    private void TurnOffFlashlight()
    {
        if (!isFlashlightOn) return;

        isFlashlightOn = false;

        if (flashlightObject != null) flashlightObject.SetActive(false);
        if (flashlightLight != null) flashlightLight.enabled = false;
        if (flashlightRenderer != null) flashlightRenderer.enabled = false;

        SoundFXManager.Instance?.PlaySoundFXClip(turnOffSound, transform, 0.5f);
    }

    private void DrainSanity()
    {
        currentSanity -= sanityDrainRate * Time.deltaTime;

        if (currentSanity < 0)
        {
            currentSanity = 0;
        }

        UpdateSanityUI();
    }

    private void RegenerateSanity()
    {
        currentSanity += sanityRegenRate * Time.deltaTime;

        if (currentSanity > maxSanity)
        {
            currentSanity = maxSanity;
        }

        UpdateSanityUI();
    }

    // Called by FlashlightPickup when player collects the flashlight
    public void GiveFlashlight()
    {
        hasFlashlight = true;
        Debug.Log("Flashlight acquired!");
        UpdateFlashlightUI();
    }

    public void RechargeBattery(float amount)
    {
        currentBattery += amount;
        if (currentBattery > maxBattery)
        {
            currentBattery = maxBattery;
        }

        UpdateBatteryUI();
    }

    public bool HasFlashlight()
    {
        return hasFlashlight;
    }

    public float GetSanity()
    {
        return currentSanity;
    }

    public float GetSanityPercentage()
    {
        return currentSanity / maxSanity;
    }

    private void UpdateBatteryUI()
    {
        if (batterySlider != null)
        {
            batterySlider.value = currentBattery / maxBattery;
        }
        UpdateBatteryColor();
    }

    private void UpdateBatteryColor()
    {
        if (batteryImage != null)
        {
            if (currentBattery > 50)
                batteryImage.color = Color.green;
            else if (currentBattery > 20)
                batteryImage.color = Color.yellow;
            else
                batteryImage.color = Color.red;
        }
    }

    private void UpdateSanityUI()
    {
        if (sanitySlider != null)
        {
            sanitySlider.value = currentSanity / maxSanity;
        }

        if (sanityImage != null)
        {
            if (currentSanity > 50)
                sanityImage.color = Color.cyan;
            else if (currentSanity > 20)
                sanityImage.color = Color.yellow;
            else
                sanityImage.color = Color.red;
        }
    }

    private void UpdateFlashlightUI()
    {
        // Optional: Show/hide UI indicator that player has flashlight
        if (flashlightUIIndicator != null)
        {
            flashlightUIIndicator.SetActive(hasFlashlight);
        }
    }
}