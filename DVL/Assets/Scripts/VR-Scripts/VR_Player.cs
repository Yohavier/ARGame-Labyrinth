using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_Player : MonoBehaviour
{
    public VR_Tile positionTile;
    public VR_ItemPickUp hand1;
    public VR_ItemPickUp hand2;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<VR_Tile>() != null)
        {
            other.GetComponent<VR_Tile>().playerPos = this;
            if (other.GetComponent<VR_EscapeCapsule>() != null)
            {
                HandleEscape(other.GetComponent<VR_EscapeCapsule>());
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<VR_Tile>() != null)
        {
            other.GetComponent<VR_Tile>().playerPos = null;
        }
    }

    private void HandleEscape(VR_EscapeCapsule cap)
    {
        if (hand1.storedItem != null)
        {
            cap.RepairEscape();
            hand1.DropItem();
        }
        if(hand2.storedItem != null)
        {
            cap.RepairEscape();
            hand2.DropItem();
        }
    }
}
