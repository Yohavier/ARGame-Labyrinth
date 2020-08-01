using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Player
{
    //Check for crew members
    public override bool CheckForOtherPlayers(Tile nextTile)
    {
		var crewMembers = nextTile.GetComponentsInChildren<CrewMember>();

		if (crewMembers != null)
        {
            if (GameManager.instance.GetTurn())
            {
                foreach (CrewMember crew in crewMembers)
                {
                    if (crew.playerState == PlayerState.ALIVE)
                    {
                        KillPlayer(crew);
                    }
                }
            }
        }
        return true;
    }

	public void KillPlayer(CrewMember crewMember)
	{
        NetworkClient.instance.SendPlayerKilled(crewMember.playerIndex);
		crewMember.playerState = PlayerState.DYING;
	}
}
