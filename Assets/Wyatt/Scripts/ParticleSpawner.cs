using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{
    public Vector3 targetPos;
    public ParticleSystem launcher;
    public ParticleSystem.Particle[] m_particle;
    void Start()
    {
        targetPos = launcher.transform.position;
    }

    void Update()
    {
        InitializeIfNeeded();
        int particlesAlive = launcher.GetParticles(m_particle);
        for (int i = 0; i < particlesAlive; i++)
        {
        }
    }


    void InitializeIfNeeded()
    {
        if (launcher == null)
        {
            launcher = GetComponent<ParticleSystem>();
        }
        if (m_particle == null || m_particle.Length < launcher.main.maxParticles)
        {
            m_particle = new ParticleSystem.Particle[launcher.main.maxParticles];
        }
    }
}
