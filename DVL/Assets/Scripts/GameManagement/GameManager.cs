using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum GameFlowState {JOIN, LOBBY, GAME, END }
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
			GUIManager.instance.DisplayEndScreen("Crew Escaped");
		}
	}
	public void CheckWinConditionMonster()
	{
		killCount++;
		if (killCount >= 3)
		{
			GUIManager.instance.DisplayEndScreen("Everybodys dead");
		}
	}
}
