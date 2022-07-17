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

    public static int PlayerHealth
    {
        get => _playerHealth;
        set
        {
            _playerHealth = value;
            audioEmitter.SetParameter("Health",_playerHealth);
        }
    }

    private EnemyDice currentEnemy;

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
    private ResultWindowScript _resultWindow;
    private static FMODUnity.StudioEventEmitter audioEmitter;


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

        _dialogueRunner.AddCommandHandler("StartBattle", PreBattle);
        _dialogueRunner.AddCommandHandler("TutorialStart",SetTutorial);
        _dialogueRunner.AddCommandHandler("FinalBattleStart",SetFinalBattle);

        _resultWindow = FindObjectOfType<ResultWindowScript>();
        _resultWindow.gameObject.SetActive(false);

        audioEmitter = GetComponent<FMODUnity.StudioEventEmitter>();
        
        _playerHealth = 10;
        audioEmitter.SetParameter("Death",0);
        audioEmitter.SetParameter("Victory",0);
    }

    public static void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void SetTutorial()
    {
        tutorial = true;}

    void SetFinalBattle()
    {
        lastBattle = true;
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
        CurrentEnemy.Reset();
    }

    void KillEnemy()
    {
        currentEnemy.gameObject.SetActive(false);
        currentEnemy = null;
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

        //Que music 
        audioEmitter.Play();
        audioEmitter.SetParameter("Battle",1f);
        SFXManager.instance.PlayRoar();
        
        //Display Enemy
        _enemyDisplay.gameObject.SetActive(true);
        _battleDisplay.UpdatePlayerHealth();
        _battleDisplay.UpdateEnemyHits(CurrentEnemy);
        _enemyDisplay.LoadDisplay(CurrentEnemy);
        yield return new WaitForSeconds(4f);
        _enemyDisplay.gameObject.SetActive(false);
        


        //Give dice select option
        diceSelect.SetActive(true);
        if (tutorial)//Choose Dice Tutorial
        {
            _dialogueRunner.StartDialogue("TutorialStart");
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
            _dialogueRunner.StartDialogue("TutorialEnemyRoll");
        }

        if (lastBattle)//Talk before the final battle
        {
            _dialogueRunner.StartDialogue("SlimeFightStart");
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
                _resultWindow.gameObject.SetActive(true);
                SFXManager.instance.PlayFail();
                _resultWindow.OpenBox("Player Lost","Enemy's Damage Goes Through");
                yield return new WaitForSeconds(2.5f);
                if (tutorial)//Outcome Tutorial
                {
                    _dialogueRunner.StartDialogue("TutorialOuch");
                    while (_dialogueRunner.IsDialogueRunning)
                    {
                        yield return null;
                    }
                }
                SFXManager.instance.PlayPlayerHit();
                _resultWindow.gameObject.SetActive(false);
               _playerHealth -= currentEnemy.DiceValue;
               _battleDisplay.UpdatePlayerHealth();
                
                yield return new WaitForSeconds(1f);
               
                Debug.Log("Player Lost Round");
                break;
            case 0: //meets it beats it
            case 1: //enemy takes hit
                _resultWindow.gameObject.SetActive(true);
                SFXManager.instance.PlaySuccess();
                _resultWindow.OpenBox("Player Win","Enemy Takes A Hit");
                yield return new WaitForSeconds(2.5f);
                if (tutorial)//Outcome Tutorial
                {
                    _dialogueRunner.StartDialogue("TutorialHit");
                    while (_dialogueRunner.IsDialogueRunning)
                    {
                        yield return null;
                    }
                }
                _resultWindow.gameObject.SetActive(false);
                currentEnemy.TakeDamage();
                _battleDisplay.UpdateEnemyHits(CurrentEnemy);
                Debug.Log("Enemy Lost Round");
                break;
            case 2: // Enemy it taken out
                _resultWindow.gameObject.SetActive(true);
                SFXManager.instance.PlaySuccess();
                _resultWindow.OpenBox("KNOCKOUT!","Enemy TKO!!");
                yield return new WaitForSeconds(1.5f);
                _resultWindow.gameObject.SetActive(false);
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
            audioEmitter.SetParameter("Victory",1);
            //start dialogue before killing enemy
            _dialogueRunner.StartDialogue(dialogueSequence[enemyIndex]);
            KillEnemy();
            
          tutorial = lastBattle = false;
            Debug.Log("EnemyDefeated");
        }
        // Player Dies
        if (_playerHealth <= 0)
        {
            if (lastBattle)//Died during the last battle
            {
                _dialogueRunner.StartDialogue("SlimeFightDie");
                //Revive the player?
            }
            else
            {
                gameOver.SetActive(true);
                audioEmitter.SetParameter("Death",1);
            }
            Debug.Log("GameOver");
        }

        battling = false;
        audioEmitter.SetParameter("Battle",0f);
        audioEmitter.Stop();
    }
}
