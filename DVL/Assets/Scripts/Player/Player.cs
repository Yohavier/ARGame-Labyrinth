using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Player : MonoBehaviour
{
	public playingPlayer playerIndex;
	public FogOfWar playerFOW;
	public Tile positionTile;
	public GameObject storedItem;
	public bool isPlayerMoving;

    #region Initialization
    public void SetUpPlayer(int count)
	{
		playerIndex = ChooseRightIndex(count);

		if (playerIndex != LocalGameManager.instance.localPlayerIndex)
		{
			GetComponent<MeshRenderer>().enabled = false;
		}
		else
		{
			LocalGameManager.instance.activePlayer = gameObject;
			playerFOW = GetComponent<FogOfWar>();
			SetInformations();
		}
	}
	private playingPlayer ChooseRightIndex(int count)
    {
		switch (count)
		{
			case 1:
				return playingPlayer.Player1;
			case 2:
				return playingPlayer.Player2;
			case 3:
				return playingPlayer.Player3;
			case 4:
				return playingPlayer.Enemy;
			default:
				Debug.LogWarning("Player Initializaion went wrong!");
				return playingPlayer.Invalid;
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
			//if active playre call his FOW
			playerFOW.OnChangePlayerPosition(positionTile);
			InformationPanel.instance.SetCoordText(positionTile.row.ToString() + " " + positionTile.column.ToString());
		}
		else if(LocalGameManager.instance.activePlayer != null)
		{
			//if not active Player, call active players FOW
			LocalGameManager.instance.activePlayer.GetComponent<Player>().playerFOW.OnChangePlayerPosition(LocalGameManager.instance.activePlayer.GetComponent<Player>().positionTile);	
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
		isPlayerMoving = true;
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
				isPlayerMoving = false;
				StopAllCoroutines();
			}
			yield return null;
		}
		isPlayerMoving = false;

		if (LocalGameManager.instance.localPlayerIndex == LocalGameManager.instance.currentTurnPlayer)
			CheckTileForOtherMods(path[path.Count - 1]);
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

    #region Handle the Door extension
    private void HandleDoors(Tile tile)
    {
		if (tile.hasLeftDoor || tile.hasRightDoor || tile.hasForwardDoor || tile.hasBackwardDoor)
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
	private void RemoveToggleDoorsListener()
    {
		InformationPanel.instance.SetToggleDoorsButton(false);
		InformationPanel.instance.toggleDoorsButton.onClick.RemoveAllListeners();
    }
    #endregion
}
