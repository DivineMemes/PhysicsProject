using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A MonoBehavior which accelerates a collection of rigidbodies away from a point, and after a wait deletes them.
/// </summary>
public class ExplodeController : MonoBehaviour {

    [SerializeField, Tooltip("The magnitude of the impulse applied to the explosion parts")]
    private float explodeImpulse;
    [SerializeField, Tooltip("The radius of the simulated explosion")]
    private float explosionRadius;
    [SerializeField]
    private float startArea;
    [SerializeField]
    private float targetSpread;
    [SerializeField, Tooltip("The length of time between exploding and deletion")]
    private float destroyTime;
    [SerializeField, Tooltip("The rigidbody to inherit velocity from")]
    private Rigidbody parentBody;
    [SerializeField, Tooltip("The child bodies which are explosion parts")]
    private Rigidbody[] parts;



    public void Explode()
    {
        Vector3 myPos = transform.position;

        for(int i = 0; i < parts.Length; ++i)
        {
            //parts[i].velocity = parentBody.velocity;
            parts[i].position += Random.insideUnitSphere * startArea;
            parts[i].rotation = Random.rotation;
            parts[i].AddExplosionForce(explodeImpulse, myPos, explosionRadius, 0, ForceMode.Impulse);
        }

        Invoke("Remove", destroyTime);
    }

    public void Explode(Vector3 target)
    {
        Vector3 myPos = transform.position;

        for(int i = 0; i < parts.Length; ++i)
        {
            //parts[i].velocity = parentBody.velocity;
            parts[i].position += Random.insideUnitSphere * startArea;
            parts[i].transform.LookAt(target);
            parts[i].transform.eulerAngles += new Vector3(Random.Range(-targetSpread, targetSpread), Random.Range(-targetSpread, targetSpread), 0f);
            //parts[i].AddExplosionForce(explodeImpulse, myPos, explosionRadius, 0, ForceMode.Impulse);
            parts[i].AddForce(parts[i].transform.forward * explodeImpulse, ForceMode.Impulse);
        }

        Invoke("Remove", destroyTime);
    }

    private void Remove()
    {
        Destroy(gameObject);
    }
    
    public void Populate()
    {
        parts = GetComponentsInChildren<Rigidbody>();
    }

}
