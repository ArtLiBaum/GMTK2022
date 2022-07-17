using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Yarn.Unity;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    
    public bool battling = false;
    
    [SerializeField] private List<GameObject> enemyDiceQueue;
    [SerializeField] private List<string> dialogueSequence;
    [SerializeField] private Transform EnemySpawn;
    [SerializeField] private bool tutorial = false;
    [SerializeField] private bool lastBattle = false;
    

    //Game Status variables
    private static int _playerHealth = 10;
    public static int PlayerHealth => _playerHealth;

    private static EnemyDice currentEnemy;

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
    
    private int enemyIndex = 0; //used for enemy and dialogue sequencing
    
    //External References
    private static TMP_Text _RollDisplay;
    private GameObject _RollPanel;
    private BattleDisplay _battleDisplay;
    private DiceManager _diceManager;
    private GameObject diceSelect;
    private GameObject gameOver;
    private EnemyDisplay _enemyDisplay;
    private DialogueRunner _dialogueRunner;


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
        _RollDisplay = _battleDisplay.RollDisplay;

        gameOver = combat.transform.Find("GameOver").gameObject;
        gameOver.SetActive(false);
        
        _RollPanel.SetActive(false);
        _RollDisplay.gameObject.SetActive(false);

        _dialogueRunner = FindObjectOfType<DialogueRunner>();
        
        _dialogueRunner.AddCommandHandler("StartBattle",PreBattle);
        
        _playerHealth = 10;
    }

    public static void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

    void PreBattle() //used to call coroutine
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
        if (tutorial)//Choose Dice Tutorial
        {
            _dialogueRunner.StartDialogue("");
        }
        
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
        
        
        if (tutorial)//Enemy Dice Land Tutorial
        {
            _dialogueRunner.StartDialogue("");
        }

        if (lastBattle)//Talk before the final battle
        {
            _dialogueRunner.StartDialogue("");
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
        if (tutorial)//Outcome Tutorial
        {
            _dialogueRunner.StartDialogue("");
            while (_dialogueRunner.IsDialogueRunning)
            {
                yield return null;
            }
        }
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
            //start dialogue before killing enemy
            _dialogueRunner.StartDialogue(dialogueSequence[enemyIndex]);
            KillEnemy();
            
            Debug.Log("EnemyDefeated");
        }
        // Player Dies
        if (_playerHealth <= 0)
        {
            //TODO GAME OVER SCREEN
            if (lastBattle)//Died during the last battle
            {
                _dialogueRunner.StartDialogue("");
                //Revive the player?
            }
            else
            {
                gameOver.SetActive(true);
            }
            Debug.Log("GameOver");
        }
    }
}
