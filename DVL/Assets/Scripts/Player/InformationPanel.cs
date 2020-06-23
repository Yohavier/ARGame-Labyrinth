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
    public Button healPlayerButton;

    [Header("Informations")]
    public Text player;
    public Text items;
    public Text coords;
    public Text progress;
    public Text steps;
    public Text state;

    [Header("PowerUpSlots")]
    public Button powerUpSlot1;
    public Button powerUpSlot2;

    [Header("Player Role")]
    public Dropdown playerRoleMenu;
    public List<SO_PlayerClass> playerRoles;
    public SO_PlayerClass selectedPlayerRole;

    private void Awake()
    {
        instance = this;
        MenuPanelButton.onClick.AddListener(ToggleMenuPanel);
        SetUpDropDownMenu();
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
    public void SetHealPlayerButton(bool interactable)
    {
        healPlayerButton.interactable = interactable;
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

    public void SetStateText(string text)
    {
        state.text = "State: " + text;
    }
    #endregion

    #region CharacterSelection
    private void SetUpDropDownMenu()
    {
        playerRoleMenu.ClearOptions();
        CreateDropDownMenu();
        playerRoleMenu.onValueChanged.AddListener(delegate {
            DropdownValueChanged(playerRoleMenu);
        });
    }

    private void CreateDropDownMenu()
    {
        foreach (SO_PlayerClass so in playerRoles)
        {
            var n = new Dropdown.OptionData();
            n.text = so.name;
            playerRoleMenu.options.Add(n);
        }
        DropdownValueChanged(playerRoleMenu);
    }

    public void DropdownValueChanged(Dropdown change)
    {
        selectedPlayerRole = playerRoles[change.value];
        Debug.Log(playerRoles[change.value].name);
    }

    //TODO: Send to all other Players as well
    public SO_PlayerClass GetPlayerRoleStats(Player player)
    {
        playerRoleMenu.interactable = false;
        return selectedPlayerRole;
    }
    #endregion
}
