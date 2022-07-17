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

    [SerializeField] protected int diceValue = -1;
   public int DiceValue => diceValue;

    private DiceSide[] _sides;

    // Start is called before the first frame update
    public virtual void Start()
    {
        if (_rigidbody == null)
        {
            Initialize();
            _rigidbody.useGravity = false;
        }
    }

    void Initialize()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _initialPos = transform.position;
        _sides = GetComponentsInChildren<DiceSide>();
    }

    private void Update()
    {
        //if has landed correctly
        if (_rigidbody.IsSleeping() && !Landed && _thrown)
        {
            Landed = true;
            DiceManager.Landed();
            ParticleManager.Intsance.LandBurst(transform.position);
            _rigidbody.useGravity = false;
            SideValueCheck();
        }
        
        //if has landed incorrectly and not on a side
        if (_rigidbody.IsSleeping() && Landed && diceValue == -1)
        {
            Reset();
            RollDice();
        }
    }

    public void RollDice()
    {
        if (!_thrown && !Landed)
        {
            diceValue = -1;
            _thrown = true;
            if (_rigidbody == null)
            {
                Initialize();
            }

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

    protected virtual int SideValueCheck()
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

    private void OnCollisionEnter(Collision collision)
    {
        if (_thrown)
        {
            ParticleManager.Intsance.HitBurst(collision.GetContact(0).point);
        }
    }
}