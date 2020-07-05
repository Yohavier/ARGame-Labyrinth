using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class MediPackPowerUp : PowerUpBase
{
    public override void OnUse(Player player, PowerUpSlot slot)
    {
        CrewMember dyingPlayer = CanUse(player.positionTile);
        if(dyingPlayer != null)
        {
            AkSoundEngine.PostEvent("powerUp_medipack", gameObject);
            dyingPlayer.GetHealed();
            NetworkClient.instance.SendPlayerHealed(dyingPlayer.playerIndex);
            slot.DropEverythingInSlot();
        }
    }

    public override void ReverseOnDrop(Player player) {}

    private CrewMember CanUse(Tile tile)
    {
        CrewMember[] playersToHeal = tile.GetComponentsInChildren<CrewMember>();
        foreach(CrewMember crew in playersToHeal)
        {
            if(crew.playerState == PlayerState.DYING)
            {
                return crew;
            }
        }
        return null;
    }

}
