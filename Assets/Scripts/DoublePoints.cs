using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoublePoints : Powerup
{
    //applies the powerup
    protected override void Apply()
    {
        LevelManager.Instance.EnableDoublePoints();
    }
}
