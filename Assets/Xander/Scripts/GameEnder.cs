using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEnder : MonoBehaviour {

    [SerializeField, Tooltip("The length of time waited between player death and going to the main menu")]
    private float deathToMenuTime;
    [SerializeField]
    private string mainMenuScene;
    [SerializeField, Tooltip("If any of these reach 0 health, this script considers the game to have ended")]
    private PlayerHealth[] criticalParts;

    private bool goingToMenu = false;



    private void Update()
    {
        if (goingToMenu)
            return;

        for(int i = 0; i < criticalParts.Length; ++i)
        {
            if(criticalParts[i].health <= 0)
            {
                Invoke("GoToMenu", deathToMenuTime);
                goingToMenu = true;
                break;
            }
        }
    }

    private void GoToMenu()
    {
        SceneManager.LoadSceneAsync(mainMenuScene);
    }

}
