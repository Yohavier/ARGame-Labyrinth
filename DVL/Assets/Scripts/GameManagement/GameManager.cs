using UnityEngine;

//Global stuff for all players 
public class GameManager : MonoBehaviour
{
	public static GameManager GameManagerInstance;

	private void Awake()
	{
		GameManagerInstance = this;
	}
}
