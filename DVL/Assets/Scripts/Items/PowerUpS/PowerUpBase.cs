using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public abstract class PowerUpBase : MonoBehaviour
{
    public GameObject powerUpPrefab;
    public Sprite powerUpImage;
    public bool pickedUp;

    public abstract void OnUse(Player player, PowerUpSlot slot);
}

