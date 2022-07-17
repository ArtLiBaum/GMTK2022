using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeEnemyDice : EnemyDice
{
    public override void SpecialAbility()
    {
        base.SpecialAbility();
        DiceManager.SpecialsBlocked = true;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
}
