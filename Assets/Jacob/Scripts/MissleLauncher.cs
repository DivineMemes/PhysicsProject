using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissleLauncher : MonoBehaviour {

    //Creates the instance of the missle launcher for the missle to use
    private static MissleLauncher instance = null;
    public static MissleLauncher SharedInstance { get; private set; }

    //The target the missle will get from the launcher
    [HideInInspector]
    public Transform target;
    [HideInInspector]
    public bool hasFired = false;

    int layerMask = 1 << 12;
    
    //The launchers Rigidbody
    private Rigidbody rb;
    //The force that will be used to aim the launcher forward
    private float gunForce = 4;
    private float rotateForce = 1000;

    //The cooldown between each rocket
    [SerializeField]
    private float rocketCooldown = 15;
    private float timer;

    [SerializeField]
    private float soundTimerStart;
    private float soundTimer = 0;

    private AudioSource locked;

    //Refrence to the Rocket prefab under Assets > Jacob > Prefabs
    public GameObject rocket;

    //Refrence to Rocket spawn point
    public Transform spawnPoint;
    //Refrence to the players camera in the player prefab under the CamRig object
    public Camera mainCam;

    //Refrence to the LineRenderer to create the Lazer sight to help with aiming
    private LineRenderer lr;

    private void Start()
    {
        locked = GetComponent<AudioSource>();
        hasFired = true;
    }

    private void Awake()
    {

        layerMask = ~layerMask;

        //Sets the instance equal to the object this script is attached to (will auto-delete
        // itself if one already exists)
        if (SharedInstance == null)
        {
            SharedInstance = this;
        }
        else
        {
            Debug.LogWarning("Duplicate MissleLauncher detected! Destroying extra copy...");
            Destroy(gameObject);
        }

        //Gets launchers Rigidbody
        rb = GetComponent<Rigidbody>();

        //Gets launchers LineRenderer
        lr = GetComponent<LineRenderer>();
        lr.enabled = false;

        timer = rocketCooldown;

        //Makes sure there is no way for hasFired to start true (it will crash the game if it 
        // starts true)
        if (hasFired == true)
        {
            hasFired = false;
        }
    }

    private void Update()
    {
        soundTimer -= Time.deltaTime;

        if (Input.GetKey(KeyCode.R))
        {
            //Activates Lazersight when "R" is held down
            lr.enabled = true;

            //Moves the gun forward
            rb.velocity = Vector3.forward * gunForce;

            //Shortcut for the launchers forward axis
            Vector3 fwd = transform.forward;

            //Creates and uses a raycast to search for any object marked with the tag "Enemy"
            RaycastHit hit;
            if (Physics.Raycast(transform.position, fwd * 1000, out hit, 10000, layerMask) && (hit.transform.tag == "Enemy" || hit.transform.tag == "Tentacle") && target == null)
            {
                //Sets the target equal to the object that met both requirements
                target = hit.transform;
                //Debug.Log("Is hitting");
            }

            if (soundTimer <= 0 && target != null)
            {
                locked.PlayDelayed(0);
                soundTimer = soundTimerStart;
            }
        }

        else if (Input.GetKeyUp(KeyCode.R))
        {
            //Disables the LineRenderer
            lr.enabled = false;
            //Resets target to null so next rocket won't Auto-fire at nothing
            target = null;
            locked.Stop();
        }

        if (hasFired)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                Instantiate(rocket, spawnPoint.position, spawnPoint.rotation, transform);
                hasFired = false;
                timer = rocketCooldown;
            }
        }

    }
}
