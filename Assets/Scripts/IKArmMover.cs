using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKArmMover: MonoBehaviour
{
    //animator
    public Animator anim;
    //main plane
    Plane p;
    //main position
    Vector3 position;

    //setup plane
    private void Start()
    {
        position = transform.localPosition;
        p = new Plane(Vector3.back, new Vector3(0, 0, transform.parent.position.z));
    }

    //on update, make sure transform is right
    private void Update()
    {
        transform.localPosition = position;
    }

    //set up plane
    private void OnEnable()
    {
        p = new Plane(Vector3.back, new Vector3(0, 0, transform.parent.position.z));
    }

    //move ik arm
    private void OnAnimatorIK(int layerIndex)
    {
        if (Time.timeScale > 0)
        {
            //get starting mouse position and convert to ray
            Vector3 mousepos = Input.mousePosition;
            mousepos.z = -20;
            Vector3 goal;
            Ray ray = Camera.main.ScreenPointToRay(mousepos);

            //calc distance
            float distance;

            //get plane intersect
            p.Raycast(ray, out distance);
            //determine goal point
            goal = ray.GetPoint(distance);

            //set ik weight to full
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            //set ik
            anim.SetIKPosition(AvatarIKGoal.RightHand, goal);
        }
    }
}
