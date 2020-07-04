using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticleOnEnd : MonoBehaviour
{
    private void Awake()
    {
        ParticleSystem particle = GetComponent<ParticleSystem>();
        Destroy(this.gameObject, particle.startLifetime);
    }
}
