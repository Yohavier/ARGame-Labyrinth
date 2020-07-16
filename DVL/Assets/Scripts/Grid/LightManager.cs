using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class LightManager : MonoBehaviour //TODO: Merge
{
    public static LightManager instance;
    public GameObject lightPrefab;

    private void Awake()
    {
        instance = this;
    }

    public void SetupLights()
    {
        for (int i = 0; i < BoardGrid.instance.grid.Count; i++)
        {
            float seed = BoardGrid.instance.seedList[i];
            if (seed >= 0.25f)
                AddLightToTile(BoardGrid.instance.grid[i], (seed - 0.25f) * 1.33333333333f);
        }
    }

    public void AddLightToTile(Tile tile, float seed)
    {
        LightType type = seed >= 0.75f ? LightType.ltFlicker : LightType.ltStatic;
        GameObject lightObject = Instantiate(lightPrefab);
        lightObject.transform.position = tile.transform.position;
        float distance = Math.Abs(0.5f - seed) * 2f;
        float offsetAngle = (seed >= 0.5f ? 1 : -1) * 360 * distance;
        float distanceScaled = 0.025f * distance;
        lightObject.transform.position += new Vector3(distanceScaled, 0.06f, distanceScaled);
        lightObject.transform.RotateAround(tile.transform.position, tile.transform.up, offsetAngle);
        DynamicLight dynamicLight = lightObject.AddComponent<DynamicLight>();
        Light light = lightObject.GetComponent<Light>();

        if (type == LightType.ltFlicker)
            light.color = Color.magenta;

        dynamicLight.type = type;
        dynamicLight.lightObject = lightObject;
        dynamicLight.light = light;
        lightObject.transform.SetParent(tile.transform);
        tile.isInFOW = tile.isInFOW;
    }
}