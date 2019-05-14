using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {

    public float maxHealth;
    //private float health;
    public float health { get; private set; }

    [SerializeField]
    private float impulseDamageThreshold;

    [SerializeField]
    private float impulseDamageMultiplier;

    public Collider partCollider;

	// Use this for initialization
	void Start () {
        health = maxHealth;
        partCollider = GetComponent<Collider>();
	}

    private void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.tag == "Player" || c.gameObject.tag == "PlayerBullets")
        {
            Physics.IgnoreCollision(c.collider, partCollider);
        }

        else
        {
            float mag = c.impulse.magnitude;

            if(mag >= impulseDamageThreshold)
            {
                health -= mag * impulseDamageMultiplier;
                if (health <= 0)
                {
                    if (gameObject.name == "LeftHand")
                    {
                        gunDeath();
                    }
                    else if (gameObject.name == "RightHand")
                    {
                        missleDeath();
                    }
                    else if (gameObject.name == "Body" || gameObject.name == "Head")
                    {
                        bodyDeath();
                    }
                    else
                    {
                        Kill();
                    }
                }
            }
        }
    }

    void bodyDeath()
    {
        CharacterJoint[] childJoints = GetComponentsInChildren<CharacterJoint>();
        foreach (CharacterJoint comp in childJoints)
        {
            Destroy(comp);
            PlayerController.SharedInstance.enabled = false;
            gunDeath();
        }
    }

    void Kill()
    {
        Destroy(GetComponent<CharacterJoint>());
        PlayerController.SharedInstance.thrust -= 200;
        PlayerController.SharedInstance.straifThrust -= 200;
        PlayerController.SharedInstance.vThrust -= 200;
        enabled = false;
    }

    void gunDeath()
    {
        Gun.SharedInstance.enabled = false;
    }

    void missleDeath()
    {
        MissleLauncher.SharedInstance.enabled = false;
    }
}
