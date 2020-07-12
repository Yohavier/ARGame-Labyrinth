using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharInfo : MonoBehaviour
{
    public void DisplayNewInfo(SO_PlayerClass role)
    {
        GetComponentInChildren<Text>().text = role.roleInfoText;
    }
}
