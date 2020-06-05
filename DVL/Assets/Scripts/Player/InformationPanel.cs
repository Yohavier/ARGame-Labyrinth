using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InformationPanel : MonoBehaviour
{
    public static InformationPanel instance;

    public Text player;
    public Text items;
    public Text coords;
    public Text progress;

    private void Awake()
    {
        instance = this;
    }

    public void SetPlayerText(string text)
    {
        player.text = "Player: " + text;
    }
    public void SetItemText(string text)
    {
        items.text = "Item: " + text;
    }
    public void SetCoordText(string text)
    {
        coords.text = "Coord: " + text;
    }
    public void SetProgressText(string text)
    {
        progress.text = "Progress: " + text;
    }
}
