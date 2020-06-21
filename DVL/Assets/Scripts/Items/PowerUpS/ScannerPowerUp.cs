using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScannerPowerUp : PowerUpBase
{
    public override void OnUse(Player player, PowerUpSlot slot)
    {
        player.fogOfWarRadius++;
        player.GetComponent<FogOfWar>().OnChangePlayerPosition(player.positionTile);
        slot.GetComponent<Button>().interactable = false;
    }
}
