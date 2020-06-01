using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeCapsule : MonoBehaviour
{
    public void DisplayProgress()
    {
        GameManager.GameManagerInstance.CheckWinConditionCrew();
    }
}
