using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockHandEvents : MonoBehaviour
{
    //plays audiosource if exists
    public void PlayRiff()
    {
        GetComponent<AudioSource>().Play();
    }

    //disables gameobject
    public void SetInactive()
    {
        gameObject.SetActive(false);
    }
}
