using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StarterAssets;

public class Keypad : MonoBehaviour
{
    public GameObject player;
    public GameObject keypadOB;
    public GameObject hud;
    public DoorWithKeyPad door; // Reference to the door script

    public Animator ANI;
    public Text textOB;
    public string answer = "12345";

    public AudioSource button;
    public AudioSource correct;
    public AudioSource wrong;

    public bool animate;

    void Start()
    {
        keypadOB.SetActive(false);
    }

    public void Number(int number)
    {
        textOB.text += number.ToString();
        button.Play();
    }

    public void Execute()
    {
        if (textOB.text == answer)
        {
            correct.Play();
            textOB.text = "Right";

            // Unlock the door
            if (door != null)
            {
                door.UnlockDoor();
                Debug.Log("Door unlocked via keypad!");
            }

            if (animate)
            {
                ANI.SetBool("animate", true);
                Debug.Log("Animation triggered.");
            }
        }
        else
        {
            wrong.Play();
            textOB.text = "Wrong";
        }
    }

    public void Clear()
    {
        textOB.text = "";
        button.Play();
    }

    public void Exit()
    {
        keypadOB.SetActive(false);
        hud.SetActive(true);
        player.GetComponent<FirstPersonController>().enabled = true;
    }

    void Update()
    {
        if (keypadOB.activeInHierarchy)
        {
            hud.SetActive(false);
            player.GetComponent<FirstPersonController>().enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
