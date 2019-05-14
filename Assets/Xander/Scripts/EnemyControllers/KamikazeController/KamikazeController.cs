
//#define DRAW_KAMIKAZE_CONTROLLER_GIZMOS



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(AudioSource))]
public class KamikazeController : MonoBehaviour {

    [Header("Damage")]
    
    [SerializeField, Tooltip("The amount of HP the entity starts with")]
    private float maxHealth;
    [SerializeField, Tooltip("The minimum collision impulse to cause damage")]
    private float impulseDamageThreshold;
    [SerializeField, Tooltip("The minimum collision impulse to cause damage")]
    private float impulseDamageMultiplier;
    [SerializeField, Tooltip("The time in seconds to let the particle system play before being destroyed")]
    private float deathParticleLifetime;
    [SerializeField]
    private float explodeDist;

    [Header("Motion")]

    [SerializeField, Tooltip("The amount of force applied every second while accelerating towards target")]
    private float thrustForce;
    [SerializeField, Tooltip("The amount of force applied every second while rotating to face the target")]
    private float rotateForce;
    [SerializeField, Tooltip("The maximum angle between body facing & target direction for acceleration to start")]
    private float angleAccelThreshold;

    [Header("Collision Avoidance")]

    [SerializeField, Tooltip("Check to enable collision avoidance")]
    private bool doCollisionAvoidance;
    [SerializeField, Tooltip("The maximum time until collision in seconds (given current velocity) below which collision avoidance will be used")]
    private float collisionAvoidanceActivationTime;
    [SerializeField, Tooltip("The length of time in seconds between collision avoidance checks")]
    private float collisionAvoidanceCheckInterval;
    [SerializeField, Tooltip("The maximum angle between body facing & target direction for collision avoidance acceleration to start")]
    private float angleAccelThresholdCA;
    [SerializeField, Tooltip("The layer mask used while raycasting for collision avoidance")]
    private LayerMask collisionAvoidanceMask;

    [Header("Sound Effects")]

    [SerializeField, Tooltip("The basic sound that the kamikaze plays")]
    private AudioClip basicSound;
    [SerializeField, Tooltip("The aggressive sound the kamikaze plays")]
    private AudioClip aggressiveSound;
    [SerializeField, Tooltip("The sound the kamikaze plays when it dies")]
    private AudioClip deathSound;
    [SerializeField, Tooltip("The minimum time between playing sound effects")]
    private float minSoundWait;
    [SerializeField, Tooltip("The maximum time between playing sound effects")]
    private float maxSoundWait;
    [SerializeField, Tooltip("The percent chance for each sound effect to be the aggressive one"), Range(0f, 100f)]
    private float aggressiveSoundChance;
    [SerializeField, Tooltip("The volume to play the basic sound at")]
    private float basicSoundVolume;
    [SerializeField, Tooltip("The volume to play the aggressive sound at")]
    private float aggressiveSoundVolume;
    [SerializeField, Tooltip("The volume to play the death sound at")]
    private float deathSoundVolume;

    [Header("Evolution")]

    [SerializeField]
    private bool doEvolution;

    [Header("Links")]

    [SerializeField, Tooltip("This game object's rigidbody")]
    private Rigidbody body;
    [SerializeField, Tooltip("The particle system to play on death\nShould be active & not start on awake")]
    private ParticleSystem deathParticles;
    [SerializeField, Tooltip("The particle system's rigidbody\nA rigidbody is required to enable velocity inhreitance")]
    private Rigidbody deathParticleBody;
    [SerializeField, Tooltip("This game object's audio source")]
    private AudioSource audioSource;
    [SerializeField, Tooltip("The audio source which will play the death sound")]
    private AudioSource deathParticleAudioSource;
    [SerializeField]
    private ExplodeController explodeController;

    private float health;
    private Transform player;

    private List<Transform> connectedTransforms = new List<Transform>();

    private float lastCheckDist = 0;
    private Vector3 lastCheckPos = Vector3.zero;
    private Vector3 lastCheckDir = Vector3.zero;
    private bool lastCheckCorrectionFlag = false;
    private Vector3 lastCheckCorrectionTarget = Vector3.zero;

    private Vector3 lastTargetDir = Vector3.zero;
    private Vector3 lastVelDir = Vector3.zero;
    private Vector3 lastFinalDir = Vector3.zero;
    private Vector3 lastCrossAxis = Vector3.zero;
    private Vector3 lastFixedUpdatePos = Vector3.zero;

    private float damageDealtToPlayer;





    private void Start()
    {
        health = maxHealth;
        player = AIStaticReferences.Player;

        if (doCollisionAvoidance)
        {
            InvokeRepeating("CollisionAvoidanceCheck", collisionAvoidanceCheckInterval, collisionAvoidanceCheckInterval);
        }
        
        Invoke("PlaySoundEffect", Random.Range(minSoundWait, maxSoundWait));
        
        if(doEvolution)
        {
            angleAccelThreshold = KamikazeEvolver.Instance.SpawnAngleAccelThreshold;
            angleAccelThresholdCA = KamikazeEvolver.Instance.SpawnAngleAccelThresholdCA;
            collisionAvoidanceActivationTime = KamikazeEvolver.Instance.CollisionAvoidanceActivationTime;
        }
    }

    private void FixedUpdate()
    {
        Vector3 target;
        float act;

        if(lastCheckCorrectionFlag)
        {
            target = lastCheckCorrectionTarget;
            act = angleAccelThresholdCA;
        }
        else
        {
            target = player.position;
            act = angleAccelThreshold;

            Vector3 targDir = (target - transform.position).normalized;

            Vector3 velDir = body.velocity.normalized;
            Vector3 crossAxis = Vector3.Cross(targDir, velDir);

#if DRAW_KAMIKAZE_CONTROLLER_GIZMOS
            lastTargetDir = targDir;
            lastVelDir = velDir;
            lastCrossAxis = crossAxis;
            lastFixedUpdatePos = transform.position;
#endif

            targDir = Quaternion.AngleAxis(-(Vector3.Angle(targDir, velDir) / 3f), crossAxis) * targDir;
            target = transform.position + targDir;

#if DRAW_KAMIKAZE_CONTROLLER_GIZMOS
            lastFinalDir = targDir;
#endif
        }

        Vector3 targetDelta = target - transform.position;

        //get the angle between transform.forward and target delta
        float angleDiff = Vector3.Angle(transform.forward, targetDelta);


        if (angleDiff > act)
        {
            // get its cross product, which is the axis of rotation to
            // get from one vector to the other
            Vector3 cross = Vector3.Cross(transform.forward, targetDelta);

            // apply torque along that axis according to the magnitude of the angle.
            body.AddTorque(cross * angleDiff * rotateForce);
        }
        else
        {
            body.AddForce(transform.forward * thrustForce * Time.fixedDeltaTime);
        }

        if(Vector3.Distance(transform.position, player.position) <= explodeDist)
        {
            Kill();
        }
    }

    private void PlaySoundEffect()
    {
        if (Random.Range(0f, 99f) < aggressiveSoundChance)
            audioSource.PlayOneShot(aggressiveSound, aggressiveSoundVolume);
        else
            audioSource.PlayOneShot(basicSound, basicSoundVolume);

        Invoke("PlaySoundEffect", Random.Range(minSoundWait, maxSoundWait));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if(!connectedTransforms.Contains(collision.transform))
            {
                connectedTransforms.Add(collision.transform);
                CharacterJoint j = gameObject.AddComponent<CharacterJoint>();
                j.connectedBody = collision.rigidbody;
            }
        }
        else
        {
            float mag = collision.impulse.magnitude;

            if (mag >= impulseDamageThreshold)
            {
                health -= mag * impulseDamageMultiplier;

                if(collision.gameObject.CompareTag("Player"))
                    damageDealtToPlayer += mag * impulseDamageMultiplier;
                
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

        if(explodeController != null)
        {
            explodeController.gameObject.SetActive(true);
            explodeController.transform.parent = null;
            explodeController.Explode(AIStaticReferences.RandomPlayerPart.position);
        }

        if (doEvolution)
            KamikazeEvolver.Instance.AlterStatsByLifeScores(damageDealtToPlayer,
                                                            Vector3.Distance(player.position, transform.position),
                                                            angleAccelThreshold, angleAccelThresholdCA,
                                                            collisionAvoidanceActivationTime);

        Destroy(gameObject);
    }

    private void CollisionAvoidanceCheck()
    {
        RaycastHit hit;

        Vector3 velocity = body.velocity;
        Vector3 dir = velocity.normalized;
        float checkDist = (velocity * collisionAvoidanceActivationTime).magnitude;

        lastCheckDist = checkDist;
        lastCheckDir = dir;
        lastCheckPos = transform.position;

        if (Physics.Raycast(transform.position, (player.position - transform.position).normalized, out hit)
            && hit.transform.tag == "Player")
        {
            lastCheckCorrectionFlag = false;
        }
        if (Physics.Raycast(transform.position, body.velocity.normalized, out hit, checkDist, collisionAvoidanceMask))
        {
            lastCheckCorrectionFlag = true;
            lastCheckCorrectionTarget = lastCheckPos + (-lastCheckDir * checkDist);
        }
        else
        {
            lastCheckCorrectionFlag = false;
        }
    }

#if DRAW_KAMIKAZE_CONTROLLER_GIZMOS
    private void OnDrawGizmos()
    {
        if (lastCheckCorrectionFlag)
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.green;
        }
        Gizmos.DrawLine(lastCheckPos, lastCheckPos + lastCheckDir * lastCheckDist);

        float lineLength = 4f;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(lastFixedUpdatePos, lastFixedUpdatePos + lastTargetDir * lineLength);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(lastFixedUpdatePos, lastFixedUpdatePos + lastVelDir * lineLength);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(lastFixedUpdatePos, lastFixedUpdatePos + lastFinalDir * 3f);

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(lastFixedUpdatePos, lastFixedUpdatePos + lastCrossAxis * lineLength);
    }
#endif

}
