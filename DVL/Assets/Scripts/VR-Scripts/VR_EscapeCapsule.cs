using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_EscapeCapsule : MonoBehaviour
{
    public int counter;
    public void RepairEscape()
    {
        counter++;
        if(counter == 4)
        {
            Debug.Log("Win");
        }
    }
}
