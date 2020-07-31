using Assets.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum PowerUpSlotIcon { None, PickUp, Exchange}
public class InformationPanel : MonoBehaviour
{
    public static InformationPanel instance;

    public GameObject MenuPanel;
    public Button MenuPanelButton;

    [Header("Buttons")]
    public Button rollDiceButton;

    [Header("Informations")]
    public Text player;
    public Text items;
    public Text state;

    [Header("PowerUpSlots")]
    public PowerUpSlot powerUpSlot1;
    public PowerUpSlot powerUpSlot2;

    [Header("Player Role")]
    public List<SO_PlayerClass> playerRoles;
    public SO_PlayerClass selectedPlayerRole;

    public List<SO_PlayerClass> enemyList;
    public List<SO_PlayerClass> crewList;
    private int enemyIndex;
    private int crewIndex;


    private void Awake()
    {
        instance = this;
        MenuPanelButton.onClick.AddListener(ToggleMenuPanel);
    }

    private void ToggleMenuPanel()
    {
        MenuPanel.SetActive(!MenuPanel.activeSelf);
        AudioWwiseManager.PostAudio("lobby_smallButton");
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
        items.text = text;
    }
    public void SetStateText(string text)
    {
        state.text = text;
    }
    #endregion

    #region CharacterSelection
    public void OnPlayerRoleChanged(int change)
    {
        SetPlayerRoles(SetRightRole(change));
        Eventbroker.instance.ChangeCharacter(selectedPlayerRole.roleIndex);
        if (LocalGameManager.instance != null)
            NetworkClient.instance.SendRoleChanged(LocalGameManager.instance.localPlayerIndex, selectedPlayerRole.roleIndex);
        Debug.Log("Selected " + selectedPlayerRole.name);
    }

    private SO_PlayerClass SetRightRole(int change)
    {
        SO_PlayerClass newRole = null;
        if(LocalGameManager.instance.localPlayerIndex != PlayerIndex.Invalid && LocalGameManager.instance.localPlayerIndex != PlayerIndex.Enemy)
        {
            crewIndex = SetIndex(change, crewList, crewIndex);
            newRole = crewList[crewIndex];
        }
        else if (LocalGameManager.instance.localPlayerIndex == PlayerIndex.Enemy)
        {
            enemyIndex = SetIndex(change, enemyList, enemyIndex);
            newRole = enemyList[enemyIndex];
        }
        return newRole;
    }

    private int SetIndex(int direction, List<SO_PlayerClass> roles, int currentIndex)
    {
        int newIndex;

        newIndex = currentIndex + direction;
        if (newIndex < 0)
            newIndex = roles.Count - 1;
        else if (newIndex > roles.Count - 1)
            newIndex = 0;

        return newIndex;
    }

    private void SetPlayerRoles(SO_PlayerClass role)
    {
        for (int i = 0; i < playerRoles.Count; i++)
        {
            if(playerRoles[i].roleIndex == role.roleIndex)
            {
                selectedPlayerRole = playerRoles[i];
            }
        }
    }
    #endregion

    #region PowerUpSlots
    public Sprite NoneT, PickUpT, ExchangeT;
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
        AudioWwiseManager.PostAudio(file);
    }

    public GameObject helpInformation;
    public void ToggleHelpInformationPanel()
    {
        helpInformation.SetActive(!helpInformation.activeSelf);
    }
}
