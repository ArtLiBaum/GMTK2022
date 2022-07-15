using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Dice : MonoBehaviour
{
    private Rigidbody _rigidbody;

    private Vector3 _initialPos;
    private bool thrown, landed;

    public bool Landed => landed;

   [SerializeField] private int diceValue;

    private DiceSide[] sides; 

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _initialPos = transform.position;
        _rigidbody.useGravity = false;
        sides = GetComponentsInChildren<DiceSide>();
    }

    private void Update()
    {
        //if has landed correctly
        if (_rigidbody.IsSleeping() && !landed && thrown)
        {
            landed = true;
            _rigidbody.useGravity = false;
            SideValueCheck();
        }
        
        //if has landed incorrectly and not on a side
        if (_rigidbody.IsSleeping() && landed && diceValue == 0)
        {
            Reset();
            RollDice();
        }
    }

    public void RollDice()
    {
        if (!thrown && !landed)
        {
            thrown = true;
            _rigidbody.useGravity = true;
            _rigidbody.AddTorque(Random.Range(0,500),Random.Range(0,500),Random.Range(0,500));
        }
    }

    public void Reset()
    {
        transform.position = _initialPos;
        thrown = landed = false;
        _rigidbody.useGravity = false;
    }

    public int SideValueCheck()
    {
        diceValue = 0;
        foreach (var side in sides)
        {
            if (side.OnGround)
            {
                diceValue = side.SideValue;
                return diceValue;
            }
        }

        return 0;
    }
}