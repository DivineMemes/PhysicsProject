using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GunAim : MonoBehaviour {

    public GameObject rocketLauncher;
    private Rigidbody rocketLauncherRB;
    public GameObject rifle;
    private Rigidbody rifleRB;
    public LayerMask mask;

    private int gunForce = 4;

    private void Start()
    {
        rocketLauncherRB = rocketLauncher.GetComponent<Rigidbody>();
        rifleRB = rifle.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update () {

        if (Input.GetKey(KeyCode.R))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity , mask))
            {
                Vector3 hitPos = hit.point;
                rocketLauncherRB.velocity = Vector3.forward * gunForce;
                rocketLauncher.transform.LookAt(hitPos);
            }

        }

        if (Input.GetKey(KeyCode.Mouse1)) {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, mask))
            {
                Vector3 hitPos = hit.point;
                rifleRB.velocity = Vector3.forward * gunForce;
                rifle.transform.LookAt(hitPos);
            }
        }

	}
}
