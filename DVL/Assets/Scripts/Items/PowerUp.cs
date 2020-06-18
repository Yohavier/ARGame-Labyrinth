using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    private void OnCollect()
    {

    }
    private void OnDrop()
    {

    }
    private bool CanStore()
    {
        return true;
    }

    protected virtual void OnUse() { }
    protected virtual bool CanUse() { return true; }
}
