
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Eventbroker : MonoBehaviour
{
    public static Eventbroker eventbroker;
    public Eventbroker()
    {
        eventbroker = this;
    }
}