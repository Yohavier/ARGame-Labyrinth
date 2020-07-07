using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShutDownPowerUp : PowerUpBase
{
    public override void OnUse(Player player, PowerUpSlot slot)
    {
        slot.DropEverythingInSlot();
        BoardGrid.instance.ShutDownGridPowerUp();
    }

    public override void ReverseOnDrop(Player player) { }
}
