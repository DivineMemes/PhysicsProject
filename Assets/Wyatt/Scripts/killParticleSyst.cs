using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class killParticleSyst : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(KillParticle());
    }

    IEnumerator KillParticle()
    {
        yield return new WaitForSeconds(2.00f);
        Destroy(gameObject);
    }
}
