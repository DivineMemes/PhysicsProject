using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleEyeDetection : MonoBehaviour
{
    public GameObject target;
    public GameObject Player;
    public bool locked = false;
    bool targetPlayer;
    bool Started;

    [Header("PanSetting")]
    Quaternion randomRotation;
    Vector3 randomAng;
    Quaternion lastRot;
    public float speed;
    public float TimeToTargetMin;
    public float TimeToTargetMax;
    float TimeToTarget;
    float timetoWait;
    bool RecalcRot = false;


    [Header("Tentacle info")]
    GameObject[] tentacles;
    GameObject tipToAttack;
    Transform currentTip;
    private Transform closestTip = null;
    float dist;

    [Header("DepthTrackingVariables")]
    [Tooltip("Increases the exclusion radius")]
    public float radius;
    [Tooltip("Increases the detection distance")]
    public float depth;
    [Tooltip("Increases the angle of the cone")]
    public float angle;
    Physics physics;

    Collider myColl;

    void Awake()
    {
        timetoWait = Random.Range(1f, 4f);
        TimeToTarget = Random.Range(TimeToTargetMin, TimeToTargetMax);
        tentacles = GameObject.FindGameObjectsWithTag("TentacleStack");
    }
    private void Start()
    {
        myColl = GetComponent<Collider>();
    }

    void Update()
    {
        if (targetPlayer == false)
        {
            StartCoroutine(TargetPlayer());
        }

        if (RecalcRot)
        {
            randomAng = new Vector3(Random.Range(-360, 360), Random.Range(-360, 360), Random.Range(-360, 360));
            randomRotation = Quaternion.Euler(randomAng);

            RecalcRot = false;
        }

        Vector3 myPos = gameObject.transform.position;
        RaycastHit[] coneHits = physics.ConeCastAll(myPos, radius, transform.forward, depth, angle);

        if (target != null)
        {
            locked = true;
        }

        else if (target == null)
        {
            locked = !true;
        }

        if (locked)
        {
            if (Started != true)
            {
                StartCoroutine(WaitForUnlock());
            }

            TentacleAttack attacker = null;
            float closestDist = Mathf.Infinity;

            // figure out which one is the closest tentacle
            for (int i = 0; i < tentacles.Length; i++)
            {
                if (tentacles[i] != null)
                {
                    currentTip = tentacles[i].transform.GetChild(tentacles[i].transform.childCount - 1);

                    TentacleAttack potentialAttacker = currentTip.GetComponent<TentacleAttack>();
                    if (potentialAttacker == false)
                    {
                        potentialAttacker = currentTip.gameObject.AddComponent<TentacleAttack>();
                        potentialAttacker.gameObject.AddComponent<ParticleSeek>();
                    }

                    float myDist = (target.transform.position - currentTip.transform.position).magnitude;
                    potentialAttacker.dist = myDist;

                    if (myDist < closestDist)
                    {
                        attacker = potentialAttacker;
                        closestDist = myDist;
                    }
                }
            }

            // give orders to attack
            if (attacker != null)
            {
                attacker.MyTurnToAttack = true;
                closestTip = attacker.transform;
            }

            // look at my target
            gameObject.transform.LookAt(target.transform.position);
        }


        if (!locked)
        {
            Quaternion currentRot = transform.rotation;
            float step = speed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, randomRotation, step);

            //Checks if the rotation locks itself and recalcs if the previous frames rotation == the current frames
            if (currentRot == lastRot)
            {
                RecalcRot = true;
            }
            lastRot = currentRot;

            //resets the rotation to look towards if it reaches its targeted rotation
            if (transform.rotation == randomRotation)
            {
                RecalcRot = true;
            }

            if (coneHits.Length > 0)
            {
                for (int i = 0; i < coneHits.Length; i++)
                {
                    if (coneHits[i].collider == myColl || coneHits[i].collider.tag == "Tentacle"
                     || coneHits[i].collider.tag == "Wall" || coneHits[i].collider.tag == "Obstacle")
                    {
                        continue;
                    }
                    target = coneHits[i].collider.gameObject;
                }
            }
        }
    }
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<Collider>().tag != "Tentacle" || other.gameObject.GetComponent<Collider>().tag != "Obstacle")
        {
            target = null;
            //locked = false;
            target = other.gameObject;
        }
    }
    IEnumerator WaitForUnlock()
    {
        Started = true;
        yield return new WaitForSeconds(timetoWait);
        timetoWait = Random.Range(1f, 4f);
        target = null;
        transform.rotation = Random.rotation;
        locked = false;
        Started = false;
    }
    IEnumerator TargetPlayer()
    {
        targetPlayer = true;
        yield return new WaitForSeconds(TimeToTarget);
        if(Player != null)
        {
            gameObject.transform.LookAt(Player.transform);
            target = Player;
            targetPlayer = false;
            TimeToTarget = Random.Range(TimeToTargetMin, TimeToTargetMax);
        }

    }
}
