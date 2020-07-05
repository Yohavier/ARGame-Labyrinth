using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Generator : Item
{
    [SerializeField] private int activationsNeeded = 0;
    public int currentActivations;
    public static Generator instance;
    public bool isFinished;

    public override void InstantiateItem()
    {
        base.InstantiateItem();
        instance = this;
    }

    public void RepairGenerator(int repairSpeed)
    {
        currentActivations += repairSpeed;
        CheckGeneratorState();
    }
    private void CheckGeneratorState()
    {
        if (activationsNeeded == currentActivations)
        {
            isFinished = true;
            GameManager.instance.CheckWinConditionCrew();
        }
    }
            
    public override void SendInteraction()
    {
        if (!alreadyRepairedThisTurn)
        {
            CrewMember player = LocalGameManager.instance.activePlayer.GetComponent<CrewMember>();
            if (player != null)
            {
                player.RepairGenerator(this);
                alreadyRepairedThisTurn = true;
            }
        }
    }
}
