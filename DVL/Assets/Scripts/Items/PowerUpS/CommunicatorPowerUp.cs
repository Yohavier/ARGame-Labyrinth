using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommunicatorPowerUp : PowerUp
{
    public GameObject targetForCommunication;
    public override void OnUse()
    {
        targetForCommunication = GetOpenPlayer();

        if (targetForCommunication)
        {
            LocalGameManager.instance.activePlayer.GetComponent<Player>().communicatorPowerUp = this;
            Eventbroker.instance.onNotifyNextTurn += NotifyNextTurn;
        }
    }

    private void NotifyNextTurn()
    {
        Eventbroker.instance.onNotifyNextTurn -= NotifyNextTurn;
        LocalGameManager.instance.activePlayer.GetComponent<Player>().communicatorPowerUp = null;
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
