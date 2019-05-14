using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetatchChildren : MonoBehaviour
{
    public ParticleSystem blood;
    public void SetThemFree()
    {
        ParticleSystem bloodparticle = Instantiate(blood.gameObject, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        transform.DetachChildren();
    }
}
