using UnityEngine;
using System.Collections.Generic;
//Global stuff for all players 
public class GameManager : MonoBehaviour
{
	public static GameManager GameManagerInstance;

	private int capsuleCount;
	public List<GameObject> allPlayers = new List<GameObject>();

	private void Awake()
	{
		GameManagerInstance = this;
	}

	public void CheckWinConditionCrew()
	{
		capsuleCount++;
		InformationPanel.playerPanel.SetProgressText(capsuleCount.ToString());
		if (capsuleCount == 4)
		{
			Debug.Log("Crew wins");
		}
	}
	public void CheckWinConditionMonster()
	{
		if (allPlayers.Count == 0)
		{
			Debug.Log("Monster wins");
		}
	}

	public void KillPlayer(Player player)
	{
		foreach (GameObject g in allPlayers)
		{
			if (player.gameObject == g)
			{
				if (player.storedItem != null)
				{
					player.storedItem.transform.SetParent(player.positionTile.transform);
					if (!player.positionTile.isInFOW)
					{
						player.storedItem.GetComponent<MeshRenderer>().enabled = true;
						FogOfWar.fow.activeFogOfWarItems.Add(player.storedItem);
					}
				}
				FogOfWar.fow.activeFogOfWarItems.Remove(player.gameObject);
				CheckWinConditionMonster();
				Destroy(player.gameObject);
			}
		}
	}
}
