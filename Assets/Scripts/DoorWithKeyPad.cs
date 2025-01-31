using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorWithKeyPad : MonoBehaviour
{
    public Animator doorAnimator;
    public GameObject openText;
    public GameObject KeypadUI; // Reference to the keypad UI GameObject
    public AudioSource doorSound;
    public AudioSource lockedSound; // Sound for locked door

    private bool inReach = false;
    private bool isOpen = false;
    private bool locked = true; // Door starts locked by default

    void Start()
    {
        if (openText != null)
            openText.SetActive(false);

        if (KeypadUI != null)
            KeypadUI.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = true;
            if (openText != null)
                openText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = false;
            if (openText != null)
                openText.SetActive(false);

            if (KeypadUI != null)
                KeypadUI.SetActive(false); // Hide keypad UI when leaving
        }
    }

    void Update()
    {
        if (inReach && Input.GetButtonDown("Interact"))
        {
            if (locked)
            {
                // Play locked door sound and do not display keypad
                if (lockedSound != null)
                {
                    lockedSound.Play();
                    Debug.Log("Door is locked!");
                }
            }
            else
            {
                // Open or close the door if it's unlocked
                ToggleDoor();
            }
        }
    }

    public void UnlockDoor()
    {
        locked = false; // Unlock the door
        Debug.Log("Door is unlocked!");
    }

    void ToggleDoor()
    {
        isOpen = !isOpen;
        doorAnimator.SetBool("isOpen", isOpen);

        if (doorSound != null)
            doorSound.Play();

        Debug.Log(isOpen ? "Door Opens" : "Door Closes");
    }
}
