using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceSide : MonoBehaviour
{
    private bool onGround;
    public bool OnGround => onGround;
    [SerializeField] private int sideValue;

    public int SideValue => sideValue;


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            onGround = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            onGround = false;
        }
    }
}
