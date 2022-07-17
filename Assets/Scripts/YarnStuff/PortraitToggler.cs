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

    public Image LeftPortrait;
    public Image RightPortrait;

    public List<StringSpritePair> Pairs;

    private void Awake()
    {
        Runner.AddCommandHandler<string, bool>("Character", ChangePortrait);

        LeftPortrait.sprite = Pairs[0].MySprite;
    }


    public void ChangePortrait(string character, bool left)
    {
        LeftPortrait.gameObject.SetActive(left);
        RightPortrait.gameObject.SetActive(!left);

        List<string> pairStrings = Pairs.Select(x => x.CharacterName).ToList();

        if (pairStrings.Contains(character))
        {
            if (left)
            {
                LeftPortrait.sprite = Pairs[pairStrings.IndexOf(character)].MySprite;
            }
            else
            {
                RightPortrait.sprite = Pairs[pairStrings.IndexOf(character)].MySprite;
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