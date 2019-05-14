using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShooterLimb : MonoBehaviour {

    [Header("Damage")]

    [SerializeField, Tooltip("The amount of HP the entity starts with")]
    private float maxHealth;
    [SerializeField, Tooltip("The minimum collision impulse to cause damage")]
    private float impulseDamageThreshold;
    [SerializeField, Tooltip("The minimum collision impulse to cause damage")]
    private float impulseDamageMultiplier;
    [SerializeField, Tooltip("The time in seconds to let the particle system play before being destroyed")]
    private float deathParticleLifetime;

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
    [SerializeField, Tooltip("The gun at the end of this arm")]
    private ShooterGun armGun;
    [SerializeField, Tooltip("The audio source which will play the death sound")]
    private AudioSource deathParticleAudioSource;

    private float health;



    private void Start()
    {
        health = maxHealth;
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
        if (armGun)
            armGun.enabled = false;
        Destroy(gameObject);
    }

}
