using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunShoot : MonoBehaviour
{
    //shoot particle system
    public ParticleSystem shoot;
    //audio source
    public AudioSource audio;
    //clip
    public AudioClip pew;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.timeScale > 0)
        {
            //pew pew
            audio.PlayOneShot(pew);
            //play effect
            if(!shoot.isPlaying) shoot.Play();
        }
    }
}
