using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Yarn.Compiler;

[CustomEditor(typeof(Dice))]
public class DiceEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var dice = target as Dice;
        if (!Application.isPlaying) return;
        if (dice.Landed)
        {
            if (GUILayout.Button("Reset"))
            {
                dice.Reset();
            }
        }
        else
        {
            if (GUILayout.Button("Roll Dice"))
            {
                dice.RollDice();
            }
        }

    }
}

[CustomEditor(typeof(EnemyDice))]
public class EnemyDiceEditor : DiceEditor
{
    
}