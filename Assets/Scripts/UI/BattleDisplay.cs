using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class BattleDisplay : MonoBehaviour
{  
    private static TMP_Text _RollDisplay;
    public TMP_Text RollDisplay => _RollDisplay;
    private GameObject _RollPanel;
    public GameObject RollPanel => _RollPanel;
    private Button _RollButton;

    private Slider healthBar;
    private GameObject enemyHits;
    private Transform[] hitObj;
    private void Awake()
    {
        _RollPanel = transform.Find("RollPanel").gameObject;
        _RollButton = _RollPanel.transform.Find("RollButton").GetComponent<Button>();
        _RollDisplay = transform.Find("RollDisplay").GetComponent<TMP_Text>();
        healthBar = transform.Find("HealthBar").GetComponent<Slider>();
        enemyHits = transform.Find("Hits").gameObject;
        hitObj = enemyHits.transform.GetComponentsInChildren<Transform>();
    }

    private void Start()
    {
        _RollButton.onClick.AddListener(DiceManager.instance.RollAllDice);
    }

    public void UpdatePlayerHealth()
    {
        healthBar.value = GameManager.PlayerHealth;
    }

    public void UpdateEnemyHits(EnemyDice enemy)
    {
        for (int i = 0; i < hitObj.Length; ++i)
        {
            hitObj[i].gameObject.SetActive(i < enemy.Hits + 2);
        }
    }


}
