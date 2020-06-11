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
				SetRollDiceButton();
			}
		}
    }

    private int diceListenerCounter;
	private bool rolledDice = false;
	private int stepsLeft = 0;
	public int StepsLeft
    {
        get { return stepsLeft; }
        set { stepsLeft = value; }
    }
		
	private void SetRollDiceButton()
	{
		if (diceListenerCounter == 0 && !rolledDice)
		{
			diceListenerCounter++;
			InformationPanel.instance.SetRollDiceButton(true);
			InformationPanel.instance.rollDiceButton.onClick.AddListener(RollDice);
		}
	}
	private void RollDice()
    {
		stepsLeft = Random.Range(1, 7);
		stepsLeft = 100;
		rolledDice = true;
		diceListenerCounter = 0;
		InformationPanel.instance.SetRollDiceButton(false);
		InformationPanel.instance.rollDiceButton.onClick.RemoveAllListeners();
		print("rolled a " + stepsLeft);
    }
}
