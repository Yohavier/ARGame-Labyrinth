using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{
    public List<GameObject> FogOfWarItems = new List<GameObject>();
    public static FogOfWar fow;
    private void Awake()
    {
        fow = this;
        Eventbroker.eventbroker.onSignUpForFogOfWar += SignGameObjectUpForFogOfWar;
    }

    private void SignGameObjectUpForFogOfWar(GameObject gameitem)
    {
        FogOfWarItems.Add(gameitem);
    }
}
