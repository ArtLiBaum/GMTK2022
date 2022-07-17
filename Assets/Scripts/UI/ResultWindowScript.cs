using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Tilemaps;

public class ResultWindowScript : MonoBehaviour
{
    [SerializeField] private TMP_Text Title, Description;


    public void OpenBox(string title, string description)
    {
        Title.text = title;
        Description.text = description;
    }
}
