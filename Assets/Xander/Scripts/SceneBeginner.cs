using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneBeginner : MonoBehaviour {

    [SerializeField]
    private Transform ship;
    [SerializeField]
    private Transform shipStart;
    [SerializeField]
    private Transform shipEnd;
    [SerializeField]
    private float shipFallTime;
    [SerializeField]
    private Light directionalLight;
    [SerializeField]
    private float dirLightStartIntensity;
    [SerializeField]
    private float dirLightEndIntensity;
    [SerializeField]
    private WaveManager waveManager;
    [SerializeField]
    private MusicManager musicManager;
    [SerializeField]
    private GameObject[] tentacleObjects;
    [SerializeField]
    private float timeToTentacleAppearance;

    private bool shipFalling = true;
    private float shipFallTimer;
    private float shipFallRate;
    private float dirLightFadeRate;



    private void Awake()
    {
        waveManager.enabled = false;
        musicManager.enabled = false;

        for (int i = 0; i < tentacleObjects.Length; ++i)
            tentacleObjects[i].SetActive(false);

        Invoke("ActivateTentacles", timeToTentacleAppearance + shipFallTime);
    }

    private void Start()
    {
        ship.position = shipStart.position;
        shipFallRate = Vector3.Distance(shipStart.position, shipEnd.position) / shipFallTime;
        dirLightFadeRate = (dirLightEndIntensity - dirLightStartIntensity) / shipFallTime;

        directionalLight.intensity = dirLightStartIntensity;
    }

    private void Update()
    {
        if(shipFalling)
        {
            float dt = Time.deltaTime;
            shipFallTimer += dt;

            ship.position += (shipEnd.position - shipStart.position).normalized * shipFallRate * dt;
            directionalLight.intensity = directionalLight.intensity + dirLightFadeRate * dt;

            if (shipFallTimer >= shipFallTime)
            {
                shipFalling = false;
                waveManager.enabled = true;
                musicManager.enabled = true;
            }
        }
    }

    private void ActivateTentacles()
    {
        for (int i = 0; i < tentacleObjects.Length; ++i)
            tentacleObjects[i].SetActive(true);
    }

}
