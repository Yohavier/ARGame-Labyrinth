using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    [SerializeField] private int activationsNeeded = 0;
    public int currentActivations;

    public void RepairGenerator(int repairSpeed)
    {
        currentActivations += repairSpeed;
        CheckGeneratorState();
    }

    private void CheckGeneratorState()
    {
        if (activationsNeeded == currentActivations)
        {
            GameManager.instance.CheckWinConditionCrew();
        }
    }
}
