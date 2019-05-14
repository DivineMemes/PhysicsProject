using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamTest : MonoBehaviour {

    [SerializeField]
    private float sensitivity;

    [SerializeField]
    private Rigidbody body;



    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");

        body.angularVelocity = transform.right * -my * sensitivity + transform.up * mx * sensitivity;
    }

}
