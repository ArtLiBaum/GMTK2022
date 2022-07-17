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

    public GameObject EndCard;

    public List<StringSpritePair> Pairs;

    private void Awake()
    {
        Runner.AddCommandHandler<string, bool, bool>("Character", ChangePortrait);
        Runner.AddCommandHandler("DoEndCard", ActivateEndCard);

        LeftPortrait.sprite = Pairs[0].MySprite;
    }


    public void ChangePortrait(string character, bool left, bool flipped = false)
    {
        LeftPortrait.gameObject.SetActive(left);
        RightPortrait.gameObject.SetActive(!left);

        List<string> pairStrings = Pairs.Select(x => x.CharacterName).ToList();

        if (pairStrings.Contains(character))
        {
            if (left)
            {
                LeftPortrait.sprite = Pairs[pairStrings.IndexOf(character)].MySprite;
                LeftPortrait.rectTransform.localRotation = flipped ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
            }
            else
            {
                RightPortrait.sprite = Pairs[pairStrings.IndexOf(character)].MySprite;
                RightPortrait.rectTransform.localRotation = flipped ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
            }
        }

    }

    public void ActivateEndCard()
    {
        EndCard.SetActive(true);
    }

}


[System.Serializable]
public class StringSpritePair
{
    public string CharacterName;
    public Sprite MySprite;
}