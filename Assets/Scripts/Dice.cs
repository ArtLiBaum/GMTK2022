using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Dice : MonoBehaviour
{
    private Rigidbody _rigidbody;

    private Vector3 _initialPos;
    private bool _thrown;

    public bool Landed { get; private set; }

    [SerializeField] private int diceValue;
   public int DiceValue => diceValue;

    private DiceSide[] _sides;

    private void OnEnable() => DiceManager.allDice.Add(this);
    private void OnDisable() => DiceManager.allDice.Remove(this);

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _initialPos = transform.position;
        _rigidbody.useGravity = false;
        _sides = GetComponentsInChildren<DiceSide>();
    }

    private void Update()
    {
        //if has landed correctly
        if (_rigidbody.IsSleeping() && !Landed && _thrown)
        {
            Landed = true;
            DiceManager.Landed();
            _rigidbody.useGravity = false;
            SideValueCheck();
        }
        
        //if has landed incorrectly and not on a side
        if (_rigidbody.IsSleeping() && Landed && diceValue == 0)
        {
            Reset();
            RollDice();
        }
    }

    public void RollDice()
    {
        if (!_thrown && !Landed)
        {
            _thrown = true;
            _rigidbody.useGravity = true;
            _rigidbody.AddTorque(Random.Range(0,500),Random.Range(0,500),Random.Range(0,500));
        }
    }

    public void Reset()
    {
        transform.position = _initialPos;
        _thrown = Landed = false;
        _rigidbody.useGravity = false;
    }

    private int SideValueCheck()
    {
        diceValue = 0;
        foreach (var side in _sides)
        {
            if (!side.OnGround) continue;
            diceValue = side.SideValue;
            return diceValue;
        }

        return 0;
    }
}