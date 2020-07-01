using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScannerPowerUp : PowerUpBase
{
    public int buffFOWRadius;
    public override void OnUse(Player player, PowerUpSlot slot)
    {
        isInUse = true;
        player.fogOfWarRadius += buffFOWRadius;
        player.GetComponent<FogOfWar>().OnChangePlayerPosition(player.positionTile);
        slot.GetComponent<Button>().interactable = false;
    }

    public override void ReverseOnDrop(Player player)
    {
        player.fogOfWarRadius -= buffFOWRadius;
        player.GetComponent<FogOfWar>().OnChangePlayerPosition(player.positionTile);
    }
}
