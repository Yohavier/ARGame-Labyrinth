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
                        StartCoroutine(InitKillPlayer(crew));
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

    private IEnumerator InitKillPlayer(CrewMember crew)
    {
        while (Vector3.Distance(crew.transform.position, transform.position) > 0.05f) 
        {
            yield return null;
        }

        pauseMovement = true;
        anim.SetTrigger("attack");
        yield return new WaitForSeconds(1f);
        pauseMovement = false;
        anim.SetBool("walk", true);
        KillPlayer(crew);
    }
}
