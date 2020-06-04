using UnityEngine;
using System.Collections.Generic;
//All Possible Player
public enum playingPlayer
{
	Invalid = -1,
	Player1,
	Player2,
	Player3,
	Enemy
}

public enum playerState { Alive, Dead}
public class LocalGameManager : MonoBehaviour
{
	//Determines you playerChar
	public playingPlayer localPlayerIndex;

	//Player index of current turn
	public playingPlayer currentTurnPlayer;


	//reference to the Player of this gameInstance
	public GameObject activePlayer;

	public static LocalGameManager instance;

	private void Awake()
	{
		instance = this;	
	}

	private void Start()
	{
		InformationPanel.playerPanel.SetPlayerText(localPlayerIndex.ToString());

		currentTurnPlayer = playingPlayer.Invalid;
	}

	public bool GetTurn()
	{
		return localPlayerIndex == currentTurnPlayer && GetInMatch();
	}

	public bool GetInMatch()
	{
		return localPlayerIndex != playingPlayer.Invalid && currentTurnPlayer != playingPlayer.Invalid;
	}

}
