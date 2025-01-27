using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public GameObject flashlight;

    public AudioSource turnOn;
    public AudioSource turnOff;

    private bool isOn = false;




    void Start()
    {
        flashlight.SetActive(false);
    }




    void Update()
    {
        if (Input.GetButtonDown("F"))
            ToggleFlashLight();
    }

    void ToggleFlashLight()
    {
        isOn = !isOn;
        flashlight.SetActive(isOn);

        if (isOn && turnOn != null)
            turnOn.Play();
        else
            turnOff.Play();
    }
}
