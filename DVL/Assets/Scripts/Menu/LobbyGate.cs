﻿using Assets.Scripts.GameManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyGate : MonoBehaviour
{
    public AnimationClip closeGate;
    public AnimationClip openGate;
    private Animation anim;

    private void Awake()
    {
        anim = GetComponent<Animation>();
        GetComponentInChildren<Canvas>().worldCamera = Camera.main;
    }

    public void InitCharSelection()
    {
        Eventbroker.instance.ToggleGate(true);
        InformationPanel.instance.OnPlayerRoleChanged(0);
        HandleButton();
    }

    public void OnToggleGate(bool toggle)
    {
        if (toggle)
        {
            anim.clip = openGate;
        }
        else
        {
            anim.clip = closeGate;
        }
        anim.Play();
    }

    private void HandleButton()
    {
        GetComponentInChildren<Button>().gameObject.SetActive(false);
        SwipeManager.instance.canSwipe = true;
        GUIManager.instance.readyToggle.interactable = true;
    }
}
