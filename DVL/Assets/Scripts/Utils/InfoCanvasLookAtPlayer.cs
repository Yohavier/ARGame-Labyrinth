using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoCanvasLookAtPlayer : MonoBehaviour
{
    public float maxAngle;

    private void Update()
    {
        FaceCamera();
    }

    private void FaceCamera()
    {
        var targetRotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        if (transform.localEulerAngles.x < 25|| targetRotation.eulerAngles.x < 25)
        {      
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 3 * Time.deltaTime);
        }
    }
}
