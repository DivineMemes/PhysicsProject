using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissle : MonoBehaviour {

    private Transform target;
    private Transform sp;
    private GameObject ParticleEffect;
    Rigidbody rb;

    public float thrust = 200;
    public float lifeTime = 15;
    public float detonationThreshold = 3f;
    private bool hasFired = false;
    private bool hasPlayed = false;

    private AudioSource launchSound;
    private AudioSource thrusterSound;

    [SerializeField]
    private ExplodeController explodeController;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        sp = MissleLauncher.SharedInstance.spawnPoint;

        launchSound = GetComponentInParent<AudioSource>();
        thrusterSound = GetComponent<AudioSource>();
    }

    private void Awake()
    {
        ParticleEffect = transform.GetChild(0).gameObject;
        if (ParticleEffect.activeSelf == true)
        {
            ParticleEffect.SetActive(false);
        }
    }

    void Update()
    {
        if (!hasFired)
        {
            follow();
        }

        if (Input.GetKey(KeyCode.R))
        {
            target = MissleLauncher.SharedInstance.target;
        }

        if (Input.GetKeyUp(KeyCode.R) && target != null)
        {
            hasFired = true;
            transform.parent.SetParent(null, true);
        }

        if (hasFired == true)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if(hasPlayed == false)
            {
                launchSound.Play();
                thrusterSound.Play();
                hasPlayed = true;
            } 

            ParticleEffect.SetActive(true);
            lifeTime -= Time.deltaTime;

            if (lifeTime <= 0)
            {
                MissleLauncher.SharedInstance.hasFired = true;
                Destroy(ParticleEffect);
                Destroy(transform.parent.gameObject);
            }

            if(distance <= detonationThreshold)
            {
                MissleLauncher.SharedInstance.hasFired = true;
                explodeController.gameObject.SetActive(true);
                explodeController.transform.parent = null;
                explodeController.Explode(target.transform.position);
                Destroy(gameObject);
            }
            launch();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("COLLISION_ENTERED, hasFired: " + hasFired);
        if (hasFired == true)
        {
            MissleLauncher.SharedInstance.hasFired = true;
            explodeController.gameObject.SetActive(true);
            explodeController.transform.parent = null;
            explodeController.Explode(target.transform.position);
            Destroy(gameObject);
        }
    }

    void launch()
    {
        transform.LookAt(target);
        rb.AddForce(transform.forward * thrust);
    }

    void follow()
    {
        transform.position = sp.position;
        transform.rotation = sp.rotation;
    }
}
