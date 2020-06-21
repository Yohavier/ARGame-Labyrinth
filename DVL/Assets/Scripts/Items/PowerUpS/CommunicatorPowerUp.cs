using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommunicatorPowerUp : PowerUpBase  
{
    public GameObject targetForCommunication;
    public Player playerWhoActivated;
    public override void OnUse(Player player, PowerUpSlot slot)
    {
        playerWhoActivated = player;
        targetForCommunication = GetOpenPlayer();

        if (targetForCommunication)
        {
            player.communicatorPowerUp = this;
            slot.GetComponent<Button>().interactable = false;
        }
    }

    private void NotifyNextTurn()
    {
        Eventbroker.instance.onNotifyNextTurn -= NotifyNextTurn;
        playerWhoActivated.communicatorPowerUp = null;
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
}
