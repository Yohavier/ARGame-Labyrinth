
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

    public event Action<GameObject> onSignUpForFogOfWar;
    public void SignUpForFogOfWar(GameObject gameItem)
    {
        if (onSignUpForFogOfWar != null)
        {
            onSignUpForFogOfWar(gameItem);
        }
    }
}