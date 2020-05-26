using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum playingPlayer { Player1, Player2, Player3, Enemy}

public class GameManager : MonoBehaviour
{
    public playingPlayer viewOfPlayer;
    public static GameManager GameManagerInstance;
    private void Awake()
    {
        GameManagerInstance = this;
        viewOfPlayer = playingPlayer.Player2;
    }
}
