using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_Door : MonoBehaviour
{
    public Animation anim;

    public AnimationClip open;
    public AnimationClip close;
    private bool isOpen;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Controller"))
        {
            if (isOpen)
                anim.clip = close;
            else
                anim.clip = open;

            isOpen = !isOpen;
            anim.Play();
        }
    }
}
