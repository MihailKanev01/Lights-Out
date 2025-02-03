using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class ReadNotes : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject noteUI;
    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject inv;
    [SerializeField] private GameObject pickUpText;
    [SerializeField] private AudioSource pickUpSound;

    private bool inReach = false;

    private void Start()
    {
        noteUI?.SetActive(false);
        hud?.SetActive(true);
        inv?.SetActive(true);
        pickUpText?.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = true;
            pickUpText?.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = false;
            pickUpText?.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Interact") && inReach)
        {
            noteUI?.SetActive(true);
            pickUpSound?.Play();
            hud?.SetActive(false);
            inv?.SetActive(false);
            player.GetComponent<FirstPersonController>().enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void ExitButton()
    {
        noteUI?.SetActive(false);
        hud?.SetActive(true);
        inv?.SetActive(true);
        player.GetComponent<FirstPersonController>().enabled = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
