using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Create ShutDown
public class ShutDownPowerUp : PowerUpBase
{
    public override void OnUse(Player player, PowerUpSlot slot)
    {
        foreach(Tile tile in BoardGrid.instance.grid)
        {
            //Set new Random Position
            tile.ToggleDoors(false);
            slot.DropEverythingInSlot();
        }
    }

    public override void ReverseOnDrop(Player player) { }
}
