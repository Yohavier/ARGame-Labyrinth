using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScannerPowerUp : PowerUpBase
{
    public int buffFOWRadius;

    public override void OnUse(Player player, PowerUpSlot slot)
    {
        if (isInUse)
            return;

        AkSoundEngine.PostEvent("powerUp_scanner", gameObject);
        isInUse = true;
        player.fogOfWarRadius += buffFOWRadius;
        player.GetComponent<FogOfWar>().OnChangeFoWPosition(player.positionTile);
        slot.GetComponent<Button>().interactable = false;
    }

    public override void ReverseOnDrop(Player player)
    {
        player.fogOfWarRadius -= buffFOWRadius;
        player.GetComponent<FogOfWar>().OnChangeFoWPosition(player.positionTile);
    }
}
