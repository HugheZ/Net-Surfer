using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instakill : Powerup
{
    //applies the powerup
    protected override void Apply()
    {
        LevelManager.Instance.GiveInstaKill();
    }
}
