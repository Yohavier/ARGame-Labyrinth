using Assets.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
/*
public class CharacterSelection : MonoBehaviour
{
    public static CharacterSelection instance;

    [Header("Character Selection")]
    public GameObject LobbyChar;
    public SubCharacterLobby[] subLobbyChar;

    public Team myTeam = Team.Enemy;
    public SO_PlayerClass myCurrentRole;

    public List<SO_PlayerClass> crewRoles;
    private int crewIndex;
    public List<SO_PlayerClass> enemyRoles;
    private int enemyIndex;

    public void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        subLobbyChar = LobbyChar.GetComponentsInChildren<SubCharacterLobby>();
        foreach (SubCharacterLobby s in subLobbyChar)
            s.gameObject.SetActive(false);
    }

    #region Character Selection
    public void InitChangeCharacter(int direction)
    {
        ChangeCharacter(direction);
        print(LocalGameManager.instance.localPlayerIndex);
        if (LocalGameManager.instance != null)
            NetworkClient.instance.SendRoleChanged(LocalGameManager.instance.localPlayerIndex, myCurrentRole.roleIndex);
        Debug.Log("Selected " + myCurrentRole.name);
    }
    private void ChangeCharacter(int direction)
    {
        SO_PlayerClass newRole = SetNewRole(direction);
        Debug.Log(newRole.roleIndex);
        HandlePlayerModelsOnChange(newRole);
        myCurrentRole = newRole;
    }
    private void HandlePlayerModelsOnChange(SO_PlayerClass newRole)
    {
        if (myCurrentRole != null)
            ToggleModel(myCurrentRole, false);

        ToggleModel(newRole, true);
    }
    public void ToggleModel(SO_PlayerClass role, bool toggle)
    {
        for (int i = 0; i < subLobbyChar.Length; i++)
        {
            if (subLobbyChar[i].role == role.roleIndex)
                subLobbyChar[i].gameObject.SetActive(toggle); 
        }
    }

    private SO_PlayerClass SetNewRole(int direction)
    {
        SO_PlayerClass newClass = null;

        if (myTeam == Team.Crew)
        {
            crewIndex = SetIndex(direction, crewRoles, crewIndex);
            newClass = crewRoles[crewIndex];
        }
        else
        {           
            enemyIndex = SetIndex(direction, enemyRoles, enemyIndex);
            newClass = enemyRoles[enemyIndex];
        }
        return newClass;
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
    #endregion
}
*/