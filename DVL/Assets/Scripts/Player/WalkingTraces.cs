using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WalkingTraces : MonoBehaviour
{
    [SerializeField] ParticleSystem FootstepTrace;

    public void SpawnParticlesystem(Tile tile)
    {
        if (VisibilityCheck(tile))
        {
            ParticleSystem trace = Instantiate(FootstepTrace);
            trace.transform.SetParent(tile.transform);
            trace.transform.localPosition = Vector3.zero;
            trace.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            Destroy(trace.gameObject, trace.startLifetime);
        }       
    }

    private bool VisibilityCheck(Tile tile)
    {
        if (tile.isInFOW)
            return false;

        DetectDirectNeighbours n = new DetectDirectNeighbours();
        if (n.Detect(tile).Contains(LocalGameManager.instance.activePlayer.GetComponent<Player>().positionTile))
            return true;
        else
            return false;
    }
}
