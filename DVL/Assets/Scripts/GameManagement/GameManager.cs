using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager GameManagerInstance;

	private void Awake()
	{
		GameManagerInstance = this;
	}
}
