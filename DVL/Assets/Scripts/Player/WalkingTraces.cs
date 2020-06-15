using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WalkingTraces : MonoBehaviour
{
    [SerializeField] GameObject FootstepTrace;

    public void SpawnParticlesystem(Tile tile)
    {       
        if (VisibilityCheck(tile))
        {
            GameObject trace = Instantiate(FootstepTrace);
            trace.transform.localPosition = tile.transform.localPosition;
        } 
    }

    private bool VisibilityCheck(Tile tile)
    {
        if (!tile.isInFOW)
            return false;
        else
            return true;
    }
}
