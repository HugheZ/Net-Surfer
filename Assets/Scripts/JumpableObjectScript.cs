using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpableObjectScript : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (transform.position.z <= -25)
        {
            Destroy(gameObject);
        }

        if(transform.position.y > 2.55)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, 2.60f, transform.position.z - 3), Time.deltaTime * 40f);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y + 5, transform.position.z), Time.deltaTime * 30f);
        }
    }
}
