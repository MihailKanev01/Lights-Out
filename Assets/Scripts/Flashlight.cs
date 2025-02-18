using UnityEngine;
using UnityEngine.UI;

public class Flashlight : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider batterySlider;
    [SerializeField] private Image fillImage;

    [Header("Flashlight Settings")]
    [SerializeField] private GameObject flashlightObject;
    [SerializeField] private Light flashlightLight;
    [SerializeField] private Renderer flashlightRenderer;
    [SerializeField] private float maxBattery = 100f;
    [SerializeField, Range(0f, 5f)] private float drainRate = 1f;
    [SerializeField] private float lowBatteryThreshold = 20f;
    private float currentBattery;

    [Header("Sound FX")]
    [SerializeField] private AudioClip turnOnSound;
    [SerializeField] private AudioClip turnOffSound;
    [SerializeField] private AudioClip lowBatterySound;

    private bool isFlashlightOn = false;
    private bool isLowBatteryWarningPlayed = false;

    private void Start()
    {
        currentBattery = maxBattery;
        Debug.Log($"Battery Initialized: {currentBattery}");
        UpdateBatteryUI();

        if (flashlightObject != null)
            flashlightObject.SetActive(false);
        if (flashlightRenderer != null)
            flashlightRenderer.enabled = false;
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
        }
    }

    private void ToggleFlashlight()
    {
        isFlashlightOn = !isFlashlightOn;

        if (flashlightObject != null)
        {
            flashlightObject.SetActive(isFlashlightOn);
            flashlightLight.enabled = isFlashlightOn;
        }

        if (flashlightRenderer != null)
        {
            flashlightRenderer.enabled = isFlashlightOn;
        }

        if (isFlashlightOn)
        {
            SoundFXManager.Instance?.PlaySoundFXClip(turnOnSound, transform, 0.5f);
        }
        else
        {
            SoundFXManager.Instance?.PlaySoundFXClip(turnOffSound, transform, 0.5f);
        }
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
        isFlashlightOn = false;

        if (flashlightObject != null)
        {
            flashlightObject.SetActive(false);
            flashlightLight.enabled = false;
        }

        if (flashlightRenderer != null)
        {
            flashlightRenderer.enabled = false;
        }

        SoundFXManager.Instance?.PlaySoundFXClip(turnOffSound, transform, 0.5f);
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

    private void UpdateBatteryUI()
    {
        if (batterySlider != null)
        {
            batterySlider.value = currentBattery / maxBattery;
            Debug.Log($"Battery: {currentBattery}, Slider Value: {batterySlider.value}");
        }
        UpdateSliderColor();
    }

    private void UpdateSliderColor()
    {
        if (fillImage != null)
        {
            if (currentBattery > 50)
                fillImage.color = Color.green;
            else if (currentBattery > 20)
                fillImage.color = Color.yellow;
            else
                fillImage.color = Color.red;
        }
    }
}
