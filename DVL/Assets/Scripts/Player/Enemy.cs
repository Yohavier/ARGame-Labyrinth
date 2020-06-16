using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Player
{
    //Check for crew members
    public override bool CheckForOtherPlayers(Tile nextTile)
    {
		CrewMember crewMember = nextTile.GetComponentInChildren<CrewMember>();

		if (crewMember != null)
        {
			if(crewMember.playerState == PlayerState.ALIVE)
            {
				KillPlayer(crewMember);
			}
        }
        return true;
    }

	public void KillPlayer(CrewMember crewMember)
	{
		crewMember.playerState = PlayerState.DYING;
	}
}
