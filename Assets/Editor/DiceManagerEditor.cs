using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DiceManager))]
public class DiceManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var diceManager = target as DiceManager;
        if (!Application.isPlaying) return;

        if(!DiceManager.Rolling)
        {
            if (GUILayout.Button("Roll Dice"))
            {
                diceManager.RollAllDice();
            }
        }
        else
        {
            if (GUILayout.Button("Reset"))
            {
                diceManager.ResetAllDice();
            }
        }

    }
}
