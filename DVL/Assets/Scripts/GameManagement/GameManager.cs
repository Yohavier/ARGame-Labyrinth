using UnityEngine;
using System.Collections.Generic;

//Global stuff for all players 
public class GameManager : MonoBehaviour
{
	public static GameManager instance;
	private int capsuleCount;
	public List<GameObject> allPlayers = new List<GameObject>();

	private void Awake()
	{
		instance = this;
	}

	public void CheckWinConditionCrew()
	{
		capsuleCount++;
		InformationPanel.instance.SetProgressText(capsuleCount.ToString());
		if (capsuleCount == 4)
		{
			Debug.Log("Crew wins!");
		}
	}
	public void CheckWinConditionMonster()
	{
		if (allPlayers.Count == 0)
		{
			Debug.Log("Monster wins!");
		}
	}

	//TODO: Crashes the game 
	public void KillPlayer(CrewMember crew)
	{
		foreach (GameObject g in allPlayers)
		{
			if (crew.gameObject == g)
			{
				if (crew.storedItem != null)
				{
					crew.storedItem.transform.SetParent(crew.positionTile.transform);
					if (!crew.positionTile.isInFOW)
					{
						crew.storedItem.GetComponent<MeshRenderer>().enabled = true;
						FogOfWar.fow.activeFogOfWarItems.Add(crew.storedItem);
					}
				}
				FogOfWar.fow.activeFogOfWarItems.Remove(crew.gameObject);
				CheckWinConditionMonster();
				Destroy(crew.gameObject);
			}
		}
	}
}
