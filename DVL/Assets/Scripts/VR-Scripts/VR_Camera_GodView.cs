using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_Camera_GodView : MonoBehaviour
{
    private Camera godCam;
    private void Start()
    {
        godCam = GetComponent<Camera>();
    }
    private void Update()
    {
        if (Display.displays[0].active)
            Debug.Log("f");
    }
}
