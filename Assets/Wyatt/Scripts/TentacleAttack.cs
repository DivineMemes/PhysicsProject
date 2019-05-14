using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleAttack : MonoBehaviour
{
    public float dist;
    public TentacleEyeDetection detect;
    public bool MyTurnToAttack = false;
    Vector3 heading;
    public GameObject MyTarget;
    //Vector3 Origin;
    void Awake()
    {
        Vector3 newForce = new Vector3(Random.Range(200, 300), Random.Range(200, 300), Random.Range(200, 300));
        gameObject.GetComponent<Rigidbody>().AddForce(newForce, ForceMode.Force);
        detect = GameObject.FindGameObjectWithTag("Eye").GetComponent<TentacleEyeDetection>();
        //Origin = gameObject.transform.position;
    }

    void Update()
    {
        if (detect.locked == false)
        {
            MyTarget = null;
            MyTurnToAttack = false;
        }
        if (detect.target != null)
        {
            MyTarget = detect.target.gameObject;
            heading = MyTarget.transform.position - gameObject.transform.position;
            dist = heading.magnitude;
        }
        if (detect.target != null && MyTurnToAttack == true)
        {
            Vector3 direction = heading / dist;
            gameObject.GetComponent<Rigidbody>().AddForce(direction * 5, ForceMode.Impulse);
        }

    }



    void OnCollisionEnter(Collision collision)
    {
        detect.target = null;
        detect.locked = false;
        //gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        MyTurnToAttack = false;
    }
}
