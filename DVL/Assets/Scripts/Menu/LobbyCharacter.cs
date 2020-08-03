using Assets.Scripts.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class LobbyCharacter : MonoBehaviour
{
    private List<LobbySubCharacter> subChars;
    public CharInfo charInfo;

    private void Awake()
    {
        subChars = GetComponentsInChildren<LobbySubCharacter>().ToList();
    }

    public void ChangeSelectedCharacter(RoleIndex newRole)
    {
        for (int i = 0; i < subChars.Count; i++)
        {
            if (subChars[i].playerClass.roleIndex == newRole)
            {
                subChars[i].gameObject.SetActive(true);
                charInfo.DisplayNewInfo(subChars[i].playerClass);
            }
            else
            {
                subChars[i].gameObject.SetActive(false);
            }
        }
    }
}
