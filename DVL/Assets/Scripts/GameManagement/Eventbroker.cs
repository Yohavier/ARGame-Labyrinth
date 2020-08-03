
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Assets.Scripts.Player;

public class Eventbroker : MonoBehaviour
{
    public static Eventbroker instance;
    public Eventbroker()
    {
        instance = this;
    }
    
    public event Action onNotifyNextTurn;
    public void NotifyNextTurn()
    {
        if(onNotifyNextTurn!=null)
        {
            onNotifyNextTurn();
        }
    }

    public event Action<GameState> onChangeGameState;
    public void ChangeGameState(GameState state)
    {
        if (onChangeGameState != null)
        {
            onChangeGameState(state);
        }
    }
    #region CharacterSelection
    public event Action<RoleIndex> onChangeCharacter;
    public void ChangeCharacter(RoleIndex index)
    {
        if (onChangeCharacter != null)
        {
            onChangeCharacter(index);
        }
    }
    public event Action<bool> onToggleGate;
    public void ToggleGate(bool toggle)
    {
        if (onToggleGate != null) 
        {
            onToggleGate(toggle);
        }
    }
    #endregion
}