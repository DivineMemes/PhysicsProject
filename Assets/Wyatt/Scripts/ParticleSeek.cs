using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSeek : MonoBehaviour
{
    public Transform target;
    public float force = 10.0f;
    TentacleAttack2 detect;

    ParticleSystem ps;

    void Awake()
    {
        detect = gameObject.GetComponent<TentacleAttack2>();
        ps = GetComponent<ParticleSystem>();
    }

    void LateUpdate()
    {
        if (detect.MyTarget != null)
        {
            target = detect.MyTarget.transform;
        }

        if (detect.MyTarget == null)
        {
            target = null;
        }
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.particleCount];
        ps.GetParticles(particles);
        if (target != null && detect.MyTurnToAttack == true)
        {


            for (int i = 0; i < particles.Length; i++)
            {
                ParticleSystem.Particle p = particles[i];
                Vector3 particleWorldPosition;
                if (ps.main.simulationSpace == ParticleSystemSimulationSpace.Local)
                {
                    particleWorldPosition = transform.TransformPoint(p.position);
                }
                else if (ps.main.simulationSpace == ParticleSystemSimulationSpace.Custom)

                {
                    particleWorldPosition = ps.main.customSimulationSpace.TransformPoint(p.position);
                }
                else
                {
                    particleWorldPosition = p.position;
                }

                Vector3 directionToTarget = (target.position - p.position).normalized;
                Vector3 seekForce = (directionToTarget * force) * Time.deltaTime;
                p.velocity += seekForce;
                particles[i] = p;
            }
            ps.SetParticles(particles, particles.Length);
        }
    }

}
