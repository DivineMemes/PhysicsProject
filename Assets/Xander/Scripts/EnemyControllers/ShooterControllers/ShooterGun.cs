using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(AudioSource))]
public class ShooterGun : MonoBehaviour {

    [Header("Damage")]

    [SerializeField, Tooltip("The amount of HP the entity starts with")]
    private float maxHealth;
    [SerializeField, Tooltip("The minimum collision impulse to cause damage")]
    private float impulseDamageThreshold;
    [SerializeField, Tooltip("The minimum collision impulse to cause damage")]
    private float impulseDamageMultiplier;
    [SerializeField, Tooltip("The time in seconds to let the particle system play before being destroyed")]
    private float deathParticleLifetime;
    
    [Header("Motion")]

    [SerializeField, Tooltip("The amount of force applied every second while rotating to face the target")]
    private float rotateForce;
    [SerializeField, Range(0f, 180f)]
    private float lockRotAngle = 10f;

    [Header("Shooting")]

    [SerializeField, Tooltip("If checked, uses core variable settings instead of below settings")]
    private bool useCoreShootingValues;
    [SerializeField, Tooltip("Core to use settings from if useCoreShootingValues is checked")]
    private ShooterController core;
    [SerializeField, Tooltip("The magnitude of the impulse applied to the shooting bullet")]
    private float bulletImpulse;
    [SerializeField, Tooltip("The length of time the bullet will exist for")]
    private float bulletLifetime;
    [SerializeField, Tooltip("The minimum length of time between shots in seconds")]
    private float shootIntervalMin;
    [SerializeField, Tooltip("The maximum length of time between shots in seconds")]
    private float shootIntervalMax;
    [SerializeField, Tooltip("The impulse applied to make the bullet rotate")]
    private float bulletTorque;

    [Header("Sound Effects")]

    [SerializeField, Tooltip("The sound the gun plays when it's destroyed")]
    private AudioClip deathSound;
    [SerializeField, Tooltip("The volume to play the death sound at")]
    private float deathSoundVolume;

    [Header("Links")]

    [SerializeField, Tooltip("This game object's rigidbody")]
    private Rigidbody body;
    [SerializeField, Tooltip("The particle system to play on death\nShould be active & not start on awake")]
    private ParticleSystem deathParticles;
    [SerializeField, Tooltip("The particle system's rigidbody\nA rigidbody is required to enable velocity inhreitance")]
    private Rigidbody deathParticleBody;
    [SerializeField, Tooltip("The bullet spawn point")]
    private Transform bulletSpawn;
    [SerializeField, Tooltip("The bullet prefab")]
    private GameObject bulletPrefab;
    [SerializeField, Tooltip("This game object's audio source for playing shooting sounds")]
    private AudioSource audioSource;
    [SerializeField, Tooltip("The audio source which will play the death sound")]
    private AudioSource deathParticleAudioSource;

    private float health;
    private Transform player;
    private float timer;

    private List<Transform> connectedTransforms = new List<Transform>();



    private void Start()
    {
        health = maxHealth;
        player = AIStaticReferences.Player;

        if(useCoreShootingValues)
        {
            bulletImpulse = core.GetBulletImpulse();
            bulletLifetime = core.GetBulletLifetime();
            shootIntervalMin = core.GetShootIntervalMin();
            shootIntervalMax = core.GetShootIntervalMax();
            bulletTorque = core.GetBulletTorque();
        }

        timer = Random.Range(shootIntervalMin, shootIntervalMax);
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if(timer <= Time.deltaTime)
        {
            timer = Random.Range(shootIntervalMin, shootIntervalMax);

            RaycastHit hit;
            if(Physics.Raycast(transform.position, (player.position - transform.position).normalized, out hit))
            //if(Physics.Raycast(transform.position, transform.up, out hit))
            {
                if(hit.transform.tag == "Player")
                {
                    GameObject bullet = Instantiate(bulletPrefab);
                    bullet.transform.position = bulletSpawn.position;
                    //bullet.transform.rotation = bulletSpawn.rotation;

                    float timeToPlayer = Vector3.Distance(bulletSpawn.position, player.position) / bulletImpulse;
                    Vector3 predictedPlayerPos = player.position + AIStaticReferences.PlayerRB.velocity * timeToPlayer;
                    bullet.transform.LookAt(predictedPlayerPos);

                    //bullet.transform.LookAt(player);
                    Rigidbody b = bullet.GetComponent<Rigidbody>();
                    b.AddForce(bullet.transform.forward * bulletImpulse, ForceMode.Impulse);
                    b.maxAngularVelocity = float.PositiveInfinity;
                    b.AddTorque(Vector3.one * bulletTorque, ForceMode.Impulse);
                    Destroy(bullet, bulletLifetime);

                    audioSource.PlayOneShot(core.GetShootSound(), core.GetShootSoundVolume());
                }
            }
        }
    }

    private void FixedUpdate()
    {
        Vector3 targetDelta = player.position - transform.position;

        //get the angle between transform.forward and target delta
        float angleDiff = Vector3.Angle(transform.up, targetDelta);

        //if (angleDiff <= lockRotAngle)
        //    transform.up = targetDelta.normalized;
        //else
        //{
            // get its cross product, which is the axis of rotation to
            // get from one vector to the other
            Vector3 cross = Vector3.Cross(transform.up, targetDelta);

            // apply torque along that axis according to the magnitude of the angle.
            body.AddTorque(cross * angleDiff * rotateForce);
        //}
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.GetComponent<KamikazeController>())
        {
            float mag = collision.impulse.magnitude;

            if (mag >= impulseDamageThreshold)
            {
                health -= mag * impulseDamageMultiplier;
                if (health <= 0)
                {
                    Kill();   
                }
            }
        }
    }

    public void Kill()
    {
        if (deathParticles != null)
        {
            deathParticles.Play();
            deathParticles.transform.parent = null;
            deathParticleBody.velocity = body.velocity;

            deathParticleAudioSource.PlayOneShot(deathSound, deathSoundVolume);

            Destroy(deathParticles.gameObject, deathParticleLifetime);
        }
        Destroy(gameObject);
    }

}
