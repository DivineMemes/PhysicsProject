using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public static PlayerController SharedInstance { get; set; }

    public float thrust = 2000;
    public float straifThrust = 2000;
    public float vThrust = 2000;
    public Camera mainCam;
    Rigidbody rb;
    bool isPaused = false;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();

        if (SharedInstance == null)
        {
            SharedInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.W))
        {
            rb.AddForceAtPosition(mainCam.transform.forward * thrust, transform.position + transform.up);
        }

        if (Input.GetKey(KeyCode.S))
        {
            rb.AddForceAtPosition(-(mainCam.transform.forward) * thrust, transform.position - transform.up);
        }

        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce((mainCam.transform.right * -1) * straifThrust);
            rb.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(mainCam.transform.forward), 0.15f);
            
        }

        if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(mainCam.transform.right * thrust);
            rb.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(mainCam.transform.forward), 0.15f);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            rb.AddForceAtPosition(mainCam.transform.up * thrust, transform.position + transform.up);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            rb.AddForceAtPosition(-(mainCam.transform.up) * thrust, transform.position - transform.up);
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            rb.velocity = rb.velocity * 0.1f;
            rb.angularVelocity = rb.angularVelocity * 0.1f;
        }

        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    if (isPaused)
        //    {
        //        isPaused = false;
        //    }
        //    else
        //    {
        //        isPaused = true;
        //    }
        //}

        //if (isPaused == false)
        //{
        //    Cursor.visible = false;
        //    Cursor.lockState = CursorLockMode.Locked;
        //}
        //else
        //{
        //    Cursor.visible = true;
        //    Cursor.lockState = CursorLockMode.None;
        //}
	}
}
