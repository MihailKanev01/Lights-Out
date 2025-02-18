using StarterAssets;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class JumpscareManager : MonoBehaviour
{
    public static JumpscareManager Instance { get; private set; }

    [Header("Jumpscare Settings")]
    [SerializeField] private GameObject jumpscareImage; // Assign a UI Image
    [SerializeField] private AudioClip jumpscareSound;  // Assign a scream sound
    [SerializeField] private float jumpscareDuration = 1.5f;

    [Header("Camera Effects")]
    [SerializeField] private bool enableCameraShake = true;
    [SerializeField] private bool enableBlurEffect = true;
    [SerializeField] private float shakeIntensity = 0.3f;
    [SerializeField] private float shakeDuration = 0.5f;

    private AudioSource audioSource;
    private FirstPersonController playerController; // Reference to Player Controller

    private void Awake()
    {
        if (Instance == null) Instance = this;
        audioSource = GetComponent<AudioSource>();
        playerController = FindFirstObjectByType<FirstPersonController>();
        jumpscareImage.SetActive(false);
    }

    public void TriggerJumpscare()
    {
        StartCoroutine(JumpscareSequence());
    }

    private IEnumerator JumpscareSequence()
    {
        // Disable player movement
        playerController.enabled = false;

        // Show Jumpscare Image & Play Sound
        jumpscareImage.SetActive(true);
        audioSource.PlayOneShot(jumpscareSound);

        // Camera Effects
        if (enableCameraShake) StartCoroutine(CameraShake());
        if (enableBlurEffect) StartCoroutine(BlurEffect());

        yield return new WaitForSeconds(jumpscareDuration);

        // Hide Jumpscare Image & Re-enable player movement
        jumpscareImage.SetActive(false);
        playerController.enabled = true;
    }

    private IEnumerator CameraShake()
    {
        Vector3 originalPos = Camera.main.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-shakeIntensity, shakeIntensity);
            float y = Random.Range(-shakeIntensity, shakeIntensity);
            Camera.main.transform.localPosition = originalPos + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Camera.main.transform.localPosition = originalPos;
    }

    private IEnumerator BlurEffect()
    {
        PostProcessingManager.Instance.EnableBlur(true);
        yield return new WaitForSeconds(jumpscareDuration);
        PostProcessingManager.Instance.EnableBlur(false);
    }

}
