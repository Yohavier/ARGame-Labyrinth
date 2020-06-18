using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommunicatorPowerUp : PowerUp
{
    private GameObject playerForCommunication;
    public override void OnUse()
    {
        playerForCommunication = GetOpenPlayer();

        if (playerForCommunication)
        {
            FogOfWar otherFOW = playerForCommunication.GetComponent<FogOfWar>();
            otherFOW.enabled = true;
            otherFOW.OnChangePlayerPosition(playerForCommunication.GetComponent<Player>().positionTile, true);
            Eventbroker.instance.onNotifyNextTurn += NotifyNextTurn;
        }
    }

    private void NotifyNextTurn()
    {
        Eventbroker.instance.onNotifyNextTurn -= NotifyNextTurn;
        playerForCommunication.GetComponent<FogOfWar>().enabled = false;
        Destroy(this.gameObject);
    }

    private GameObject GetOpenPlayer()
    {
        foreach (GameObject player in GameManager.instance.allPlayers)
        {
            if (player.GetComponent<CrewMember>())
            {
                if (player != LocalGameManager.instance.activePlayer)
                {
                    return player;
                }
            }
        }
        return null;
    }
    protected override bool CanUse()
    {
        return true;
    }
}
