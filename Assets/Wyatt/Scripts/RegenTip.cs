using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegenTip : MonoBehaviour
{
    public GameObject RegenSegment;
    public float min = 5;
    public float max = 10;
    public float RandomTime;
    bool started = false;
    void Start()
    {
        var prefab = Resources.Load<GameObject>("Prefabs/Capsule");
        RegenSegment = prefab;
        RandomTime = Random.Range(min, max);
        StartCoroutine(WaitForRegen());

    }

    IEnumerator WaitForRegen()
    {
        started = true;
        yield return new WaitForSeconds(RandomTime);
        GameObject SpawnedObject = Instantiate(RegenSegment, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z), gameObject.transform.rotation);
        SpawnedObject.transform.SetParent(transform);
        SpawnedObject.transform.localPosition = new Vector3(0, 2, 0);
        SpawnedObject.transform.localRotation = Quaternion.identity;
        var joint = gameObject.AddComponent<CharacterJoint>();
        joint.connectedBody = SpawnedObject.GetComponent<Rigidbody>();
        RandomTime = Random.Range(min, max);
        started = false;
    }

}
