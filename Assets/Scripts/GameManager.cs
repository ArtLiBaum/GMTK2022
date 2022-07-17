using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Yarn.Unity;

public class GameManager : MonoBehaviour
{
    private static TMP_Text _RollDisplay;
    private GameObject _RollPanel;
    private Button _RollButton;
    private BattleDisplay _battleDisplay;
    [SerializeField] private List<GameObject> enemyDiceQueue;
    private int enemyIndex = 0;
    [SerializeField] private Transform EnemySpawn;

    public static EnemyDice currentEnemy;
    private DiceManager _diceManager;

    private GameObject diceSelect;
    private EnemyDisplay _enemyDisplay;

    private DialogueRunner _dialogueRunner;

    public bool battling = false;

    private static int _playerHealth = 5;
    public static int PlayerHealth => _playerHealth;
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


    private void Start()
    {
        _diceManager = FindObjectOfType<DiceManager>();
        _battleDisplay = FindObjectOfType<BattleDisplay>();
        GameObject combat = GameObject.Find("CombatHud");
        
        _enemyDisplay = FindObjectOfType<EnemyDisplay>();
        _enemyDisplay.gameObject.SetActive(false);
        diceSelect = combat.transform.Find("DiceSelection").gameObject;
        diceSelect.transform.Find("StartButton").GetComponent<Button>().onClick.AddListener(StartBattle);
        diceSelect.SetActive(false);
        
        _RollPanel = _battleDisplay.RollPanel;
        _RollButton = _battleDisplay.RollButton;
        _RollDisplay = _battleDisplay.RollDisplay;
        
        _RollPanel.SetActive(false);
        _RollDisplay.gameObject.SetActive(false);

        _dialogueRunner = FindObjectOfType<DialogueRunner>();
        
        _dialogueRunner.AddCommandHandler("StartBattle",PreBattle);
        
        _playerHealth = 5;

        //TEMP FOR TESTING
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
        battling = true;
    }

    void PreBattle()
    {
        StartCoroutine(BattlePhase());
    }

    public IEnumerator BattlePhase()
    {
        Debug.Log("Starting Battle Phase");
        
        //Display Enemy
        _enemyDisplay.gameObject.SetActive(true);
        _enemyDisplay.LoadDisplay(CurrentEnemy);
        yield return new WaitForSeconds(4f);
        _enemyDisplay.gameObject.SetActive(false);
        //Give dice select option
        diceSelect.SetActive(true);
        
        //Player chooses dice
        while (!battling)
            yield return null;
        diceSelect.SetActive(false);
        
        _diceManager.ActivateSpecials();
        
        RestartRound:
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
               _playerHealth -= currentEnemy.DiceValue;
               _battleDisplay.UpdatePlayerHealth();
                Debug.Log("Player Lost Round");
                break;
            case 0: //meets it beats it
            case 1: //enemy takes hit
                currentEnemy.TakeDamage();
                _battleDisplay.UpdateEnemyHits(CurrentEnemy);
                Debug.Log("Enemy Lost Round");
                break;
            case 2: // Enemy it taken out
                currentEnemy.KnockedOut();
                _battleDisplay.UpdateEnemyHits(CurrentEnemy);
                Debug.Log("Enemy Was TKO'd!");
                break;
        }
        // Enemy & Player both alive, go to Enemy's Rolls
        if (currentEnemy.IsAlive() && _playerHealth > 0)
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
        if (_playerHealth <= 0)
        {
            //TODO GAME OVER SCREEN
            Debug.Log("GameOver");
        }
    }
}
