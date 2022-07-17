using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
    public static DiceManager instance;
    [SerializeField] private List<Transform> PlayerSpawns = new List<Transform>();
    public List<PlayerDice> allDice = new List<PlayerDice>();
    private static int numLanded = 0;
    private static int rollTotal = 0;
    private bool isDirty = true;
    public static void Landed() => ++numLanded;
    private static bool rolling = false;
    public static bool Rolling => rolling;

    public static bool SpecialsBlocked = false;

    [SerializeField] private List<PlayerDice> dicePrefabs = new List<PlayerDice>();
    private int[] diceSelection = { 0, 0, 0, 0, 0 };

    private void OnEnable()
    {
        if (instance)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {


        rolling = false;
        numLanded = 0;
        rollTotal = 0;
        foreach (Transform t in transform)
        {
            PlayerSpawns.Add(t);
        }
    }

    private void OnDisable()
    {
        PlayerSpawns.Clear();
    }


    public int SelectDice(bool scrollUp, int slot)
    {
        isDirty = true;
        int index = diceSelection[slot];
        index = Math.Abs((scrollUp ? index + 1 : index > 0 ? index - 1: 2) % 3);
        return diceSelection[slot] = index;
    }

    public void SpawnDice()
    {
        foreach (var dice in allDice)
        {
            Destroy(dice.gameObject);
        }

        allDice.Clear();

        for (int i = 0; i < 5; ++i)
        {
            Instantiate(dicePrefabs[diceSelection[i]], PlayerSpawns[UnityEngine.Random.Range(0, PlayerSpawns.Count)]);
        }
    }

    public void NewRound()
    {
        if (isDirty)
        {
            SpawnDice();
            isDirty = false;
        }
        else
        {
            ResetAllDice();
        }
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
        if (SpecialsBlocked) return;

        foreach (var dice in allDice)
        {
            dice.SpecialAbility();
        }
    }

    public void ActivatePowerUps()
    {
        if (SpecialsBlocked) return;
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