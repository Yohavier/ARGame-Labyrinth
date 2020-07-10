using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Player;

public class SpawnPlayer : MonoBehaviour
{
	public GameObject[] PlayerRoles;

	private int playerCount;

	//receives list of 4 corner tiles and places players there
	public void SpawnPlayersInCorner(List<Tile> cornerTiles)
	{
		SO_PlayerClass role = null;
		foreach(Tile corner in cornerTiles)
		{
			playerCount++;
			if (NetworkClient.instance.networkPlayers[playerCount - 1].roleIndex > RoleIndex.Invalid) 
			{
				role = InformationPanel.instance.playerRoles[(int)NetworkClient.instance.networkPlayers[playerCount - 1].roleIndex];
				foreach(GameObject playerRole in PlayerRoles)
                {
					if(playerRole.GetComponent<Player>().playerRole.roleIndex == role.roleIndex)
                    {
						GameObject player = Instantiate(playerRole);
						player.GetComponent<Player>().SetUpPlayer(playerCount);
						player.transform.SetParent(corner.transform);
						player.GetComponent<Player>().ChangePlayerPosition(corner);
						player.transform.localPosition = Vector3.zero;
						GameManager.instance.allPlayers.Add(player);
					}
                }
			}
            else
            {
				GameObject player = Instantiate(PlayerRoles[1]);
				player.GetComponent<Player>().SetUpPlayer(playerCount);
				player.transform.SetParent(corner.transform);
				player.GetComponent<Player>().ChangePlayerPosition(corner);
				player.transform.localPosition = Vector3.zero;
				GameManager.instance.allPlayers.Add(player);
			}
		}
	}
}
