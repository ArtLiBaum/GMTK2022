using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
    public static List<PlayerDice> allDice = new List<PlayerDice>();
    private static int numLanded = 0;
    private static int rollTotal = 0;
    public static void Landed() => ++numLanded;
    private static bool rolling = false;
    public static bool Rolling =>rolling;

    public static bool SpecialsBlocked = false;

    private void Start()
    {
        rolling = false;
        numLanded = 0;
        rollTotal = 0;
    }

    public void RollAllDice()
    {
        numLanded = 0;
        rolling = true;
        StartCoroutine(TallyRolls());
        foreach (var dice in allDice)
        {
            dice.RollDice();
        }
    }

    public void ResetAllDice()
    {
        foreach (var dice in allDice)
        {
            dice.Reset();
        }
        rolling = false;
    }

    public void ActivateSpecials()
    {
        if(SpecialsBlocked) return;
        
        foreach (var dice in allDice)
        {
            dice.SpecialAbility();
        }
    }

    public void ActivatePowerUps()
    {
        if(SpecialsBlocked) return;
        foreach (var dice in allDice)
        {
            dice.RoundUpGrade();
        }
    }

    IEnumerator TallyRolls()
    {
        int numDice = allDice.Count;
        while (numLanded < numDice)
        {
            yield return null;
        }

        rollTotal = 0;
        foreach (var dice in allDice)
        {
            rollTotal += dice.DiceValue;
        }

        GameManager.RollResult = rollTotal;
        rolling = false;
    }
}
