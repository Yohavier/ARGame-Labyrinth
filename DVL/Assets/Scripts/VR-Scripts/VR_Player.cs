using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_Player : MonoBehaviour
{
    public VR_Tile positionTile;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<VR_Tile>() != null)
        {
            other.GetComponent<VR_Tile>().playerPos = this;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<VR_Tile>() != null)
        {
            other.GetComponent<VR_Tile>().playerPos = null;
        }
    }
}
