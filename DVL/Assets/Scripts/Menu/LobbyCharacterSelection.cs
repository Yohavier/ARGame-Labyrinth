using Assets.Scripts.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class LobbyCharacterSelection : MonoBehaviour
{
    private Color newColor;
    private Animation anim;
    private void Awake()
    {
        anim = GetComponent<Animation>();
    }
    public void OnChangeSelectedCharacter(RoleIndex newRole)
    {
        switch (newRole)
        {
            case RoleIndex.Juggernaut:
                newColor = Color.white;
                break;
            case RoleIndex.Mechanic:
                newColor = Color.red;
                break;
            case RoleIndex.Runnner:
                newColor = Color.black;
                break;
            case RoleIndex.Scout:
                newColor = Color.blue;
                break;
            case RoleIndex.Standart:
                newColor = Color.yellow;
                break;
            default:
                Debug.LogError("Something went wrong selecting your character");
                return;
        }
        if (anim != null)
        {
            anim.Play();
            InformationPanel.instance.playerRoleMenu.interactable = false;
        }
    }

    public void ChangeCharModel()
    {
        GetComponent<MeshRenderer>().material.color = newColor;
    }

    public void ChangeComplete()
    {
        InformationPanel.instance.playerRoleMenu.interactable = true;
    }
}
