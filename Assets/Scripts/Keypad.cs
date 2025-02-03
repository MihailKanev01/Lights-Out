using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StarterAssets;

public class Keypad : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject keypadOB;
    [SerializeField] private GameObject hud;
    [SerializeField] private DoorWithKeyPad door;
    [SerializeField] private Animator ANI;
    [SerializeField] private Text textOB;
    [SerializeField] private string answer = "12345";
    [SerializeField] private AudioSource button;
    [SerializeField] private AudioSource correct;
    [SerializeField] private AudioSource wrong;
    [SerializeField] private bool animate;

    private void Start()
    {
        if (keypadOB != null)
            keypadOB.SetActive(false);
    }

    public void Number(int number)
    {
        if (textOB != null)
            textOB.text += number.ToString();

        if (button != null)
            button.Play();
    }

    public void Execute()
    {
        if (textOB == null) return;

        if (textOB.text == answer)
        {
            if (correct != null)
                correct.Play();

            textOB.text = "Right";

            if (door != null)
            {
                door.UnlockDoor();
                Debug.Log("Door unlocked via keypad!");
            }

            if (animate && ANI != null)
            {
                ANI.SetBool("animate", true);
                Debug.Log("Animation triggered.");
            }

            StartCoroutine(HideKeypadAfterDelay(1f));
        }
        else
        {
            if (wrong != null)
                wrong.Play();

            textOB.text = "Wrong";
        }
    }

    public void Clear()
    {
        if (textOB != null)
            textOB.text = "";

        if (button != null)
            button.Play();
    }

    public void Exit()
    {
        if (keypadOB != null)
            keypadOB.SetActive(false);

        if (hud != null)
            hud.SetActive(true);

        if (player != null)
        {
            var controller = player.GetComponent<FirstPersonController>();
            if (controller != null)
                controller.enabled = true;
        }
    }

    private void Update()
    {
        if (keypadOB != null && keypadOB.activeInHierarchy)
        {
            if (hud != null)
                hud.SetActive(false);

            if (player != null)
            {
                var controller = player.GetComponent<FirstPersonController>();
                if (controller != null)
                    controller.enabled = false;
            }

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private IEnumerator HideKeypadAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (keypadOB != null)
            keypadOB.SetActive(false);

        if (hud != null)
            hud.SetActive(true);

        if (player != null)
        {
            var controller = player.GetComponent<FirstPersonController>();
            if (controller != null)
                controller.enabled = true;
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
