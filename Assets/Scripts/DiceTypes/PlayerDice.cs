using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDice : Dice
{
    private void OnEnable() => DiceManager.instance.allDice.Add(this);
    //private void OnDisable() => DiceManager.allDice.Remove(this);

    [SerializeField] protected string description;
    
    public virtual void SpecialAbility()
    {}
    
    public virtual void RoundUpGrade()
    {}
}
