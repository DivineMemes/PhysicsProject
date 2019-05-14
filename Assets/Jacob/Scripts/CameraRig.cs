using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour {

    [SerializeField]
    private GameObject body;

    private void Update()
    {
        transform.position = body.transform.position;
    }
}
