using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static int LastRoll = 0;
    [SerializeField] private static TMP_Text _RollDisplay;

    private static int rollResult;
    public static int RollResult
    {
        get => rollResult;
        set
        {
            rollResult = value;
            _RollDisplay.text = rollResult.ToString();
        }
    }

    private static int _health;

    private void Start()
    {
        _RollDisplay = GameObject.Find("RollDisplay").GetComponent<TMP_Text>();
    }
}
