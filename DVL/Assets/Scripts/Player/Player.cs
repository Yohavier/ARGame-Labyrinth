using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public playingPlayer player;

	public Tile positionTile;

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
			Eventbroker.eventbroker.SignUpForFogOfWar(this.gameObject);
			this.GetComponent<SphereCollider>().radius = 0.5f;
		}
		else
		{
			LocalGameManager.local.activePlayer = this.gameObject;
		}
	}

	public void MoveToTarget(List<Tile> path)
	{
		this.StartCoroutine(Moving(path));
	}

	private IEnumerator Moving(List<Tile> path)
	{
		foreach (Tile item in path)
		{
			Tile tile = positionTile = item;
			this.transform.SetParent(tile.transform);
			this.transform.localPosition = new Vector3(0f, 1f, 0f);
			tile.GetComponent<MeshRenderer>().material.color = tile.prefabColor;
			yield return (object)new WaitForSeconds(0.25f);
		}
	}

	public void OnTriggerEnter(Collider other)
	{
		if (FogOfWar.fow.FogOfWarItems.Contains(other.gameObject))
		{
			other.GetComponent<MeshRenderer>().enabled = true;
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (FogOfWar.fow.FogOfWarItems.Contains(other.gameObject))
		{
			other.GetComponent<MeshRenderer>().enabled = false;
		}
	}
}
