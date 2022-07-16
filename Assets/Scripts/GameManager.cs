using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.UI;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static TMP_Text _RollDisplay;
    private GameObject _RollPanel;
    private Button _RollButton;
    [SerializeField] private List<GameObject> enemyDiceQueue;
    private int enemyIndex = 0;
    [SerializeField] private Transform EnemySpawn;
 
    
    
    public static EnemyDice currentEnemy;
    private DiceManager _diceManager;

    private int playerHealth = 5;
    public EnemyDice CurrentEnemy
    {
        get
        {
            if (currentEnemy == null)
            {
               
                currentEnemy = Instantiate(enemyDiceQueue[enemyIndex],EnemySpawn).GetComponent<EnemyDice>();
            }

            return currentEnemy;
        }
        set => currentEnemy = value;
    }
    

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
        _RollPanel = GameObject.Find("RollPanel");
        _RollButton = _RollPanel.transform.Find("RollButton").GetComponent<Button>();
        _RollDisplay = GameObject.Find("RollDisplay").GetComponent<TMP_Text>();
        
        _RollPanel.SetActive(false);
        _RollDisplay.gameObject.SetActive(false);
        _diceManager = FindObjectOfType<DiceManager>();
        //TEMP FOR TESTING
        //StartCoroutine(BattlePhase());
    }

    int CheckRollOutcome()
    {
        if (rollResult < currentEnemy.Toughness)
            return -1;
        if (rollResult > currentEnemy.Toughness)
        {
            return rollResult > currentEnemy.TKO ? 2 : 1;
        }
        return 0;
    }

    void Restart()
    {
        _diceManager.ResetAllDice();
        currentEnemy.Reset();
    }

    void KillEnemy()
    {
        Destroy(currentEnemy.gameObject);
        ++enemyIndex;
    }

    public void StartBattle()
    {
        //Called after Player Chooses Dice
        StartCoroutine(BattlePhase());
    }

   public IEnumerator BattlePhase()
    {
        goto FirstRound;

        RestartRound:
        _diceManager.ActivateSpecials();

        FirstRound:
        _diceManager.NewRound();
        CurrentEnemy.RollDice();
        
        
        
        //Enemy Rolls its Damage
        while (!CurrentEnemy.Landed)
        {
            yield return null;//wait for enemy dice to resolve
        }
        
        _RollPanel.SetActive(true);
        _RollDisplay.gameObject.SetActive(true);
        
        //Player Rolls Dice
        while (!DiceManager.Rolling)
        {
            yield return null;
        }
        _RollPanel.SetActive(false);
        while (DiceManager.Rolling)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        _RollDisplay.gameObject.SetActive(false);

        // Dice Value is checked against Enemy Stats
         // One of three results
        switch (CheckRollOutcome())
        {
            case -1 : //player takes damage
               playerHealth -= currentEnemy.DiceValue;
                Debug.Log("Player Lost Round");
                break;
            case 0: //meets it beats it
            case 1: //enemy takes hit
                currentEnemy.TakeDamage();
                Debug.Log("Enemy Lost Round");
                break;
            case 2: // Enemy it taken out
                currentEnemy.KnockedOut();
                Debug.Log("Enemy Was TKO'd!");
                break;
        }
        // Enemy & Player both alive, go to Enemy's Rolls
        if (currentEnemy.IsAlive() && playerHealth > 0)
        {
            _diceManager.ActivatePowerUps();
            Restart();
            goto RestartRound;
        }
        // Enemy Dies
        if (!currentEnemy.IsAlive())
        {
            //TODO: Load Next Sequence
            KillEnemy();
            Debug.Log("EnemyDefeated");
        }
        // Player Dies
        if (playerHealth <= 0)
        {
            //TODO GAME OVER SCREEN
            Debug.Log("GameOver");
        }
    }
}
