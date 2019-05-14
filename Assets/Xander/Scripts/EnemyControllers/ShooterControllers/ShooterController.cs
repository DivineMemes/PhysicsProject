
//#define DRAW_SHOOTER_CONTROLLER_GIZMOS



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShooterController : MonoBehaviour {
    
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

    [SerializeField, Tooltip("The magnitude of impulse applied when wandering")]
    private float wanderImpulse;
    [SerializeField, Tooltip("The length of time in seconds between wander thrusts")]
    private float wanderInterval;
    [SerializeField, Tooltip("The length of time in seconds between a wander thrust and halting")]
    private float wanderLength;

    [Header("Collision Avoidance")]

    [SerializeField, Tooltip("Check to enable collision avoidance")]
    private bool doCollisionAvoidance;
    [SerializeField, Tooltip("The maximum time until collision in seconds (given current velocity) below which collision avoidance will be used")]
    private float collisionAvoidanceActivationTime;
    [SerializeField, Tooltip("The length of time in seconds between collision avoidance checks")]
    private float collisionAvoidanceCheckInterval;
    [SerializeField, Tooltip("The amount of force to apply when doing collision avoidance thrusts")]
    private float collisionAvoidanceCorrectionImpulse;
    [SerializeField, Tooltip("The layer mask used while raycasting for collision avoidance")]
    private LayerMask collisionAvoidanceMask;

    [Header("Shooting")]
    
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
    [SerializeField, Tooltip("The sound guns play when shooting")]
    private AudioClip shootSound;
    [SerializeField, Tooltip("The volume of the guns' shooting sound effect")]
    private float shootSoundVolume;

    [Header("Sound Effects")]

    [SerializeField, Tooltip("The sound the shooter plays when it dies")]
    private AudioClip deathSound;
    [SerializeField, Tooltip("The volume to play the death sound at")]
    private float deathSoundVolume;

    [Header("Links")]

    [SerializeField, Tooltip("This entity's rigidbody")]
    private Rigidbody body;
    [SerializeField, Tooltip("The particle system to play on death\nShould be active & not start on awake")]
    private ParticleSystem deathParticles;
    [SerializeField, Tooltip("The particle system's rigidbody\nA rigidbody is required to enable velocity inhreitance")]
    private Rigidbody deathParticleBody;
    [SerializeField, Tooltip("The audio source which will play the death sound")]
    private AudioSource deathParticleAudioSource;

    private float health;
    private Transform player;

    private Vector3 lastImpulse;

    private List<Transform> connectedTransforms = new List<Transform>();

    private float lastCheckDist = 0;
    private Vector3 lastCheckPos = Vector3.zero;
    private Vector3 lastCheckDir = Vector3.zero;
    private bool lastCheckDidCorrection = false;





    private void Start()
    {
        health = maxHealth;
        player = AIStaticReferences.Player;

        InvokeRepeating("Wander", wanderInterval, wanderInterval);
        InvokeRepeating("Halt", wanderInterval + wanderLength, wanderInterval);
        if(doCollisionAvoidance)
        {
            InvokeRepeating("CollisionAvoidanceCheck", collisionAvoidanceCheckInterval, collisionAvoidanceCheckInterval);
        }
    }

    private void Wander()
    {
        lastImpulse = Random.onUnitSphere * wanderImpulse;
        body.AddForce(lastImpulse, ForceMode.Impulse);
    }

    private void Halt()
    {
        body.AddForce(-lastImpulse, ForceMode.Impulse);
    }

    private void CollisionAvoidanceCheck()
    {
        RaycastHit hit;
        Vector3 velocity = body.velocity;
        Vector3 dir = velocity.normalized;
        float checkDist = (velocity * collisionAvoidanceActivationTime).magnitude;

        if(Physics.Raycast(transform.position, body.velocity.normalized, out hit, checkDist, collisionAvoidanceMask))
        {
            body.AddForce(-dir * collisionAvoidanceCorrectionImpulse, ForceMode.Impulse);

#if DRAW_SHOOTER_CONTROLLER_GIZMOS
            lastCheckDidCorrection = true;
#endif

        }
#if DRAW_SHOOTER_CONTROLLER_GIZMOS
        else
        {
            lastCheckDidCorrection = false;
        }

        lastCheckDist = checkDist;
        lastCheckDir = dir;
        lastCheckPos = transform.position;
#endif
    }

#if DRAW_SHOOTER_CONTROLLER_GIZMOS
    private void OnDrawGizmos()
    {
        if(lastCheckDidCorrection)
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.green;
        }
        Gizmos.DrawLine(lastCheckPos, lastCheckPos + lastCheckDir * lastCheckDist);
    }
#endif

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



    public float GetBulletImpulse() { return bulletImpulse; }
    public float GetBulletLifetime() { return bulletLifetime; }
    public float GetShootIntervalMin() { return shootIntervalMin; }
    public float GetShootIntervalMax() { return shootIntervalMax; }
    public float GetBulletTorque() { return bulletTorque; }
    public float GetShootSoundVolume() { return shootSoundVolume; }
    public AudioClip GetShootSound() { return shootSound; }

}
