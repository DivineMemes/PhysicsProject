using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnTouch : MonoBehaviour
{
    void OnCollisionEnter(Collision other)
    {
        var det = other.gameObject.GetComponent<DetatchChildren>();
        
        if (det)
        {
            det.SetThemFree();
        }

        Destroy(other.gameObject);
    }
}
