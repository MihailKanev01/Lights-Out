using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    [SerializeField] private GameObject onOB;
    [SerializeField] private GameObject offOB;
    [SerializeField] private GameObject lightsText;
    [SerializeField] private GameObject lightOB;
    [SerializeField] private AudioSource switchClick;

    private bool lightsAreOn = false;
    private bool lightsAreOff = true;
    private bool inReach = false;

    private void Start()
    {
        onOB?.SetActive(false);
        offOB?.SetActive(true);
        lightOB?.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = true;
            lightsText?.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = false;
            lightsText?.SetActive(false);
        }
    }

    private void Update()
    {
        if (inReach && Input.GetButtonDown("Interact"))
        {
            ToggleLights();
        }
    }

    private void ToggleLights()
    {
        lightsAreOn = !lightsAreOn;
        lightsAreOff = !lightsAreOn;

        lightOB?.SetActive(lightsAreOn);
        onOB?.SetActive(lightsAreOn);
        offOB?.SetActive(lightsAreOff);
        switchClick?.Play();
    }
}
