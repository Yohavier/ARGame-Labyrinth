using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum playingPlayer { Invalid = -1, Player1, Player2, Player3, Enemy}

public class GameManager : MonoBehaviour
{
    public playingPlayer viewOfPlayer;
    public playingPlayer currentTurnPlayer;
    public static GameManager instance = new GameManager();
    private void Awake()
    {
        instance = this;
        viewOfPlayer = playingPlayer.Invalid;//playingPlayer.Player2;
        currentTurnPlayer = playingPlayer.Invalid;
    }

    public bool GetTurn()
    {
        return viewOfPlayer == currentTurnPlayer && GetInMatch();
    }

    public bool GetInMatch()
    {
        return viewOfPlayer != playingPlayer.Invalid && currentTurnPlayer != playingPlayer.Invalid;
    }
}
