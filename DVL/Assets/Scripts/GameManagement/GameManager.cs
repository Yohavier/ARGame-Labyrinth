using UnityEngine;
using System.Collections.Generic;

//TODO move everything to localgamemanager
//TODO Handle WinCondition in here
//TODO Rename Class
//TODO make static classes Singleton/ScriptableObject(LocalGameManager, FogOfWar, EventBroker, HandleTrackedImageLib, selectARObjectWithFInger, BoardGrid, InformationPanel)
public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	private int capsuleCount;
	public List<GameObject> allPlayers = new List<GameObject>();

	private void Awake()
	{
		instance = this;
	}

	public void CheckWinConditionCrew()
	{
		capsuleCount++;
		InformationPanel.instance.SetProgressText(capsuleCount.ToString());
		if (capsuleCount == 4)
		{
			Debug.Log("Crew wins!");
		}
	}
	public void CheckWinConditionMonster()
	{
		if (allPlayers.Count == 0)
		{
			Debug.Log("Monster wins!");
		}
	}
}
