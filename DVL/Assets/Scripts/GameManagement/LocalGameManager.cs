using UnityEngine;
using System.Collections;
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
	private playingPlayer previousTurn;

	//reference to the Player of this gameInstance
	public GameObject activePlayer;

	public static LocalGameManager instance;

	//Fields for the dice
	private int diceListenerCounter;
	private bool rolledDice;
	private int stepsLeft = 0;
	public int StepsLeft
	{
		get { return stepsLeft; }
		set
		{ 
			stepsLeft = value;
			InformationPanel.instance.SetLeftStepsText(value.ToString());
		}
	}

	private void Awake()
	{
		instance = this;
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
	//TODO: Call SetRollDiceButton only once at beginning of your turn
    private void Update()
    {
        if (GetTurn())
        {
			if (previousTurn != currentTurnPlayer)
			{
				rolledDice = false;
				stepsLeft = 0;
				previousTurn = currentTurnPlayer;
			}
            else
            {
				HandleRollDiceButton();
			}
		}
    }

    #region Handle Roll Dice extension
    private void HandleRollDiceButton()
	{
		if (diceListenerCounter == 0 && !rolledDice)
		{
			diceListenerCounter++;
			InformationPanel.instance.SetRollDiceButton(true);
			InformationPanel.instance.rollDiceButton.onClick.AddListener(RollDice);
		}
        else
        {
			RemoveRollDiceButtonListener();
        }
	}
	private void RollDice()
    {
		StepsLeft = Random.Range(1, 7);
		rolledDice = true;
		RemoveRollDiceButtonListener();
		print("rolled a " + StepsLeft);
    }
	private void RemoveRollDiceButtonListener()
    {
		InformationPanel.instance.SetRollDiceButton(false);
		InformationPanel.instance.rollDiceButton.onClick.RemoveAllListeners();
		diceListenerCounter--;
    }
    #endregion
}
