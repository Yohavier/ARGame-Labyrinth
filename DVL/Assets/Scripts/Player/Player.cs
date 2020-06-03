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
		StartCoroutine(Moving(path, 1));
	}

	private IEnumerator Moving(List<Tile> path, float time)
	{
		foreach(Tile tile in path)
		{
			if (checkForOtherPlayers(tile))
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
				this.transform.SetParent(tile.transform);
				this.transform.localPosition = new Vector3(0f, .5f, 0f);
				ChangePlayerPosition(tile);
			}
			else
			{
				StopAllCoroutines();
			}
			yield return null;
		}
		CheckTileForOtherMods(path[path.Count - 1]);
	}

	private void AdjustRotation(Tile lookTarget)
	{
		Vector3 relativePos = lookTarget.transform.position - transform.position;

		// the second argument, upwards, defaults to Vector3.up
		Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
		transform.rotation = rotation;
		transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
	}

	public virtual bool checkForOtherPlayers(Tile nextTile)
	{
		if (nextTile.GetComponentInChildren<Player>() != null)
		{
			return false;
		}
		return true;
	}
	public virtual void CheckTileForOtherMods(Tile target) { }
}
