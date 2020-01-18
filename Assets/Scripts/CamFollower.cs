using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollower : MonoBehaviour
{
    //transform to follow
    public Transform toFollow;
    //speed to follow at
    public float followSpeed;
    //distance to follow from
    public float followDistance;
    //height to follow from
    public float followHeight;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, toFollow.position + new Vector3(0, followHeight, -followDistance), Time.deltaTime * followSpeed);
    }
}
