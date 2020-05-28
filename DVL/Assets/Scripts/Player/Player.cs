using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public playingPlayer player;
	private FogOfWar playerFOW;
	public Tile positionTile;

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
			this.GetComponent<Pathfinding>().enabled = false;
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
	}

	//Move along a List of Tiles
	public void MoveToTarget(List<Tile> path)
	{
		this.StartCoroutine(Moving(path));
	}

	//moving "Animation"
	private IEnumerator Moving(List<Tile> path)
	{
		foreach (Tile item in path)
		{
			Tile tile = positionTile = item;
			this.transform.SetParent(tile.transform);
			this.transform.localPosition = new Vector3(0f, 1f, 0f);
			tile.GetComponent<MeshRenderer>().material.color = tile.prefabColor;
			ChangePlayerPosition(item);
			yield return new WaitForSeconds(0.25f);
		}
	}
}
