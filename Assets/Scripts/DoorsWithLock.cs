using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorsWithLock : MonoBehaviour
{
    public Animator doorAnimator;
    public GameObject openText;
    public GameObject KeyINV;
    public AudioSource doorSound;
    public AudioSource lockedSound;

    private bool inReach = false;
    private bool isOpen = false;
    private bool locked = true;

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
        if (KeyINV.activeInHierarchy)
        {
            locked = true;
        }
        else
        {
            locked = false;
        }

        if (inReach && Input.GetButtonDown("Interact"))
        {
            if (locked)
            {
                lockedSound.Play();
                Debug.Log("Door is locked!");
            }
            else
            {
                ToggleDoor();
            }
        }
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
