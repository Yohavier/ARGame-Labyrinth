using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WalkingTraces : MonoBehaviour
{
    [SerializeField] GameObject FootstepTrace;

    public void SpawnParticlesystem(Tile tile)
    {
        if (tile != null)
        {
            if (LocalGameManager.instance.activePlayer != null)
            {
                if (!tile.isInFOW)
                    return;

                Tile A = LocalGameManager.instance.activePlayer.GetComponent<Player>().positionTile;
                Tile B = tile;

                float distance = (A.transform.position - B.transform.position).sqrMagnitude;
                Debug.Log(distance);

                if (distance < 0.1)
                {
                    GameObject trace = Instantiate(FootstepTrace);
                    trace.transform.localPosition = tile.transform.localPosition;
                }
            }
        }
    }
}
