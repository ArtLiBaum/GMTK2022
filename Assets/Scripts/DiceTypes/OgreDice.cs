using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OgreDice : PlayerDice
{
    [SerializeField] int powerUp = 0;
    public override void RoundUpGrade()
    {
        base.RoundUpGrade();
        ++powerUp;
    }

    protected override int SideValueCheck()
    {
        
        base.SideValueCheck();
        return diceValue += powerUp;
    }
}
