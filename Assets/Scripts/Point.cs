using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : Powerup
{
    //point value
    public int pointVal;

    //applies the powerup
    protected override void Apply()
    {
        LevelManager.Instance.GivePoints(pointVal);
    }
}
