using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class LobbyRocket : MonoBehaviour
{
    public ParticleSystem rocketBlaster;
    private void Start()
    {
        HandleTrackedImageLib.instance.trackLobby = true;
    }
    public void StartBooster()
    {
        HandleTrackedImageLib.instance.trackLobby = false;
        rocketBlaster.Play();
        StartCoroutine(Boost());
    }
    private IEnumerator Boost()
    {
        yield return new WaitForSeconds(1f);
        float a;
        float t;
        float distance;
        bool loop = true;
        a = 0;
        t = 0;

        while (loop)
        {
            a += t;

            transform.position += rocketBlaster.transform.up * a * 0.01f;
            t += Time.deltaTime * 0.01f;
            distance = Vector3.Distance(Camera.main.transform.position, transform.position);
            if(distance > 50)
            {
                loop = false;
            }
            yield return null;
        }
        this.gameObject.SetActive(false);
    }
}
