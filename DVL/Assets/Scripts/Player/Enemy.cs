using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Player
{
    //Check for crew members
    List<CrewMember> prevVisibleCrewMember = new List<CrewMember>();
    public ParticleSystem exceptionMark;

    public override bool CheckForOtherPlayers(Tile nextTile)
    {
		CrewMember[] crewMembers = nextTile.GetComponentsInChildren<CrewMember>();

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
    protected override void CheckNewSurrounding(Tile tile)
    {
        List<Tile> tiles = DetectDirectNeighbours.DetectTileRadius(tile, fogOfWarRadius, true);
        List<CrewMember> newVisibleCrewMember = new List<CrewMember>();
        for (int i = 0; i < tiles.Count; i++)
        {
            CrewMember crew = tiles[i].GetComponentInChildren<CrewMember>();
            if(crew != null)
            {
                newVisibleCrewMember.Add(crew);
                if (!prevVisibleCrewMember.Contains(crew))
                {
                    stopMovement = true;
                    if (exceptionMark != null)
                    {
                        var a = Instantiate(exceptionMark);
                        a.transform.SetParent(this.transform);
                        a.transform.localPosition = new Vector3(0, 6, 0);
                        a.transform.localScale = new Vector3(1, 1, 1);
                    }
                }
            }
        }
        prevVisibleCrewMember = newVisibleCrewMember;
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
