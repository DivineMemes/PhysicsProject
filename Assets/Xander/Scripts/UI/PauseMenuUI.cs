using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour {

    [SerializeField]
    private GameObject pauseMenu;
    [SerializeField]
    private string mainMenu;

    private bool paused = false;



    private void Start()
    {
        Resume();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown(KeyCode.Escape))
        {
            if(paused)
            {
                Resume();
            }
            else
            {
                pauseMenu.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0;
                paused = true;
            }
        }
    }

    public void GoToMenu()
    {
        SceneManager.LoadSceneAsync(mainMenu);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        paused = false;
        Time.timeScale = 1;
    }

}
