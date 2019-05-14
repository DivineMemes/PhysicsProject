using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCam : MonoBehaviour
{

    /////////////////////////////////////////////////////////////////
    // This script is outdated, don't use it
    /////////////////////////////////////////////////////////////////

    public Transform target;
    public float distance;

    public float udRotation; // degrees of rotation
    public float lrRotation; // degrees of rotation

    public Vector3 axis = Vector3.up;

    private void LateUpdatee()
    {


        udRotation = Input.GetAxis("Mouse Y") * 10.0f * Time.deltaTime;
        lrRotation = Input.GetAxis("Mouse X") * 10.0f * Time.deltaTime;

        Vector3 offset = (target.position - transform.position).normalized;

        Vector3 direction = Quaternion.AngleAxis(lrRotation, axis.normalized) * offset;
        direction = Quaternion.AngleAxis(udRotation, Vector3.Cross(transform.up, transform.forward)) * direction;

        transform.position = target.position + direction * distance;
        transform.forward = target.position - transform.position;
        transform.LookAt(target, axis);
    }

    private void LateUpdate()
    {
        Vector3 direction = Quaternion.AngleAxis(lrRotation, axis.normalized) * Vector3.back;
        direction = Quaternion.AngleAxis(udRotation, transform.right) * direction;

        //Vector3 orthagonal = Vector3.Cross(axis, Quaternion.AngleAxis(-90.0f, axis) * axis);

        transform.position = target.position - direction * distance;
        transform.forward = target.position - transform.position;
        transform.LookAt(target, axis);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green / 2.0f;
        Gizmos.DrawSphere(target.position, distance);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(target.position, axis * 3.0f);

        Gizmos.color = Color.yellow;
        //Gizmos.DrawRay(transform.position, transform.forward * 3.0f);

        Vector3 input = new Vector3(Mathf.Cos(Mathf.Deg2Rad * (lrRotation+90)),
                            0.0f,
                            Mathf.Sin(Mathf.Deg2Rad * (lrRotation + 90)));
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position,  input.normalized * 3.2f);
    }
}