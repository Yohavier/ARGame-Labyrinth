using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UIElements;

public class WalkingTraces : MonoBehaviour
{
    public ParticleSystem FootstepTrace;

    public void SpawnParticlesystem(Tile tile)
    {
        if (tile != null)
        {
            if (GameManager.instance.activePlayer != null)
            {
                if (VisibilityCheck(tile))
                {                    
                    Instantiate(FootstepTrace, transform.position, Quaternion.identity);
                    AkSoundEngine.SetSwitch("character", "trace", gameObject);
                    AkSoundEngine.PostEvent("character_footstep", gameObject);
                }             
            }
        }
    }

    private bool VisibilityCheck(Tile tile)
    {
        if (!tile.isInFOW || !BoardGrid.instance.grid.Contains(tile))
            return false;

        if (DetectDirectNeighbours.DetectTileRadius(tile,GameManager.instance.activePlayer.GetComponent<Player>().footstepDetectionRadius, false).Contains(GameManager.instance.activePlayer.GetComponent<Player>().positionTile))
            return true;
        else
            return false;
    }
}
