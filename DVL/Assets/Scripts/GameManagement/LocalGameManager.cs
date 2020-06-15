using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
	private playingPlayer previousTurn;

	//reference to the Player of this gameInstance
	public GameObject activePlayer;

	public static LocalGameManager instance;

	//Fields for the dice
	private int stepsLeft = 0;
	public int StepsLeft
	{
		get { return stepsLeft; }
		set
		{ 
			if(currentTurnPlayer == localPlayerIndex)
            {
				stepsLeft = value;
				InformationPanel.instance.SetLeftStepsText(value.ToString());
			}
		}
	}

	private void Awake()
	{
		instance = this;
		currentTurnPlayer = playingPlayer.Invalid;
	}
    private void Update()
    {
        if (GetTurn())
        {
			if(previousTurn != currentTurnPlayer)
            {
				previousTurn = currentTurnPlayer;
				NextTurn();
            } 
        }
		else if(previousTurn != currentTurnPlayer)
        {
			previousTurn = currentTurnPlayer;
			RemoveRollDiceButtonListener();
        }
    }
    public bool GetTurn()
	{
		return localPlayerIndex == currentTurnPlayer && GetInMatch();
	}
	public bool GetInMatch()
	{
		return localPlayerIndex != playingPlayer.Invalid && currentTurnPlayer != playingPlayer.Invalid;
	}

	public void NextTurn()
    {
        if (GetTurn())
        {
			HandleRollDiceButton();
		}
        else
        {
			RemoveRollDiceButtonListener();
        }
	}

    #region Handle Roll Dice extension
    private void HandleRollDiceButton()
	{
		if (InformationPanel.instance)
		{
			InformationPanel.instance.SetRollDiceButton(true);
			InformationPanel.instance.rollDiceButton.onClick.AddListener(RollDice);
		}
	}
	private void RollDice()
    {
		StepsLeft = Random.Range(1, 7);
		RemoveRollDiceButtonListener();
		print("rolled a " + StepsLeft);
    }
	private void RemoveRollDiceButtonListener()
    {
        if (InformationPanel.instance)
		{
			Debug.Log("f");
			InformationPanel.instance.SetRollDiceButton(false);
			InformationPanel.instance.rollDiceButton.onClick.RemoveAllListeners();
		}
	}
    #endregion
}
