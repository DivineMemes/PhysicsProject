using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthScript : MonoBehaviour
{
    public float health;
    public float damagethresh;
    public float damagemult;


    private void Awake()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Wall") == false && collision.collider.CompareTag("Tentacle") == false)
        {
            float mag = collision.impulse.magnitude;

            if (mag >= damagethresh)
            {
                health -= mag * damagemult;
            }

            if (health <= 0)
            {
                var det = gameObject.GetComponent<DetatchChildren>();

                if (det)
                {
                    det.SetThemFree();
                }
                Destroy(gameObject);
            }
        }
    }
}
