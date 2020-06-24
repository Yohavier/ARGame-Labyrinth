using Assets.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerClass", menuName = "Player Class", order = 1)]
public class SO_PlayerClass : ScriptableObject
{
	public string roleName;
	public RoleIndex roleIndex = RoleIndex.Invalid;
	public int diceModificator;
	public int fogOfWarRadius;
	public int footstepDetectionRadius;
	public int repairSpeed;
	public int maxDeathTurnCounter;

	public void SetPlayerStats(Player player)
    {
		player.diceModificator = diceModificator;
		player.fogOfWarRadius = fogOfWarRadius;
		player.footstepDetectionRadius = footstepDetectionRadius;
		player.repairSpeed = repairSpeed;
		player.maxDeathTurnCounter = maxDeathTurnCounter;
    }
}
