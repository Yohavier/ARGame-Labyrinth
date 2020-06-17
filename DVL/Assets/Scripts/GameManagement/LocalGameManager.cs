using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum PlayerIndex
{
	Invalid = -1,
	Player1,
	Player2,
	Player3,
	Enemy
}
//TODO: Add Turn Counter and notificator on turn change
public enum playerState { Alive, Dead}
public class LocalGameManager : MonoBehaviour
{
	//Determines you playerChar
	public PlayerIndex localPlayerIndex;

	//Player index of current turn
	public PlayerIndex currentTurnPlayer;
	private PlayerIndex previousTurn;

	//reference to the Player of this gameInstance
	public GameObject activePlayer;

	public static LocalGameManager instance;

	//Fields for the dice
	private int stepsLeft = 0;
	public int turnCounter = 0;

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
		currentTurnPlayer = PlayerIndex.Invalid;
	}
    private void OnEnable()
    {
		Eventbroker.instance.onNotifyNextTurn += NotifyNextTurn;
    }
	private void OnDisable()
    {
		Eventbroker.instance.onNotifyNextTurn -= NotifyNextTurn;
	}
    private void Update()
    {
		if(previousTurn != currentTurnPlayer)
        {
			Eventbroker.instance.NotifyNextTurn();
        }
    }
	private void NotifyNextTurn()
    {
		previousTurn = currentTurnPlayer;
		turnCounter++;
		InformationPanel.instance.SetTurnCounterText(turnCounter.ToString());

		if (GetTurn())
        {
			YourNextTurn();
		}
        else
        {
			RemoveRollDiceButtonListener();
		}
    }
    public bool GetTurn()
	{
		return localPlayerIndex == currentTurnPlayer && GetInMatch();
	}
	public bool GetInMatch()
	{
		return localPlayerIndex != PlayerIndex.Invalid && currentTurnPlayer != PlayerIndex.Invalid;
	}

	public void YourNextTurn()
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
			InformationPanel.instance.SetRollDiceButton(false);
			InformationPanel.instance.rollDiceButton.onClick.RemoveAllListeners();
		}
	}
    #endregion
}
