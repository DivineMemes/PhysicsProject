using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam2 : MonoBehaviour
{
    [SerializeField]
    private float sensitivity;

    [SerializeField]
    private float turnSpeed = 50;

    [SerializeField]
    private GameObject mainCam;

    private float camDistance;

    private Rigidbody rb;

    public LayerMask layerMask;

    //private bool isPaused = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        camDistance = -mainCam.transform.localPosition.z;
    }

    private void Update()
    {
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");

        rb.angularVelocity = transform.right * -my * sensitivity + transform.up * mx * sensitivity;

        if (Input.GetKey(KeyCode.E))
        {
            rb.AddTorque(transform.forward * (-turnSpeed));
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            rb.AddTorque(transform.forward * turnSpeed);
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.forward, out hit, camDistance, layerMask))
        {
            mainCam.transform.position = hit.point;
        }
        else
            mainCam.transform.position = transform.position + -transform.forward * camDistance;

        Debug.DrawRay(transform.position, (-transform.forward) * camDistance, Color.blue);

        //Mouse visability
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    if (isPaused)
        //    {
        //        isPaused = false;
        //        Cursor.lockState = CursorLockMode.Locked;
        //    }
        //    else
        //    {
        //        isPaused = true;
        //        Cursor.lockState = CursorLockMode.None;
        //    }
        //}
    }
}