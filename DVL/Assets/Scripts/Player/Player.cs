using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public enum PlayerState { ALIVE, DEAD, DYING}
public class Player : MonoBehaviour
{
	[Header("Player Modifications")]
	public int diceModificator = 0;
	public int fogOfWarModificator = 0;


	public PlayerIndex playerIndex;
	public FogOfWar playerFOW;
	public Tile positionTile;
	public GameObject storedItem;
	private WalkingTraces trace;
	public bool isWalking;

	public PlayerState _playerState;
	public PlayerState playerState
    {
        get
        {
			return _playerState;
        }
        set
        {
			_playerState = value;
			InformationPanel.instance.SetStateText(_playerState.ToString());
			if(_playerState == PlayerState.DYING)
            {
				Dying();
            }
			else if(_playerState == PlayerState.DEAD)
            {
				Dead();
            }
        }
    }

	#region Initialization
	public void SetUpPlayer(int count)
	{
		playerIndex = ChooseRightIndex(count);
		trace = GetComponent<WalkingTraces>();

		if (playerIndex != LocalGameManager.instance.localPlayerIndex)
		{
			GetComponent<MeshRenderer>().enabled = false;
		}
		else
		{
			LocalGameManager.instance.activePlayer = gameObject;
			playerState = PlayerState.ALIVE;
			playerFOW = GetComponent<FogOfWar>();
			SetInformations();
		}
	}
	private PlayerIndex ChooseRightIndex(int count)
    {
		switch (count)
		{
			case 1:
				return PlayerIndex.Player1;
			case 2:
				return PlayerIndex.Player2;
			case 3:
				return PlayerIndex.Player3;
			case 4:
				return PlayerIndex.Enemy;
			default:
				Debug.LogWarning("Player Initializaion went wrong!");
				return PlayerIndex.Invalid;
		}
	}
	private void SetInformations()
    {
		InformationPanel.instance.SetPlayerText(playerIndex.ToString());
		InformationPanel.instance.SetItemText("None");
		InformationPanel.instance.SetProgressText("0");
	}
    #endregion

    #region Player Movement
    public void ChangePlayerPosition(Tile newPos)
	{
		positionTile = newPos;

		if (playerFOW != null)
		{
			//if active player call his FOW
			playerFOW.OnChangePlayerPosition(positionTile, false);
			InformationPanel.instance.SetCoordText(positionTile.row.ToString() + " " + positionTile.column.ToString());
		}
		else if(LocalGameManager.instance.activePlayer != null)
		{
			//if not active Player, call active players FOW
			LocalGameManager.instance.activePlayer.GetComponent<Player>().playerFOW.OnChangePlayerPosition(LocalGameManager.instance.activePlayer.GetComponent<Player>().positionTile, false);	
		}

		CreatePositionIndicatorTrace();
	}

	private void CreatePositionIndicatorTrace()
    {
		if (trace != null)
		{
			trace.SpawnParticlesystem(positionTile);
		}
	}

	//Move along a List of Tiles
	public void MoveToTarget(List<Tile> path)
	{
		if (path.Count > 0)
			StartCoroutine(Moving(path, 1));
	}

	//Moving Player Along Path
	private IEnumerator Moving(List<Tile> path, float time)
	{
		isWalking = true;
		foreach(Tile tile in path)
		{
			if (CheckForOtherPlayers(tile))
			{
				AdjustRotation(tile);
				float i = 0.0f;
				float rate = 1.0f / time;
				while (i < 1.0f)
				{
					i += Time.deltaTime * rate;
					var movementVector = Vector3.Lerp(new Vector3(positionTile.transform.position.x, transform.position.y, positionTile.transform.position.z),
													  new Vector3(tile.transform.position.x, transform.position.y, tile.transform.position.z), i);
					transform.position = movementVector;				
					yield return null;
				}
				transform.SetParent(tile.transform);
				transform.localPosition = Vector3.zero;
				ChangePlayerPosition(tile);
				LocalGameManager.instance.StepsLeft--;
			}
			else
			{
				StopAllCoroutines();
				isWalking = false;
			}
			yield return null;
		}

		if (LocalGameManager.instance.localPlayerIndex == LocalGameManager.instance.currentTurnPlayer)
			CheckTileForOtherMods(path[path.Count - 1]);
		isWalking = false;
	}

	//Rotate player in move direction 
	private void AdjustRotation(Tile lookTarget)
	{
		Vector3 relativePos = lookTarget.transform.position - transform.position;

		// the second argument, upwards, defaults to Vector3.up
		Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
		transform.rotation = rotation;
		transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
	}
    #endregion


    //Check for other Players
    public virtual bool CheckForOtherPlayers(Tile nextTile)
	{
		if (nextTile.GetComponentInChildren<Player>() != null)
		{
			return false;
		}
		return true;
	}
	
	public virtual void CheckTileForOtherMods(Tile tile) 
	{
		//Check for Doors
		HandleDoors(tile);
	}
	protected virtual void Dying() { }
	protected virtual void Dead() { }
	protected virtual void CheckDeathCounter() { }

    #region Handle the Door extension
    private void HandleDoors(Tile tile)
    {
		if (tile.ingameForwardModule == TileDirectionModule.DOOR || tile.ingameBackwardModule == TileDirectionModule.DOOR || tile.ingameRightModule == TileDirectionModule.DOOR || tile.ingameLeftModule == TileDirectionModule.DOOR)
		{
			InformationPanel.instance.SetToggleDoorsButton(true);
			InformationPanel.instance.toggleDoorsButton.onClick.AddListener(() => ToggleDoors(tile));
		}
		else
		{
			RemoveToggleDoorsListener();
		}
	}
	private void ToggleDoors(Tile tile)
    {
		RemoveToggleDoorsListener();
		tile.ToggleDoors();
    }
	public void RemoveToggleDoorsListener()
    {
		InformationPanel.instance.SetToggleDoorsButton(false);
		InformationPanel.instance.toggleDoorsButton.onClick.RemoveAllListeners();
    }
    #endregion
	
	public virtual void NotifyNextTurn(bool toggle) { }
}
