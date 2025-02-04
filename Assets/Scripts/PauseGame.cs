using UnityEngine;
using UnityEngine.UI; 

public class PauseGame : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private Text pauseText; 

    private bool isPaused = false;

    private void Start()
    {
        menu?.SetActive(false);
        pauseText?.gameObject.SetActive(false); 
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
        menu?.SetActive(isPaused);
        pauseText?.gameObject.SetActive(isPaused); 

        if (isPaused)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1;
        menu?.SetActive(false);
        pauseText?.gameObject.SetActive(false); 
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Exit()
    {
        Time.timeScale = 1; 
        Application.Quit();
    }
}
