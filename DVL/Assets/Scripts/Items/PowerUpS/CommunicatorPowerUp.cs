using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommunicatorPowerUp : PowerUpBase
{
    [HideInInspector] public GameObject targetForCommunication;
    [HideInInspector] public Player playerWhoActivated;
    public override void OnUse(Player player, PowerUpSlot slot)
    {
        playerWhoActivated = player;
        targetForCommunication = GetOpenPlayer();

        if (targetForCommunication)
        {
            AkSoundEngine.PostEvent("powerUp_communicator", gameObject);
            isInUse = true;
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
                if (player != GameManager.instance.activePlayer)
                {
                    return player;
                }
            }
        }
        return null;
    }

    public override void ReverseOnDrop(Player player) { }
}
