using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [SerializeField] private GameObject flashlight;
    [SerializeField] private AudioSource turnOn;
    [SerializeField] private AudioSource turnOff;

    private bool isOn = false;

    private void Start()
    {
        if (flashlight != null)
            flashlight.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetButtonDown("F"))
            ToggleFlashlight();
    }

    private void ToggleFlashlight()
    {
        isOn = !isOn;

        if (flashlight != null)
            flashlight.SetActive(isOn);

        if (isOn)
        {
            if (turnOn != null)
                turnOn.Play();
        }
        else
        {
            if (turnOff != null)
                turnOff.Play();
        }
    }
}
