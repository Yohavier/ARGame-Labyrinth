using UnityEngine;

public enum playingPlayer
{
	Player1,
	Player2,
	Player3,
	Enemy
}
public class LocalGameManager : MonoBehaviour
{
	public playingPlayer viewOfPlayer;

	public GameObject activePlayer;

	public static LocalGameManager local;

	private void Awake()
	{
		local = this;
		viewOfPlayer = playingPlayer.Player2;		
	}
}
