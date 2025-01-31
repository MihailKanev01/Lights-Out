using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StarterAssets;

public class Keypad : MonoBehaviour
{
    public GameObject player;
    public GameObject keypadOB;
    public GameObject hud;
    public DoorWithKeyPad door; // Reference to the door script

    public Animator ANI;
    public Text textOB;
    public string answer = "12345"; // Correct PIN

    public AudioSource button;
    public AudioSource correct;
    public AudioSource wrong;

    public bool animate;

    void Start()
    {
        keypadOB.SetActive(false); // Ensure the keypad is initially hidden
    }

    public void Number(int number)
    {
        textOB.text += number.ToString();
        button.Play();
    }

    public void Execute()
    {
        if (textOB.text == answer)
        {
            correct.Play();
            textOB.text = "Right"; // Display "Right" when the correct PIN is entered

            // Unlock the door
            if (door != null)
            {
                door.UnlockDoor();
                Debug.Log("Door unlocked via keypad!");
            }

            if (animate)
            {
                ANI.SetBool("animate", true);
                Debug.Log("Animation triggered.");
            }

            // Hide the keypad after 3 seconds
            StartCoroutine(HideKeypadAfterDelay(1f)); // Call the coroutine to hide after 3 seconds
        }
        else
        {
            wrong.Play();
            textOB.text = "Wrong";
        }
    }

    public void Clear()
    {
        textOB.text = "";
        button.Play();
    }

    public void Exit()
    {
        keypadOB.SetActive(false);
        hud.SetActive(true);
        player.GetComponent<FirstPersonController>().enabled = true;
    }

    void Update()
    {
        if (keypadOB.activeInHierarchy)
        {
            hud.SetActive(false);
            player.GetComponent<FirstPersonController>().enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    // Coroutine to hide the keypad after a delay and hide the cursor
    private IEnumerator HideKeypadAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified time (3 seconds)
        keypadOB.SetActive(false); // Hide the keypad
        hud.SetActive(true); // Re-enable the HUD
        player.GetComponent<FirstPersonController>().enabled = true; // Enable player controls

        // Hide the cursor and lock it back to the center
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
