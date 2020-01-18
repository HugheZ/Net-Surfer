using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public PlayerMover player;
    public LevelManager manager;
    Animator anim;
    float waitTime;
    Vector3 playerPosition;
    bool atEnd = false;
    bool triggerDive;
    bool inOffsetQueue;

    public float offset;
    public int listPosition;

    //audio source
    AudioSource enemySound;
    //death sound
    public AudioClip death;
    //dive sound
    public AudioClip dive;
    //value in points
    public int pointVal;

    // Start is called before the first frame update
    void Start()
    {
        enemySound = GetComponent<AudioSource>();
        player = FindObjectOfType<PlayerMover>();
        GetComponent<Collider>().enabled = true;
        ParticleSystem ps = GetComponent<ParticleSystem>();
        if(ps.isPlaying) ps.Stop();
        anim = GetComponent<Animator>();
        triggerDive = false;
        waitTime = Time.time;

        manager = FindObjectOfType<LevelManager>();
        inOffsetQueue = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float diff = Time.time - waitTime;
        
        if(transform.position.z >= 100 || atEnd == true)
        {
            atEnd = true;
            if (diff >= 10)
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(playerPosition.x, playerPosition.y, playerPosition.z - 35), Time.deltaTime * 5f);
                if (!triggerDive)
                {
                    //diving
                    triggerDive = true;
                    //play sound and show animation
                    if (enemySound) enemySound.PlayOneShot(dive);
                    anim.SetBool("DIVE", true);
                }
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(player.transform.position.x + offset, 30, 100), Time.deltaTime * 5f);
                playerPosition = player.transform.position;
            }
        }
        
        if(atEnd == false)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, 30, transform.position.z + 3), Time.deltaTime * 20f);
        }

        if(transform.position.z <= -55 && atEnd == true)
        {
            Kill();
        }
    }

    //detects clicks
    private void OnMouseDown()
    {
        KillEnemy();
    }

    //kills the enemy prefab
    public void KillEnemy()
    {
        //disable collider
        GetComponent<Collider>().enabled = false;
        //play particle system
        GetComponent<ParticleSystem>().Play();
        //play sound
        if (enemySound) enemySound.PlayOneShot(death);
        //invoke kill
        Invoke("Kill", 3);
        //disable behavior
        this.enabled = false;

        LevelManager.Instance.GivePoints(pointVal);
    }

    //destroys the prefab
    void Kill()
    {
        this.enabled = true;
        Debug.Log(this.enabled);
        manager.enemyList[listPosition] = null;
        Destroy(gameObject);
    }

}
