using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

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
    

    [Header("Lobby Panel")]
    public Button startMatchButton;
    public Toggle readyToggle;
    [HideInInspector] public bool needsMenuUpdate = false;
    [HideInInspector] public bool needsPlayerUpdate = false;
    [HideInInspector] public Text[] playerLabels = new Text[4];
    private int readyCounter = 0;
    [HideInInspector] public bool isReady;

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
    public Toggle debugVisionToggle;
    public Toggle debugMovementToggle;
    public Toggle controllerToggle;
    public Toggle toggleHelpUI;
    public GameObject[] helpUI;
    [HideInInspector] public bool isDebug = false;

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
        readyToggle.onValueChanged.AddListener((value) => OnReadyToggleValueChanged(value));
        settingsButton.onClick.AddListener(() => ToggleSettingsPanel());

        debugVisionToggle.onValueChanged.AddListener((value) => ToggleDebugVisionMode(value));
        debugMovementToggle.onValueChanged.AddListener((value) => ToggleDebugMoveMode(value));
        controllerToggle.onValueChanged.AddListener((value) => ToggleController(value));
        toggleHelpUI.onValueChanged.AddListener((value) => ToggleHelpInfo(value));

        hostIPInput.text = serverIP;
        lobbyCanvas.SetActive(false);
        instance = this;
        isDebug = false;
        diceObject.SetActive(false);

#if UNITY_EDITOR || UNITY_STANDALONE
        controllerToggle.interactable = false;
#endif
    }

    private void Start()
    {
        playerCanvas.SetActive(false);
        endCanvas.SetActive(false);
        Eventbroker.instance.ChangeGameState(GameState.JOIN);
    }

    private void Update()
    {
        if (needsMenuUpdate)
        {
            lobbyCanvas.SetActive(false);
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
        nextTurnButton.interactable = GameManager.instance.GetTurn();
    }

    void OpenLobbyMenu()
    {
        setupCanvas.SetActive(false);
        lobbyCanvas.SetActive(true);
        startMatchButton.interactable = false;
        AkSoundEngine.PostEvent("lobby_join", gameObject);
        Eventbroker.instance.ChangeGameState(GameState.LOBBY);
    }

    #region Debug Toggles
    public void ToggleDebugVisionMode(bool value)
    {
        isDebug = value;
        DebugConsole.instance.enabled = value;
    }
    public void ToggleDebugMoveMode(bool value)
    {
        GameManager.instance.isDebugingMovement = value;
    }
    public void ToggleController(bool value)
    {
        Controller.instance.MobileHaptikTileControl(value);
    }
    public void ToggleHelpInfo(bool value)
    {
        foreach (GameObject help in helpUI)
            help.SetActive(value);
    }
    #endregion
    void OnNextTurnButtonClick()
    {
        if (GameManager.instance.GetTurn())
            NetworkClient.instance.SendTurnChange();
    }

    #region Join Methods
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
    #endregion

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

    public void ToggleSettingsPanel()
    {
        settingsCanvas.SetActive(!settingsCanvas.activeSelf);
    }
    #endregion

    #region CharacterSelection
    public void OnPlayerRoleChanged(int change)
    {
        SetPlayerRoles(SetRightRole(change));
        Eventbroker.instance.ChangeCharacter(selectedPlayerRole.roleIndex);
        if (GameManager.instance != null)
            NetworkClient.instance.SendRoleChanged(GameManager.instance.localPlayerIndex, selectedPlayerRole.roleIndex);
    }

    private SO_PlayerClass SetRightRole(int change)
    {
        SO_PlayerClass newRole = null;
        if (GameManager.instance.localPlayerIndex != PlayerIndex.Invalid && GameManager.instance.localPlayerIndex != PlayerIndex.Enemy)
        {
            crewIndex = SetIndex(change, crewList, crewIndex);
            newRole = crewList[crewIndex];
        }
        else if (GameManager.instance.localPlayerIndex == PlayerIndex.Enemy)
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

