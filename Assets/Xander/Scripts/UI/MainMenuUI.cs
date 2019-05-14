using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour {

    [SerializeField, Tooltip("The name of the game scene")]
    private string gameScene;



    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void GoToGameScene()
    {
        SceneManager.LoadSceneAsync(gameScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
