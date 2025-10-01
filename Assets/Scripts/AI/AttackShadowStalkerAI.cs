using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AttackShadowStalkerAI : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackAnimationDuration = 3f;
    public float cameraShakeIntensity = 0.1f;
    public AudioClip attackSound;

    [Header("Eye Focus Settings")]
    public Transform headTransoform; // Reference to the eyes position
    public float cameraLockSpeed = 5f; // Speed of camera locking
    public bool enableHeadLock = true; // Toggle for eye locking

    private Camera playerCamera;
    private StarterAssets.FirstPersonController playerController;
    private bool attackActive = false;
    private Quaternion originalCameraRotation;

    void Start()
    {
        gameObject.SetActive(false);
        // Ensure eyes transform exists
        if (headTransoform == null)
        {
            // Try to find it by name
            Transform eyesChild = transform.Find("Eyes");
            if (eyesChild != null)
            {
                headTransoform = eyesChild;
            }
            else
            {
                // If no dedicated eyes transform, use the monster's head or upper position
                headTransoform = transform;
                Debug.LogWarning("No eyes transform found for camera locking. Using monster transform instead.");
            }
        }
    }

    // This method is called by ShadowStalkerAI when triggering the attack
    public void StartAttack(Camera camera, StarterAssets.FirstPersonController controller)
    {
        gameObject.SetActive (true);
        playerCamera = camera;
        playerController = controller;
        attackActive = true;

        // Store original camera rotation
        if (playerCamera != null)
        {
            originalCameraRotation = playerCamera.transform.rotation;
        }

        // Position to face the camera
        transform.LookAt(playerCamera.transform);

        // Play attack sound
        if (attackSound != null)
        {
            SoundFXManager.Instance?.PlaySoundFXClip(attackSound, transform, 1f);
        }

        // Add camera shake first
        StartCoroutine(CameraShake(0.5f, cameraShakeIntensity));

        // Start gameOver sequence after animation completes
        StartCoroutine(GameOverSequence());
    }

    void Update()
    {
        if (attackActive && playerCamera != null)
        {
            // Keep positioned in front of camera
            Vector3 idealPosition = playerCamera.transform.position + playerCamera.transform.forward * 1.5f;
            transform.position = Vector3.Lerp(
                transform.position,
                idealPosition,
                Time.deltaTime * 2f);

            // Always face the camera
            transform.LookAt(playerCamera.transform);

            // Lock camera onto monster's eyes
            if (enableHeadLock && headTransoform != null)
            {
                // Calculate direction from camera to eyes
                Vector3 directionToEyes = headTransoform.position - playerCamera.transform.position;

                // Create rotation to look at eyes
                Quaternion targetRotation = Quaternion.LookRotation(directionToEyes);

                // Smoothly rotate camera to look at eyes
                playerCamera.transform.rotation = Quaternion.Slerp(
                    playerCamera.transform.rotation,
                    targetRotation,
                    Time.deltaTime * cameraLockSpeed
                );
            }

            // Slight random movement for intensity (reduced when eye-locking)
            transform.Rotate(
                Random.Range(-0.5f, 0.5f),
                Random.Range(-1f, 1f),
                Random.Range(-0.5f, 0.5f)
            );
        }
    }

    IEnumerator CameraShake(float duration, float magnitude)
    {
        if (playerCamera == null) yield break;

        Vector3 originalPosition = playerCamera.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            playerCamera.transform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        playerCamera.transform.localPosition = originalPosition;

        // After shake, immediately begin eye locking
        if (enableHeadLock && headTransoform != null)
        {
            // Force immediate look at eyes after shake
            Vector3 directionToEyes = headTransoform.position - playerCamera.transform.position;
            playerCamera.transform.rotation = Quaternion.LookRotation(directionToEyes);
        }
    }

    IEnumerator GameOverSequence()
    {
        // Wait for attack animation to complete
        yield return new WaitForSeconds(attackAnimationDuration);

        // Trigger game over
        Debug.Log("GAME OVER");
        SceneManager.LoadScene("GameOverScene");
    }
}