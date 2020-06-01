using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Player
{
    public override bool checkForOtherPlayers(Tile nextTile)
    {  
        if (nextTile.GetComponentInChildren<Player>() != null)
        {
            GameManager.GameManagerInstance.KillPlayer(nextTile.GetComponentInChildren<Player>());
            return true;
        }
        return true;
    }
}
