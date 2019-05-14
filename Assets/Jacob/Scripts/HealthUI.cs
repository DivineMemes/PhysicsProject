using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour {

    [SerializeField]
    private PlayerHealth partHealth;
    private float currentHealth;
    private float maxHealth;
    private float currentPercent;

    private bool isDead = false;

    private Color currentColor;

    private Image image;

	// Use this for initialization
	void Start () {
        maxHealth = partHealth.maxHealth;
        image = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {

        currentHealth = partHealth.health;

        currentPercent = currentHealth / maxHealth;

        image.color = Color.Lerp(Color.red, Color.white, currentPercent);

        if (currentHealth <= 0)
        {
            image.color = Color.black;
        }
    }
}
