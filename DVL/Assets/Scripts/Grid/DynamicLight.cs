using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class DynamicLight : MonoBehaviour //TODO: Merge
{
    public LightType type;
    public Light light;
    public GameObject lightObject;
    public Tile tile;
    public float defaultIntensity = 1.5f;
    private bool inFlicker = false;
    private float flickerLength = 0;
    private float flickerTime;

    private void Update()
    {
        switch (type)
        {
            case LightType.ltFlicker:
                DoFlicker();
                break;
        }
    }

    private void DoFlicker()
    {
        if (!inFlicker)
        {
            float flickerSeed = Random.Range(0.1f, 0.8f);
            bool shouldFlicker = flickerSeed >= 0.15f;

            if (!shouldFlicker)
                return;

            inFlicker = true;
            flickerTime = 0;
            light.intensity = defaultIntensity;
            flickerLength = flickerSeed;
        }

        else
        {
            flickerTime += Time.deltaTime;
            float transition = defaultIntensity * flickerTime / flickerLength;
            light.intensity = defaultIntensity - transition;

            if (flickerTime >= flickerLength)
            {
                inFlicker = false;
            }
        }
        /*bool shouldFlicker = Random.Range(0f, 1f) >= 0.9f;
        if (shouldFlicker)
        {
            light.intensity = 0f;
        }

        else
            light.intensity = 1f;*/
    }
}
