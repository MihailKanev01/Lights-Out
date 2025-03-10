using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorWithKeyPad : MonoBehaviour
{
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private GameObject openText;
    [SerializeField] private GameObject keypadUI;
    [SerializeField] private AudioSource doorSound;
    [SerializeField] private AudioSource lockedSound;

    private bool inReach = false;
    private bool isOpen = false;
    private bool locked = true;

    private void Start()
    {
        if (openText != null)
            openText.SetActive(false);

        if (keypadUI != null)
            keypadUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = true;
            if (openText != null)
                openText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = false;
            if (openText != null)
                openText.SetActive(false);

            if (keypadUI != null)
                keypadUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (inReach && Input.GetButtonDown("Interact"))
        {
            if (locked)
            {
                if (lockedSound != null)
                {
                    lockedSound.Play();
                    Debug.Log("Door is locked!");
                }
            }
            else
            {
                ToggleDoor();
            }
        }
    }

    public void UnlockDoor()
    {
        locked = false;
        Debug.Log("Door is unlocked!");
    }

    private void ToggleDoor()
    {
        isOpen = !isOpen;
        doorAnimator.SetBool("isOpen", isOpen);

        if (doorSound != null)
            doorSound.Play();

        Debug.Log(isOpen ? "Door Opens" : "Door Closes");
    }
}
