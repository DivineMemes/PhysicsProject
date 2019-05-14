using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleEye2 : MonoBehaviour
{

    public GameObject target;
    public GameObject Player;
    public bool locked = false;
    bool targetPlayer;
    bool Started;




    [Header("PanSettings")]
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

    [Header("tentacle management")]
    public List<Transform> bases;
    public List<Transform> tips;
    public List<Transform> segments;
    public GameObject[] TentacleArrays;
    public Transform[] allChildren;


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
    }

    void Start()
    {
        myColl = GetComponent<Collider>();
    }

    void Update()
    {
        TentacleArrays = GameObject.FindGameObjectsWithTag("Tentacle"); //theres probably a better way to update the lists but this worked for me with no noticable problems
        for (int i = 0; i < TentacleArrays.Length; i++)
        {
            allChildren = TentacleArrays[i].GetComponentsInChildren<Transform>();
            foreach (Transform child in allChildren)
            {
                /*
                if (segments.Contains(child.transform) == false)
                {
                    segments.Add(child.transform);
                    if (child.transform.gameObject.GetComponent<Light>() == false)
                    {
                        Light theLight = child.transform.gameObject.AddComponent<Light>();
                        Color lightColor = new Vector4(119, 255, 0, 255);
                        theLight.color = lightColor;
                        theLight.intensity = 10;
                    }
                }
                */

                if (child.transform.parent == null)
                {
                    if (bases.Contains(child.transform) == false)
                    {
                        bases.Add(child.transform);
                    }
                }
                else if (child.transform.parent != null && bases.Contains(child.transform))
                {
                    bases.Remove(child.transform);
                }

                if (child.transform.childCount == 0)
                {

                    if (tips.Contains(child.transform) == false)
                    {
                        tips.Add(child.transform);
                        if (child.transform.gameObject.GetComponent<TentacleAttack2>() == null)
                        {
                            child.transform.gameObject.AddComponent<TentacleAttack2>();
                        }
                        if(child.transform.gameObject.GetComponent<RegenTip>() == null)
                        {
                            child.transform.gameObject.AddComponent<RegenTip>();
                        }
                    }
                }
                else if (child.transform.childCount != 0 && tips.Contains(child.transform))
                {
                    tips.Remove(child.transform);
                    Destroy(child.gameObject.GetComponent<TentacleAttack2>());
                    Destroy(child.gameObject.GetComponent<RegenTip>());
                }
            }
        }

        for (int i = 0; i < tips.Count; i++)
        {
            if (tips[i] == null)
            {
                tips.RemoveAt(i);
            }
        }
        for (int i = 0; i < bases.Count; i++)
        {
            if (bases[i] == null)
            {
                bases.RemoveAt(i);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////
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

            TentacleAttack2 attacker = null;
            float closestDist = Mathf.Infinity;

            // figure out which one is the closest tentacle
            for (int i = 0; i < tips.Count; i++)
            {
                //if (tips[i] != null)
                {
                    currentTip = tips[i].transform;

                    TentacleAttack2 potentialAttacker = currentTip.GetComponent<TentacleAttack2>();
                    //if (potentialAttacker == false)
                    //{
                    //    potentialAttacker = currentTip.gameObject.AddComponent<TentacleAttack>();
                    //    potentialAttacker.gameObject.AddComponent<ParticleSeek>();
                    //}

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
                    if (coneHits[i].collider == myColl || coneHits[i].collider.CompareTag("Tentacle")
                     || coneHits[i].collider.CompareTag("Wall") || coneHits[i].collider.CompareTag("Obstacle"))
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
        if (Player != null)
        {
            gameObject.transform.LookAt(Player.transform);
            target = Player;
            targetPlayer = false;
            TimeToTarget = Random.Range(TimeToTargetMin, TimeToTargetMax);
        }

    }
}
