using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour {

    /////////////////////////////////////////////////////////////////
    // This script is outdated, don't use it
    /////////////////////////////////////////////////////////////////

    public Transform target;
    public float distance = 5.0f;
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;

    public float zRotation = 10.0f;
 
    public float distanceMin = .5f;
    public float distanceMax = 15f;
 
    private Rigidbody rb;
 
    float x = 0.0f;
    float y = 0.0f;
    Vector3 lookAngle;

    float startZ;
    float currentZ;
 
    // Use this for initialization
    void Start () 
    {
        startZ = transform.rotation.z;
        currentZ = startZ;

        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
 
        rb = GetComponent<Rigidbody>();
 
        // Make the rigid body not change rotation
        if (rb != null)
        {
            rb.freezeRotation = true;
        }
    }

    void LateUpdate()
    {
        Vector3 input = new Vector3(Input.GetAxis("Mouse X"),
                                    -Input.GetAxis("Mouse Y"),
                                    Input.GetAxis("Roll")); // TODO: roll

        input *= Time.deltaTime;

        lookAngle += input;

        Quaternion.Euler(lookAngle.y, lookAngle.x, 0.0f);

        transform.rotation *= Quaternion.AngleAxis(input.x * xSpeed, transform.up);
        transform.rotation *= Quaternion.AngleAxis(input.y * ySpeed, transform.right);

        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 position = negDistance + target.position;

        transform.position = position;
    }

    void LateUpdatee () 
    {
        if (target)
        {
            x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
 
            Quaternion rotation = Quaternion.Euler(y, x, currentZ);
 
            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel")*5, distanceMin, distanceMax);
 
            RaycastHit hit;
            if (Physics.Linecast (target.position, transform.position, out hit)) 
            {
                distance -=  hit.distance;
            }
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + target.position;
 
            transform.rotation = rotation;
            transform.position = position;

            if (Input.GetKey(KeyCode.E))
            {
                currentZ -= zRotation;
            }
            else if (Input.GetKey(KeyCode.Q))
            {
                currentZ += zRotation;
            }
        }
    }
}
