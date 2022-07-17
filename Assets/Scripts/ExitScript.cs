using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitScript : MonoBehaviour
{

    public Button ExitButton;

    // Start is called before the first frame update
    void Awake()
    {
        ExitButton.onClick.AddListener(Application.Quit);
    }
}
