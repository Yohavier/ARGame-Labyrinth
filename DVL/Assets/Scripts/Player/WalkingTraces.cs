﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UIElements;

public class WalkingTraces : MonoBehaviour
{
    public GameObject FootstepTrace;

    public void SpawnParticlesystem(Tile tile)
    {
        if (tile != null)
        {
            if (LocalGameManager.instance.activePlayer != null)
            {
                if (VisibilityCheck(tile))
                {                    
                    GameObject trace = Instantiate(FootstepTrace);
                    trace.transform.localPosition = tile.transform.localPosition;
                }             
            }
        }
    }

    private bool VisibilityCheck(Tile tile)
    {
        if (!tile.isInFOW || !BoardGrid.instance.grid.Contains(tile))
            return false;

        if (DetectDirectNeighbours.DetectTileRadius(tile,LocalGameManager.instance.activePlayer.GetComponent<Player>().footstepDetectionRadius, false).Contains(LocalGameManager.instance.activePlayer.GetComponent<Player>().positionTile))
            return true;
        else
            return false;
    }
}
