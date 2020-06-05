using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Player
{
    //Check for crew members
    public override bool CheckForOtherPlayers(Tile nextTile)
    {  
        if (nextTile.GetComponentInChildren<CrewMember>() != null)
        {
            GameManager.instance.KillPlayer(nextTile.GetComponentInChildren<CrewMember>());
            return true;
        }
        return true;
    }
}
