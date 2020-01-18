using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Powerup : MonoBehaviour
{
    System.Random rand;
    //is movable? resets on disable
    public bool isMovable = true;

    // Start is called before the first frame update
    void Start()
    {
        if (isMovable)
        {
            rand = new System.Random();
            int xSpawn = rand.Next(-120, 120);
            transform.position = new Vector3(xSpawn, -200, 300);
        }
    }

    private void OnEnable()
    {
        if (isMovable)
        {
            rand = new System.Random();
            int xSpawn = rand.Next(-120, 120);
            transform.position = new Vector3(xSpawn, -200, 300);
        }
    }

    private void FixedUpdate()
    {
        if (transform.position.z <= -50)
        {
            ResetPowerup();
        }

        if (isMovable)
        {
            if (transform.position.y > 2.55)
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, 2.60f, transform.position.z - 3), Time.deltaTime * 40f);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y + 5, transform.position.z), Time.deltaTime * 30f);
            }
        }
    }

    //applies the powerup
    public void Collect()
    {
        Pickup();
        Apply();
        Invoke("ResetPowerup", 2.5f);
    }

    //powerup takes effect
    protected abstract void Apply();

    //pickup, disables appropriate properties
    void Pickup()
    {
        if (!CompareTag("DOUBLE_SCORE"))
        {
            GetComponent<Collider>().enabled = false;
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<ParticleSystem>().Play();
            this.enabled = false;
        }
        transform.SetParent(null);
        GetComponent<AudioSource>().Play();
    }

    //called for if holder died
    public void HolderDied()
    {
        transform.SetParent(null);
        isMovable = true;
    }

    //on particle system done, re-enable and disable
    void ResetPowerup()
    {
        if (!CompareTag("DOUBLE_SCORE"))
        {
            GetComponent<Collider>().enabled = true;
            GetComponent<MeshRenderer>().enabled = true;
            GetComponent<ParticleSystem>().Stop();
        }
        isMovable = true;
        gameObject.SetActive(false);
        this.enabled = true;
    }
}
