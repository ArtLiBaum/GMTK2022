using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;
using UnityEngine.UI;
using System.Linq;

public class PortraitToggler : MonoBehaviour
{

    public DialogueRunner Runner;

    public Image MainCharPortrait;
    public Image SecondaryCharPortrait;

    public List<StringSpritePair> Pairs;

    private void Awake()
    {
        Runner.AddCommandHandler<string>("Change_Portrait", ChangePortrait);

        MainCharPortrait.sprite = Pairs[0].MySprite;
    }


    public void ChangePortrait(string character)
    {
        bool Me = (character == "Me");

        MainCharPortrait.gameObject.SetActive(Me);
        SecondaryCharPortrait.gameObject.SetActive(!Me);

        if (!Me)
        {
            List<string> pairStrings = Pairs.Select(x => x.CharacterName).ToList();

            if (pairStrings.Contains(character))
            {
                SecondaryCharPortrait.sprite = Pairs[pairStrings.IndexOf(character)].MySprite;
            }
        }

    }

}


[System.Serializable]
public class StringSpritePair
{
    public string CharacterName;
    public Sprite MySprite;
}