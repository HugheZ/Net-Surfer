using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSlide : MonoBehaviour
{
    //speed value
    public float speed;
    //height to rise to
    public float regularHeight;
    //ending transform position
    Vector3 endPos;
    //position to move twoards on pawn
    Vector3 movePos;
    //transform for powerup, null if not exists
    public Transform pUp;
    //powerup script for said powwerup
    Powerup pScript;

    // Start is called before the first frame update
    void Start()
    {
        endPos = new Vector3(transform.position.x, regularHeight, -50f);
        movePos = new Vector3(transform.position.x, regularHeight, transform.position.z);

        if (pUp)
        {
            GameObject p = LevelManager.Instance.GetRandomPowerup();
            pScript = p.GetComponent<Powerup>();
            p.transform.SetParent(transform);
            p.transform.localPosition = pUp.transform.localPosition;
            pScript.isMovable = false;
            p.SetActive(true);
        }
    }

    //get end position on enable
    private void OnEnable()
    {
        
    }

    //on fixedupdate, move objects
    void FixedUpdate()
    {
        if (transform.position.z <= -50)
        {
            //if has powerup and not collected, remove it
            if (pUp)
            {
                    pScript.HolderDied();
            }

            gameObject.SetActive(false);
        }

        //if ready to move forward, do it
        if (transform.position.y >= regularHeight)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPos, Time.deltaTime * speed);
        }
        else //else move up
        {
            transform.position = Vector3.MoveTowards(transform.position, movePos, Time.deltaTime * speed);
        }
    }
}
