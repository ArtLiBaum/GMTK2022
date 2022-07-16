using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
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

    private GameObject diceSelect;
    private EnemyDisplay _enemyDisplay;

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
        _diceManager = FindObjectOfType<DiceManager>();
        
        GameObject combat = GameObject.Find("CombatHud");
        
        _enemyDisplay = FindObjectOfType<EnemyDisplay>();
        _enemyDisplay.gameObject.SetActive(false);
        diceSelect = combat.transform.Find("DiceSelection").gameObject;
        diceSelect.transform.Find("StartButton").GetComponent<Button>().onClick.AddListener(StartBattle);
        diceSelect.SetActive(false);
        
        _RollPanel = combat.transform.Find("BattlePanel/RollPanel").gameObject;
        _RollButton = _RollPanel.transform.Find("RollButton").GetComponent<Button>();
        _RollButton.onClick.AddListener(_diceManager.RollAllDice);
        _RollDisplay = combat.transform.Find("BattlePanel/RollDisplay").GetComponent<TMP_Text>();
        
        _RollPanel.SetActive(false);
        _RollDisplay.gameObject.SetActive(false);
        

        //TEMP FOR TESTING
        StartCoroutine(PreBattle());
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

    public IEnumerator PreBattle()
    {
        //Display enemy
        //TODO populate display with current enemy
        
        
        _enemyDisplay.gameObject.SetActive(true);
        _enemyDisplay.LoadDisplay(CurrentEnemy);
        yield return new WaitForSeconds(4f);
        _enemyDisplay.gameObject.SetActive(false);
        //Give dice select option
        diceSelect.SetActive(true);
    }

    public void StartBattle()
    {
        //Called after Player Chooses Dice
        StartCoroutine(BattlePhase());
        diceSelect.SetActive(false);
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
