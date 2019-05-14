using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurviveTimeUI : MonoBehaviour {

    [SerializeField]
    private TMPro.TextMeshProUGUI textMesh;
    [SerializeField]
    private PlayerHealth bodyHealth;
    [SerializeField]
    private PlayerHealth headHealth;

    private float timer;



    private void Update()
    {
        if (bodyHealth.health <= 0 || headHealth.health <= 0)
            return;

        timer += Time.deltaTime;

        int seconds = (int)timer;
        int minutes = seconds / 60;
        seconds %= 60;

        if(minutes > 0)
            textMesh.text = "Survived:\n" + minutes + ":" + seconds;
        else
            textMesh.text = "Survived:\n" + seconds;
    }

}
