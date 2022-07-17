using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    [SerializeField]
    private FMODUnity.StudioEventEmitter UIClick, DiceCollision, Fail, Success, Select, Roar, PlayerHit;
    // Start is called before the first frame update

    private void Start()
    {
        if (instance)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    public void PlayFail()
    {
        Fail.Play();
    }
    public void PlayDiceCollision()
    {
        DiceCollision.Play();
    }
    
    public void PlayUIClick()
    {
        UIClick.Play();
    }
    public void PlaySuccess()
    {
        Success.Play();
    }
    public void PlaySelect()
    {
        Select.Play();
    }
    public void PlayRoar()
    {
        Roar.Play();
    }
    public void PlayPlayerHit()
    {
        PlayerHit.Play();
    }
    
}
