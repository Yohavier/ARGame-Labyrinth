using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
	public GameObject Player;

	private int playerCount;

	public void SpawnPlayersInCorner(Tile cornerTile)
	{
		playerCount++;
		GameObject player = Instantiate(Player);
		player.GetComponent<Player>().SetUpPlayer(playerCount);
		player.transform.SetParent(cornerTile.transform);
		player.GetComponent<Player>().positionTile = cornerTile;
		player.transform.localPosition = new Vector3(0f, 1f, 0f);
	}
}
