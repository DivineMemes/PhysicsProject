using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

    public static Gun SharedInstance { get; private set; }

    public GameObject projectile;
    public GameObject sp;
    public Camera mainCam;
    private Rigidbody rb;
    private AudioSource gunShot;

    [SerializeField]
    private float timeBetweenShots = 0.3f;
    private float timer;

    public float bulletSpeed = 1000;

    public int strayFactor = 10;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        timer = timeBetweenShots;
        gunShot = GetComponent<AudioSource>();

        if (SharedInstance == null)
        {
            SharedInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (Input.GetKey(KeyCode.Mouse0) && timer <= 0)
        {
            int randomNumberX = Random.Range(-strayFactor, strayFactor);
            int randomNumberY = Random.Range(-strayFactor, strayFactor);
            int randomNumberZ = Random.Range(-strayFactor, strayFactor);

            GameObject bullet = Instantiate(projectile, sp.transform.position, transform.rotation) as GameObject;
            bullet.transform.Rotate(randomNumberX, randomNumberY, 0, Space.Self);
            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * bulletSpeed);

            //gunShot.Play(0);
            gunShot.PlayOneShot(gunShot.clip);

            timer = timeBetweenShots;
        }
    }
}
