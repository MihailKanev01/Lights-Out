using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpKey : MonoBehaviour
{
    [SerializeField] private GameObject keyOB;
    [SerializeField] private GameObject invOB;
    [SerializeField] private GameObject pickUpText;
    [SerializeField] private AudioSource keySound;

    private bool inReach = false;

    private void Start()
    {
        pickUpText?.SetActive(false);
        invOB?.SetActive(false);
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
        if (inReach && Input.GetButtonDown("Interact"))
        {
            keyOB?.SetActive(false);
            keySound?.Play();
            invOB?.SetActive(true);
            pickUpText?.SetActive(false);
        }
    }
}
