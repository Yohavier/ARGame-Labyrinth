using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    public GameObject Player;
    private int playerCount;
    public void SpawnPlayersInCorner(Tile cornerTile)
    {
        playerCount++;
        var player = Instantiate(Player);
        player.GetComponent<Player>().SetUpPlayer(playerCount);
        player.transform.SetParent(cornerTile.transform);
        player.transform.localPosition = new Vector3(0, 1, 0);
    }
}
