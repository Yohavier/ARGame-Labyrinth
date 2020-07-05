using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	private int capsuleCount;
	private int killCount;
	public List<GameObject> allPlayers = new List<GameObject>();

	private void Awake()
	{
		instance = this;
	}

	public void KillPlayer(Player player)
	{
		player.playerState = PlayerState.DYING;
	}

	public void CheckWinConditionCrew()
	{
		capsuleCount++;
		BoardEnvironment.instance.ActivateNextSignal(capsuleCount);
		if (capsuleCount == 4)
		{
			Debug.Log("Crew wins!");
		}
	}
	public void CheckWinConditionMonster()
	{
		killCount++;
		if (killCount >= 3)
		{
			Debug.Log("Monster wins!");
		}
	}
}
