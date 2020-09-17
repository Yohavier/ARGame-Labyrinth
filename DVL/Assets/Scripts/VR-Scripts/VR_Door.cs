using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_Door : MonoBehaviour
{
    VR_Tile parentTile;
    private void Start()
    {
        parentTile = transform.parent.GetComponent<VR_Tile>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Controller"))
        {
            Debug.Log("ff");
            parentTile.ToggleDoors();
        }
    }
}
