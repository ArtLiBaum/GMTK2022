using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class BackgroundToggler : MonoBehaviour
{
    private Image background;

    private void Start()
    {
        background = GetComponent<Image>();
        FindObjectOfType<DialogueRunner>().AddCommandHandler < bool>("BackgroundActive",BackgroundActive);
    }

    public void BackgroundActive(bool active)
    {
        background.enabled = active;
    }
}
