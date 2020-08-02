using UnityEngine;
using System.Collections.Generic;

public enum PlayerIndex
{
	Invalid = -1,
	Player1,
	Player2,
	Player3,
	Enemy
}
public enum GameState { JOIN, LOBBY, GAME, END }

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	[Header("Character")]
	public PlayerIndex localPlayerIndex = PlayerIndex.Invalid;
	public GameObject activePlayer;
	private PlayerIndex previousTurn;

	[Header("Turn")]
	public bool canMove = true;
	public PlayerIndex currentTurnPlayer;

	[HideInInspector] public bool isDebugingMovement = false;

	private int capsuleCount;
	private int killCount;
	public List<GameObject> allPlayers = new List<GameObject>();

	#region Getter Setter
	private bool moveTileToken;
	public bool _moveTileToken
    {
        get
        {
			return moveTileToken;
        }
        set
        {
			if (isDebugingMovement)
				moveTileToken = true;
			else
				moveTileToken = value;
        }
    }

	private int stepsLeft = 0;
	public int _stepsLeft
	{
		get { return stepsLeft; }
		set
		{ 
			if(currentTurnPlayer == localPlayerIndex)
            {
				if (isDebugingMovement)
					stepsLeft = 10;
				else
					stepsLeft = value;
				GUIManager.instance.stepsLeftLabel.text = stepsLeft.ToString();
				DiceHandler.instance.OnChangeDiceText(stepsLeft, false);
			}
		}
	}
    #endregion

    #region Unity Methods
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
			previousTurn = currentTurnPlayer;
			Eventbroker.instance.NotifyNextTurn();
        }
    }
    #endregion

    #region Handle Turn
    private void NotifyNextTurn()
    {
		if (GetTurn())
        {
			HandleRollDiceButton();
			_moveTileToken = true;
		}
        else
        {
			RemoveRollDiceButtonListener();
			_moveTileToken = false;
			if (activePlayer)
				activePlayer.GetComponent<Player>().NotifyNextTurn(false);
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
    #endregion

    #region Handle Roll Dice extension
    private void HandleRollDiceButton()
	{
		if (GUIManager.instance)
		{
			_stepsLeft = 0;
			GUIManager.instance.SetRollDiceButton(true);
			GUIManager.instance.rollDiceButton.onClick.AddListener(RollDice);
		}
	}
	private void RollDice()
    {
		GUIManager.instance.diceObject.SetActive(true);
		DiceHandler.instance.RollDiceAnimation(Random.Range(1, 7));
		RemoveRollDiceButtonListener();

		if(activePlayer)
			activePlayer.GetComponent<Player>().NotifyNextTurn(true);
    }
	private void RemoveRollDiceButtonListener()
    {
        if (GUIManager.instance)
		{
			GUIManager.instance.SetRollDiceButton(false);
			GUIManager.instance.rollDiceButton.onClick.RemoveAllListeners();
		}
	}
	#endregion

	#region Handle Win Conditions
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
	#endregion
}
