﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    [SerializeField] private int activationsNeeded = 0;
    public int currentActivations;
    public static Generator instance;
    public bool isFinished;

    public void Awake()
    {
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
}
