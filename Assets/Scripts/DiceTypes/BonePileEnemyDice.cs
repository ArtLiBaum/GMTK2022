using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonePileEnemyDice : EnemyDice
{
    // Start is called before the first frame update
    public override void SpecialAbility()
    {
        base.SpecialAbility();
        BoostHits();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
