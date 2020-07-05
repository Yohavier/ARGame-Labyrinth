using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Generator : Item
{
    [SerializeField] public int activationsNeeded = 0;
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
            if (!position.isInFOW)
            {
                AkSoundEngine.PostEvent("generator_finish", gameObject);
            }
        }
        else if (!position.isInFOW)
        {
            AkSoundEngine.PostEvent("generator_repair", gameObject);
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
