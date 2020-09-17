using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_BoardEnv : MonoBehaviour
{
    public Transform board;

    void Update()
    {
        if (board != null)
        {
            this.transform.position = board.position;
            this.transform.rotation = board.rotation;
            this.transform.localScale = board.localScale;
        }
    }
}
