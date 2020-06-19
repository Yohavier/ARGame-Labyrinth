using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScannerPowerUp : PowerUp
{
    public override void OnUse()
    {
        var player = LocalGameManager.instance.activePlayer;
        player.GetComponent<Player>().fogOfWarModificator++;
        player.GetComponent<FogOfWar>().OnChangePlayerPosition(player.GetComponent<Player>().positionTile);
        Destroy(this.gameObject);
    }

    protected override bool CanUse()
    {
        return true;
    }
}
