using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeCapsule : Tile
{
    public void DisplayProgress()
    {
        GameManager.instance.CheckWinConditionCrew();
    }
}
