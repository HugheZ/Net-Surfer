using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    //rigidbody
    Rigidbody rb;
    //jump force
    public float jumpForce;
    //degrading force
    float jumpForceApplied;
    //degrade value
    public float jumpForceDegrade;
    //gravity degrade
    public float gravDegrade;
    //player speed
    public float speed;
    //direction
    float direc;
    //if jumped
    bool jumped;
    //if jumping and in air
    bool jumping;
    //is grounded
    bool grounded;
    //is falling
    bool falling;
    //is paused, turns off input
    bool paused;
    //non player physics layer
    public LayerMask groundLayer;
    //collider
    public CapsuleCollider collide;
    //animator controller for animation
    public Animator anim;
    //jumping audio
    public AudioClip jump;
    //audio source
    AudioSource jumpSource;

    // Start is called before the first frame update
    void Start()
    {
        //anim init
        anim.SetBool("IN_AIR", false);
        //regular init
        jumped = false;
        jumping = false;
        grounded = false;
        falling = false;
        paused = false;
        jumpForceApplied = 0;
        rb = GetComponent<Rigidbody>();
        jumpSource = GetComponent<AudioSource>();
        direc = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!paused)
        {
            direc = Input.GetAxis("Horizontal");
            if (Input.GetKeyDown(KeyCode.Space) && grounded)
            {
                //set anim
                anim.SetTrigger("JUMPED");
                //set jump vals
                jumped = true;
                jumping = true;
                jumpForceApplied = jumpForce;
                //play sound
                if (jumpSource) jumpSource.PlayOneShot(jump);
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                jumping = false;
                falling = true;
                if (rb.velocity.y > 0)
                {
                    rb.velocity = new Vector3(rb.velocity.x, 0, 0);
                    jumpForceApplied = 0;
                }
            }
        }
    }

    //Fixed update move player
    private void FixedUpdate()
    {
        //get groundedness
        grounded = Grounded();

        //set falling vals
        if (grounded)
        {
            falling = false;
        }

        rb.velocity = new Vector3(direc * speed, rb.velocity.y);

        //jump logic
        if (jumped)
        {
            jumped = false;
            rb.AddForce(Vector3.up * jumpForceApplied, ForceMode.VelocityChange);
        }
        if (jumping || falling)
        {
            rb.AddForce(Vector3.up * jumpForceApplied, ForceMode.VelocityChange);

            float totalDegrade;
            if (jumping) totalDegrade = jumpForceDegrade;
            else totalDegrade = jumpForceDegrade + gravDegrade;

            jumpForceApplied -= totalDegrade;
            jumpForceApplied = Mathf.Clamp(jumpForceApplied, -jumpForce, jumpForce);
        }
        if(!grounded && rb.velocity.y < 0)
        {
            jumping = false;
            falling = true;
        }

        //anim control
        if(falling || jumping)
        {
            anim.SetBool("IN_AIR", true);
        }
        else
        {
            anim.SetBool("IN_AIR", false);
        }
    }

    //sets pause value
    public void SetPause(bool pause)
    {
        paused = pause;
    }

    //Determines if the player is grounded
    bool Grounded()
    {
        Physics.SphereCast(transform.position, collide.radius - .1f, Vector3.down, out RaycastHit hit, collide.height/2.0f, groundLayer);
        Debug.DrawRay(transform.position, new Vector3(0, -(collide.height / 2.0f + collide.radius), 0));
        return hit.collider != null;
    }
}
