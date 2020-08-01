using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Toggle = UnityEngine.UI.Toggle;

public enum PowerUpSlotIcon { None, PickUp, Exchange }
public class GUIManager : MonoBehaviour
{
    public static GUIManager instance;

    [Header("Canvas Objects")]
    public GameObject setupCanvas;
    public GameObject lobbyCanvas;
    public GameObject playerCanvas;
    public GameObject settingsCanvas;
    public GameObject endCanvas;
        
    [Header("Join Panel")]
    public Button hostButton;
    public Button joinButton;
    public InputField hostIPInput;
    string serverIP = "192.168.0.206";
    public bool isServer = false;
    public bool isDebug = false;

    [Header("Lobby Panel")]
    public Button startMatchButton;
    public Toggle readyToggle;
    [HideInInspector] public bool needsMenuUpdate = false;
    [HideInInspector] public bool needsPlayerUpdate = false;
    [HideInInspector] public Text[] playerLabels = new Text[4];

    [Header("Game Panel")]
    public Button rollDiceButton;
    public Button nextTurnButton;
    public GameObject diceObject;
    public Text playerText;
    public Text itemText;
    public Text stateText;
    public Text stepsLeftLabel;
    public PowerUpSlot slot1;
    public PowerUpSlot slot2;
    public Sprite NoneT, PickUpT, ExchangeT;
    public List<SO_PlayerClass> playerRoles;
    public SO_PlayerClass selectedPlayerRole;
    public List<SO_PlayerClass> enemyList;
    public List<SO_PlayerClass> crewList;
    private int enemyIndex;
    private int crewIndex;

    [Header("Settings Panel")]
    public Button settingsButton;
    public Button debugButton;
    public Toggle controllerToggle;

    private void Awake()
    {
        Eventbroker.instance.onNotifyNextTurn += OnTurnChange;
        playerLabels[0] = GameObject.Find("Player1Label").GetComponent<Text>();
        playerLabels[1] = GameObject.Find("Player2Label").GetComponent<Text>();
        playerLabels[2] = GameObject.Find("Player3Label").GetComponent<Text>();
        playerLabels[3] = GameObject.Find("Player4Label").GetComponent<Text>();

        hostButton.onClick.AddListener(() => OnHostButtonClick());
        joinButton.onClick.AddListener(() => OnJoinButtonClick());
        startMatchButton.onClick.AddListener(() => OnStartMatchButtonClick());
        nextTurnButton.onClick.AddListener(() => OnNextTurnButtonClick());
        debugButton.onClick.AddListener(() => OnDebugButtonClicked());
        readyToggle.onValueChanged.AddListener((value) => OnReadyToggleValueChanged(value));
        settingsButton.onClick.AddListener(() => ToggleHelpInformationPanel());
        hostIPInput.text = serverIP;
        lobbyCanvas.SetActive(false);
        instance = this;
        isDebug = false;
        diceObject.SetActive(false);
    }

    private void Start()
    {
        playerCanvas.SetActive(false);
        endCanvas.SetActive(false);
        Eventbroker.instance.ChangeGameState(GameFlowState.JOIN);
    }

    private void Update()
    {
        if (needsMenuUpdate)
        {
            lobbyCanvas.SetActive(false);
            HandleLobbyEnvironment();
            playerCanvas.SetActive(true);
            needsMenuUpdate = false;
        }

        if (needsPlayerUpdate)
        {
            for (int i = 0; i < 4; i++)
            {
                playerLabels[i].text = "Player" + (i + 1) + ": " + NetworkClient.instance.networkPlayers[i].ip + " " + (NetworkClient.instance.networkPlayers[i].isReady ? "✓" : "") + Environment.NewLine
                + "Role: " + NetworkClient.instance.networkPlayers[i].roleIndex;      
            }

            needsPlayerUpdate = false;
            if (startMatchButton != null)
                CheckIfEveryoneIsReady();
        }
    }
    private void CheckIfEveryoneIsReady()
    {
        for (int i = 0; i < 4; i++)
        {
            if(NetworkClient.instance.networkPlayers[i].ip != "")
            {
                if (!NetworkClient.instance.networkPlayers[i].isReady)
                {
                    startMatchButton.interactable = false;
                    return;
                }
            } 
        }
        startMatchButton.interactable = true;
    }
    void OnTurnChange()
    {
        nextTurnButton.interactable = LocalGameManager.instance.GetTurn();
    }

    void OpenLobbyMenu()
    {
        setupCanvas.SetActive(false);
        lobbyCanvas.SetActive(true);
        startMatchButton.interactable = false;
        AkSoundEngine.PostEvent("lobby_join", gameObject);
        AudioWwiseManager.instance.SetMusicGameState(GameState.Lobby);
        Eventbroker.instance.ChangeGameState(GameFlowState.LOBBY);
    }

    void OnDebugButtonClicked()
    {
        isDebug = !isDebug;

        if (isDebug)
            DebugConsole.instance.enabled = true;
        else
            DebugConsole.instance.enabled = false;
    }

    void OnNextTurnButtonClick()
    {
        if (LocalGameManager.instance.GetTurn())
            NetworkClient.instance.SendTurnChange();
    }

    void OnStartMatchButtonClick()
    {
        if (isServer)
            NetworkServer.instance.StartMatch();
    }

    void OnHostButtonClick()
    {
        isServer = true;
        NetworkServer.instance.SetupServer();
        NetworkClient.instance.Connect(hostIPInput.text);
        OpenLobbyMenu();
    }

    void OnJoinButtonClick()
    {
        NetworkClient.instance.Connect(hostIPInput.text);          
        OpenLobbyMenu();
        Destroy(startMatchButton.gameObject);
    }

    private int readyCounter = 0;
    void OnReadyToggleValueChanged(bool value)
    {
        if (value)
        {
            readyCounter++;
            Eventbroker.instance.ToggleGate(false);
        }
        else
        {
            Eventbroker.instance.ToggleGate(true);
            readyCounter--;
        }
        NetworkClient.instance.SendReadyChanged(value);
        AkSoundEngine.PostEvent("lobby_smallButton", gameObject);
        SwipeManager.instance.canSwipe = !value;
        AudioWwiseManager.instance.SetMusicIntensity((MusicIntensity)readyCounter);
    }

    public void OnChangeRole()
    {
        AkSoundEngine.PostEvent("lobby_smallButton", gameObject);
    }

    public void DisplayEndScreen(string result)
    {
        endCanvas.SetActive(true);
        endCanvas.GetComponentInChildren<Text>().text = result;
    }

    private void HandleLobbyEnvironment()
    {
        SwipeManager.instance.canSwipe = false;
    }

    #region Game UI Functionality
    public void SetRollDiceButton(bool interactable)
    {
        rollDiceButton.interactable = interactable;
    }

    public void SetPlayerText(string text)
    {
        playerText.text = text;
    }

    public void SetItemText(string text)
    {
        itemText.text = text;
    }

    public void SetStateText(string text)
    {
        stateText.text = text;
    }

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

    public void ToggleHelpInformationPanel()
    {
        Debug.Log("f");
        settingsCanvas.SetActive(!settingsCanvas.activeSelf);
    }
    #endregion

    #region CharacterSelection
    public void OnPlayerRoleChanged(int change)
    {
        SetPlayerRoles(SetRightRole(change));
        Eventbroker.instance.ChangeCharacter(selectedPlayerRole.roleIndex);
        if (LocalGameManager.instance != null)
            NetworkClient.instance.SendRoleChanged(LocalGameManager.instance.localPlayerIndex, selectedPlayerRole.roleIndex);
    }

    private SO_PlayerClass SetRightRole(int change)
    {
        SO_PlayerClass newRole = null;
        if (LocalGameManager.instance.localPlayerIndex != PlayerIndex.Invalid && LocalGameManager.instance.localPlayerIndex != PlayerIndex.Enemy)
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
            if (playerRoles[i].roleIndex == role.roleIndex)
            {
                selectedPlayerRole = playerRoles[i];
            }
        }
    }
    #endregion

    #region UI Audio Function
    public void PlayButtonSound(string file)
    {
        AudioWwiseManager.PostAudio(file);
    }
    #endregion
}

