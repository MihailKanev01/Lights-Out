using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    public Animator doorAnimator; // Renamed for clarity
    public GameObject openText;
    public AudioSource doorSound;

    private bool inReach = false;
    private bool isOpen = false; // Tracks door state

    void Start()
    {
        if (openText != null)
            openText.SetActive(false);
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
        }
    }

    void Update()
    {
        if (inReach && Input.GetButtonDown("Interact"))
        {
            ToggleDoor();
        }
    }

    void ToggleDoor()
    {
        isOpen = !isOpen; // Toggle the door state
        doorAnimator.SetBool("isOpen", isOpen); // Use a single bool parameter

        if (doorSound != null)
            doorSound.Play();

        Debug.Log(isOpen ? "Door Opens" : "Door Closes");
    }
}
