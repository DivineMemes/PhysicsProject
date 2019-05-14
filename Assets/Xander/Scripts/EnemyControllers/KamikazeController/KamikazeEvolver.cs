using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamikazeEvolver : MonoBehaviour {

	public static KamikazeEvolver Instance { get; private set; }

    [SerializeField]
    private float angleAccelThreshold;
    [SerializeField]
    private float angleAccelThresholdMin;
    [SerializeField]
    private float angleAccelThresholdMax;
    [SerializeField]
    private float angleAccelThresholdMutRate;

    public float SpawnAngleAccelThreshold
    {
        get {
            return Mathf.Clamp(angleAccelThreshold + Random.Range(-angleAccelThresholdMutRate, angleAccelThresholdMutRate),
                               angleAccelThresholdMin, angleAccelThresholdMax);
        }
    }

    [SerializeField]
    private float angleAccelThresholdCA;
    [SerializeField]
    private float angleAccelThresholdCAMin;
    [SerializeField]
    private float angleAccelThresholdCAMax;
    [SerializeField]
    private float angleAccelThresholdCAMutRate;

    public float SpawnAngleAccelThresholdCA
    {
        get
        {
            return Mathf.Clamp(angleAccelThresholdCA + Random.Range(-angleAccelThresholdCAMutRate, angleAccelThresholdCAMutRate),
                               angleAccelThresholdCAMin, angleAccelThresholdCAMax);
        }
    }

    [SerializeField]
    private float collisionAvoidanceActivationTime;
    [SerializeField]
    private float collisionAvoidanceActivationTimeMin;
    [SerializeField]
    private float collisionAvoidanceActivationTimeMax;
    [SerializeField]
    private float collisionAvoidanceActivationTimeMutRate;

    public float CollisionAvoidanceActivationTime
    {
        get
        {
            return Mathf.Clamp(collisionAvoidanceActivationTime + Random.Range(-collisionAvoidanceActivationTimeMutRate, collisionAvoidanceActivationTimeMutRate),
                               collisionAvoidanceActivationTimeMin, collisionAvoidanceActivationTimeMax);
        }
    }

    private float mostDamageDealtToPlayer;



    private void OnValidate()
    {
        angleAccelThreshold = Mathf.Clamp(angleAccelThreshold, angleAccelThresholdMin, angleAccelThresholdMax);
        angleAccelThresholdCA = Mathf.Clamp(angleAccelThresholdCA, angleAccelThresholdCAMin, angleAccelThresholdCAMax);
        collisionAvoidanceActivationTime = Mathf.Clamp(collisionAvoidanceActivationTime, collisionAvoidanceActivationTimeMin, collisionAvoidanceActivationTimeMax);
    }

    private void Awake()
    {
        Instance = this;
    }

    public void AlterStatsByLifeScores(float damageDealtToPlayer, float distFromPlayerOnDeath, float angleAccelThreshold,
                                       float angleAccelThresholdCA, float collisionAvoidanceActivationTime)
    {
        if (damageDealtToPlayer > mostDamageDealtToPlayer)
            mostDamageDealtToPlayer = damageDealtToPlayer;

        float lerpVal = Mathf.InverseLerp(0f, mostDamageDealtToPlayer, damageDealtToPlayer);

        this.angleAccelThreshold = Mathf.Lerp(this.angleAccelThreshold, angleAccelThreshold, lerpVal);
        this.angleAccelThresholdCA = Mathf.Lerp(this.angleAccelThresholdCA, angleAccelThresholdCA, lerpVal);
        this.collisionAvoidanceActivationTime = Mathf.Lerp(this.collisionAvoidanceActivationTime, collisionAvoidanceActivationTime, lerpVal);
    }

}
