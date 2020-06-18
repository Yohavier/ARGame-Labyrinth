﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public abstract class PowerUp : MonoBehaviour
{
    public GameObject powerUpPrefab;
    public Sprite powerUpImage;

    public abstract void OnUse();
    protected abstract bool CanUse();
}

