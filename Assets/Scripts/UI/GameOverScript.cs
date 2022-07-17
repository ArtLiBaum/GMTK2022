using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.Find("RestartButton").GetComponent<Button>().onClick.AddListener(GameManager.ReloadScene);
    }
}
