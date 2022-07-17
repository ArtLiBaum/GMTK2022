using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyDisplay : MonoBehaviour
{
    private Image portrait;
    private TMP_Text enemyName;
    private GameObject hits;
    private TMP_Text tko;
    private TMP_Text toughness;

    private void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        portrait = transform.Find("Image").GetComponent<Image>();
        enemyName = transform.Find("Name").GetComponent<TMP_Text>();
        hits = transform.Find("Stats/Hits").gameObject;
        toughness = transform.Find("Stats/Toughness").GetComponent<TMP_Text>();
        tko = transform.Find("Stats/TKO").GetComponent<TMP_Text>();
    }


    public void LoadDisplay(EnemyDice enemy)
    {
        if(portrait == null) Initialize();
        portrait.sprite = enemy.Portrait;
        enemyName.text = enemy.EnemyName;
        toughness.text = "TOUGHNESS: " + enemy.Toughness;
        tko.text = "TKO: " +enemy.TKO;

        Transform[] hitObj = hits.transform.GetComponentsInChildren<Transform>();
        for (int i = 0; i < hitObj.Length; ++i)
        {
            hitObj[i].gameObject.SetActive(i < enemy.Hits + 2);
        }
    }
}