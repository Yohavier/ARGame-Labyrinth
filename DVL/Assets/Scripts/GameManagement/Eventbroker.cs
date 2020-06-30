
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Eventbroker : MonoBehaviour
{
    public static Eventbroker instance;
    public Eventbroker()
    {
        instance = this;
    }
    
    public event Action onNotifyNextTurn;
    public void NotifyNextTurn()
    {
        if(onNotifyNextTurn!=null)
        {
            onNotifyNextTurn();
        }
    }
}