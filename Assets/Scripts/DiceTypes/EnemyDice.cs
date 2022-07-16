using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class EnemyDice : Dice
{
    private TMP_Text enemyDisplayText, enemyToughnessText;
    [SerializeField] private int hits = 2;
    public int Hits => hits;

    [SerializeField] private int toughness = 15;
    public int Toughness => toughness;

    [SerializeField] private int tko = 24;
    public int TKO => tko;
    

    public override void Start()
    {
        base.Start();
        enemyDisplayText = GameObject.Find("EnemyRollDisplay").GetComponent<TMP_Text>();
        enemyToughnessText = GameObject.Find("EnemyToughnessDisplay").GetComponent<TMP_Text>();
        enemyToughnessText.text = toughness.ToString();
    }

    protected override int SideValueCheck()
    {
        base.SideValueCheck();
        enemyDisplayText.text = DiceValue.ToString();
        return DiceValue;

    }

    public virtual void SpecialAbility()
    {
    }

    public virtual int Attack()
    {
        return DiceValue;
    }

    public virtual void TakeDamage()
    {
        --hits;
        if (hits <= 0)
        {
            //fights over, enemy dies
        }
    }

    public void Weaken()
    {
        --toughness;
        enemyToughnessText.text = toughness.ToString();
    }

    protected void BoostHits()
    {
        ++hits;
    }


    public void KnockedOut()
    {
        hits = 0;
    }

    public bool IsAlive()
    {
        return hits > 0;
    }
}
