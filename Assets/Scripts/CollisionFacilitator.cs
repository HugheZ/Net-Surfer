using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionFacilitator : MonoBehaviour
{
    //particle system for death
    public ParticleSystem deathParticles;
    //time to delay death
    public float deathTime;
    //angle offset to allow death
    public float angleOffset;
    //animator
    public Animator anim;
    //guitar mesh renderer
    public MeshRenderer guitarMesh;
    //player mesh renderer
    public SkinnedMeshRenderer playerMesh;

    //handles collisions
    private void OnCollisionEnter(Collision collision)
    {
        //if collide with enemy, kill instantly
        if (collision.gameObject.CompareTag("ENEMY"))
        {
            Die();

        } //else if obstacle, kill if not collided from below
        else if (collision.gameObject.CompareTag("OBSTACLE"))
        {
            print(Vector3.Dot(collision.GetContact(0).normal, Vector3.up));
            //if angle of contact is less than offset and collided in front of or at current depth, kill
           if(Vector3.Dot(collision.GetContact(0).normal, Vector3.up) < angleOffset && collision.GetContact(0).point.z >= transform.position.z)
            {
                Die();
            }
        }
    }

    //handles powerup collision
    private void OnTriggerEnter(Collider other)
    {
        //if collide with static score, double score, or instakill, collect
        if (other.gameObject.CompareTag("POWERUP") ||
            other.gameObject.CompareTag("DOUBLE_SCORE"))
        {
            other.gameObject.GetComponent<Powerup>().Collect();

        }
    }

    //called to kill the player
    private void Die()
    {
        //tell manager
        LevelManager.Instance.PlayerDied();
        //set animator
        anim.SetTrigger("DIED");
        //disable scripts
        GetComponent<PlayerMover>().enabled = false;
        GetComponentInChildren<IKArmMover>().enabled = false;
        GetComponentInChildren<GunShoot>().enabled = false;
        this.enabled = false;
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        //play particles
        if(deathParticles) deathParticles.Play();
        //play sound
        GetComponent<AudioSource>().Play();
        //invoke last death method
        Invoke("Dead", deathTime);
    }

    //called after player died
    private void Dead()
    {
        //tell manager
        LevelManager.Instance.EndGame();
        //set renderers inactive
        guitarMesh.enabled = false;
        playerMesh.enabled = false;
    }
}
