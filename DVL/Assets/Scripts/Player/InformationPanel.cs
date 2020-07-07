using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PowerUpSlotIcon { None, PickUp, Exchange}
public class InformationPanel : MonoBehaviour
{
    public static InformationPanel instance;

    public GameObject MenuPanel;
    public Button MenuPanelButton;
    public GameObject lobbyChar;

    [Header("Buttons")]
    public Button rollDiceButton;

    [Header("Informations")]
    public Text player;
    public Text items;
    public Text state;

    [Header("PowerUpSlots")]
    public Button powerUpSlot1;
    public Button powerUpSlot2;

    [Header("Player Role")]
    public List<SO_PlayerClass> playerRoles;
    public SO_PlayerClass selectedPlayerRole;

    private void Awake()
    {
        lobbyChar = GameObject.Find("LobbyChar");
        instance = this;
        MenuPanelButton.onClick.AddListener(ToggleMenuPanel);
    }

    private void ToggleMenuPanel()
    {
        MenuPanel.SetActive(!MenuPanel.activeSelf);
        AudioCalls.PostAudio("lobby_smallButton");
    }

    #region HandleButtons
    public void SetRollDiceButton(bool interactable)
    {
        rollDiceButton.interactable = interactable;
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
    public void SetStateText(string text)
    {
        state.text = "State: " + text;
    }
    #endregion

    #region CharacterSelection
    public void DropdownValueChanged(int change)
    {
        int value = SetRightNumber(change);
        selectedPlayerRole = playerRoles[value];
        lobbyChar.GetComponent<LobbyCharacterSelection>().OnChangeSelectedCharacter(selectedPlayerRole.roleIndex, change);
        if (LocalGameManager.instance != null)
            NetworkClient.instance.SendRoleChanged(LocalGameManager.instance.localPlayerIndex, selectedPlayerRole.roleIndex);
        Debug.Log("Selected " + playerRoles[value].name);
    }

    private int SetRightNumber(int change)
    {
        int current = 0;
        if (selectedPlayerRole != null)
            current = (int)selectedPlayerRole.roleIndex;

        int temp = current + change;
        if (temp < 0)
            return playerRoles.Count - 1;
        else if (temp > playerRoles.Count - 1)
            return 0;
        else
            return temp;
    }
    public SO_PlayerClass GetPlayerRoleStats(Player player)
    {
        //playerRoleMenu.interactable = false;
        return selectedPlayerRole;
    }
    #endregion

    #region PowerUpSlots
    public Sprite NoneT, PickUpT, ExchangeT;
    public Button slotIcon1, slotIcon2;
    public void ChangeSlotIcon(PowerUpSlotIcon icon, Button slotIcon)
    {
        switch (icon)
        {
            case PowerUpSlotIcon.None:
                slotIcon.image.sprite = NoneT;
                break;
            case PowerUpSlotIcon.PickUp:
                slotIcon.image.sprite = PickUpT;
                break;
            case PowerUpSlotIcon.Exchange:
                slotIcon.image.sprite = ExchangeT;
                break;
        }
    }
    #endregion

    public void PlayButtonSound(string file)
    {
        AudioCalls.PostAudio(file);
    }
}
