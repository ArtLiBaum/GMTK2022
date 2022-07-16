using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionSlot : MonoBehaviour
{
    [SerializeField] private Image DicePortraitUI;

    [SerializeField] private int slotNumber = 0; 

    [SerializeField] private Sprite[] portraits;

    private int index = 0;

    private void Start()
    {
        DicePortraitUI = transform.Find("Display").GetComponent<Image>();
        RefreshDisplay();
    }

    void RefreshDisplay()
    {
        DicePortraitUI.sprite = portraits[index];
    }

    public void Select(bool up)
    {
        index = DiceManager.instance.SelectDice(up,slotNumber);
        RefreshDisplay();
    }
    
}
