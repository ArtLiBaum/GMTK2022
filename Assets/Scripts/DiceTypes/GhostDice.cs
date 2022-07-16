using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GhostDice : PlayerDice
{
    private GameManager _manager;
    public override void Start()
    {
        base.Start();
        _manager = FindObjectOfType<GameManager>();
    }
    public override void SpecialAbility()
    {
        //Reduce Enemy's toughness by 1 per ghost
        _manager.CurrentEnemy.Weaken();
    }
}
