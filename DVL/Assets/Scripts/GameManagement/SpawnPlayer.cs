using UnityEngine;
using System.Collections.Generic;

public class SpawnPlayer : MonoBehaviour
{
	public GameObject[] Player;

	private int playerCount;

	//receives list of 4 corner tiles and places players there
	public void SpawnPlayersInCorner(List<Tile> cornerTiles)
	{
		foreach(Tile corner in cornerTiles)
		{
			playerCount++;
			GameObject player = Instantiate(Player[playerCount-1]);
			player.GetComponent<Player>().SetUpPlayer(playerCount);
			player.transform.SetParent(corner.transform);
			player.GetComponent<Player>().ChangePlayerPosition(corner);
			player.transform.localPosition = Vector3.zero;
			GameManager.instance.allPlayers.Add(player);
		}
	}
}
