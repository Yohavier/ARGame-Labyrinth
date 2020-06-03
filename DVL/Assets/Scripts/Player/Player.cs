using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public playingPlayer player;
	private FogOfWar playerFOW;
	public Tile positionTile;
	public GameObject storedItem;

	//called once after init to determine if this is the active player of the scene
	public void SetUpPlayer(int count)
	{
		switch (count)
		{
		case 1:
			player = playingPlayer.Player1;
			break;
		case 2:
			player = playingPlayer.Player2;
			break;
		case 3:
			player = playingPlayer.Player3;
			break;
		case 4:
			player = playingPlayer.Enemy;
			break;
		}
		if (player != LocalGameManager.local.viewOfPlayer)
		{
			this.GetComponent<MeshRenderer>().enabled = false;
		}
		else
		{
			LocalGameManager.local.activePlayer = this.gameObject;
			playerFOW = GetComponent<FogOfWar>();
		}
	}

	//if the Player moves
	public void ChangePlayerPosition(Tile newPos)
	{
		if(playerFOW != null)
			playerFOW.OnChangePlayerPosition(newPos);


		positionTile = newPos;
		if(LocalGameManager.local.activePlayer == this.gameObject)
		{
			InformationPanel.playerPanel.SetCoordText(positionTile.row.ToString()+ " " + positionTile.column.ToString());
		}
	}

	//Move along a List of Tiles
	public void MoveToTarget(List<Tile> path)
	{
		StartCoroutine(Moving(path));
	}

	//moving "Animation"
	private IEnumerator Moving(List<Tile> path)
	{
		foreach (Tile item in path)
		{
			if (checkForOtherPlayers(item))
			{
				this.transform.SetParent(item.transform);
				this.transform.localPosition = new Vector3(0f, .5f, 0f);
				ChangePlayerPosition(item);				
			}
			else
			{
				StopAllCoroutines();		
			}
			yield return new WaitForSeconds(0.25f);
		}
		CheckTileForOtherMods(path[path.Count-1]);
	}

	public virtual bool checkForOtherPlayers(Tile nextTile)
	{
		if (nextTile.GetComponentInChildren<Player>() != null)
		{
			return false;
		}
		return true;
	}
	public virtual void CheckTileForOtherMods(Tile target)
	{

	}
}
