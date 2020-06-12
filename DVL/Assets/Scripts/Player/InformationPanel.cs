using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InformationPanel : MonoBehaviour
{
    public static InformationPanel instance;

    public GameObject MenuPanel;
    public Button MenuPanelButton;


    [Header("Buttons")]
    public Button pickUpItemButton;
    public Button repairGeneratorButton;
    public Button dropItemButton;
    public Button rollDiceButton;
    public Button toggleDoorsButton;

    [Header("Informations")]
    public Text player;
    public Text items;
    public Text coords;
    public Text progress;
    public Text steps;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        MenuPanelButton.onClick.AddListener(ToggleMenuPanel);
    }

    private void ToggleMenuPanel()
    {
        MenuPanel.SetActive(!MenuPanel.activeSelf);
    }

    #region HandleButtons
    public void SetPickUpItemButton(bool interactable)
    {
        pickUpItemButton.interactable = interactable;
    }
    public void SetRepairGeneratorButton(bool interactable)
    {
        repairGeneratorButton.interactable = interactable;
    }
    public void SetDropItemButton(bool interactable)
    {
        dropItemButton.interactable = interactable;
    }
    public void SetRollDiceButton(bool interactable)
    {
        rollDiceButton.interactable = interactable;
    }
    public void SetToggleDoorsButton(bool interactable)
    {
        toggleDoorsButton.interactable = interactable;
    }
    #endregion

    #region SetInformations
    public void SetPlayerText(string text)
    {
        player.text = text;
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

    public void SetLeftStepsText(string text)
    {
        steps.text = "Steps Left: " + text;
    }
    #endregion
}
