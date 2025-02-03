using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject resume;
    [SerializeField] private GameObject quit;

    private bool isPaused = false;

    private void Start()
    {
        menu?.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetButtonDown("pause"))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
        menu?.SetActive(isPaused);
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1;
        menu?.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
