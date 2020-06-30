using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHackPowerUp : PowerUpBase
{
    public override void OnUse(Player player, PowerUpSlot slot)
    {
        if (CanUse(player.positionTile))
        {
            player.positionTile.ToggleDoors();
            NetworkClient.instance.SendDoorHackUsed(player.positionTile);
            slot.DropEverythingInSlot();
        }
    }

    public override void ReverseOnDrop(Player player) { }

    private bool CanUse(Tile tile)
    {
        if (tile.TileContainsDoor())
            return true;
        else
            return false;
    }
}
