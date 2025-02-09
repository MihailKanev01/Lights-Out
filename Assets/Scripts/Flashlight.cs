using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [SerializeField] private GameObject flashlightObject;
    [SerializeField] private Light flashlightLight;
    [SerializeField] private AudioSource turnOn;
    [SerializeField] private AudioSource turnOff;

    private bool isOn = false;

    private void Start()
    {
        if (flashlightObject != null)
            flashlightObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            ToggleFlashlight();
    }

    private void ToggleFlashlight()
    {
        isOn = !isOn;

        if (flashlightObject != null)
            flashlightObject.SetActive(isOn);

        if (flashlightLight != null)
            flashlightLight.enabled = isOn;

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
