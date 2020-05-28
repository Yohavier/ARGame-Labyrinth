using UnityEngine;

//All Possible Player
public enum playingPlayer
{
	Player1,
	Player2,
	Player3,
	Enemy
}
public class LocalGameManager : MonoBehaviour
{
	//Determines you playerChar
	public playingPlayer viewOfPlayer;

	//reference to the Player of this gameInstance
	public GameObject activePlayer;

	public static LocalGameManager local;

	private void Awake()
	{
		local = this;
		viewOfPlayer = playingPlayer.Player2;		
	}
}
