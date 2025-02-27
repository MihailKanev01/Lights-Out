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

    [Header("Sanity System")] // 🟢 New Sanity Variables
    [SerializeField] private float maxSanity = 100f;
    [SerializeField] private float sanityDrainRate = 5f; 
    [SerializeField] private float sanityRegenRate = 10f; 
    private float currentSanity;

    [Header("Sound FX")]
    [SerializeField] private AudioClip turnOnSound;
    [SerializeField] private AudioClip turnOffSound;
    [SerializeField] private AudioClip lowBatterySound;

    private bool isFlashlightOn = false;
    private bool isLowBatteryWarningPlayed = false;

    private void Start()
    {
        currentBattery = maxBattery;
        currentSanity = maxSanity; 
        UpdateBatteryUI();
        UpdateSanityUI(); 

        isFlashlightOn = false;
        if (flashlightObject != null) flashlightObject.SetActive(false);
        if (flashlightLight != null) flashlightLight.enabled = false;
        if (flashlightRenderer != null) flashlightRenderer.enabled = false;
    }

    private void Update()
    {
        if (Input.GetButtonDown("F"))
        {
            ToggleFlashlight();
        }

        if (isFlashlightOn)
        {
            DrainBattery();
            RegenerateSanity(); 
        }
        else
        {
            DrainSanity();
        }
    }

    private void ToggleFlashlight()
    {
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
        UpdateBatteryUI();

        if (currentBattery <= 0)
        {
            currentBattery = 0;
            batteryImage.enabled = false;
            TurnOffFlashlight();
        }

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

    public void RechargeBattery(float amount)
    {
        currentBattery += amount;
        if (currentBattery > maxBattery)
        {
            currentBattery = maxBattery;
            batteryImage.enabled = true;
        }

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

    private void DrainSanity()
    {
        currentSanity -= sanityDrainRate * Time.deltaTime;
        if (currentSanity < 0)
        {
            currentSanity = 0;
            sanityImage.enabled = false;
        }

            UpdateSanityUI();
    }

    private void RegenerateSanity()
    {
        currentSanity += sanityRegenRate * Time.deltaTime;
        if (currentSanity > maxSanity)
        {
            currentSanity = maxSanity;
            sanityImage.enabled = true;
        }

        UpdateSanityUI();
    }

    public float GetSanity()
    {
        return currentSanity;
    }

    private void UpdateSanityUI()
    {
        if (sanitySlider != null)
        {
            sanitySlider.value = currentSanity / maxSanity;
        }
        if (sanityImage != null)
        {
            sanityImage.color = Color.red;
        }
    }
}
