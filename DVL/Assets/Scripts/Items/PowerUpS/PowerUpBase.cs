using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public abstract class PowerUpBase : MonoBehaviour
{
    public GameObject powerUpPrefab;
    public Sprite powerUpImage;
    public bool pickedUp;
    public bool isInUse;

    public abstract void OnUse(Player player, PowerUpSlot slot);

    //TODO: Sync so everybody knows that player looses fow PowerUp or smth
    public abstract void ReverseOnDrop(Player player);
}

