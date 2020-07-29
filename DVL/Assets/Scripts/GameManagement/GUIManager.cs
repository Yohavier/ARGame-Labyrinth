using Assets.Scripts.Utils;
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

namespace Assets.Scripts.GameManagement
{
    public class GUIManager : MonoBehaviour
    {
        private Camera arCamera;

        public static GUIManager instance;
        public GameObject setupCanvas;
        public GameObject lobbyCanvas;
        public GameObject lobbyEnvironment;
        public LobbyGate lobbyGate;
        public GameObject boardEnvironment;
        public GameObject playerCanvas;
        public GameObject endCanvas;
        public GameObject diceObject;
        public InputField hostIPInput;
        public Button hostButton;
        public Button joinButton;
        public Button startMatchButton;
        public Button nextTurnButton;
        public Button debugButton;
        public Toggle readyToggle;
        public Text[] playerLabels = new Text[4];
        public Text stepsLeftLabel;

        string serverIP = "192.168.194.169";
        public bool isServer = false;
        public bool isDebug = false;

        public bool needsMenuUpdate = false;
        public bool needsPlayerUpdate = false;

        private void Awake()
        {
            arCamera = Camera.main;
            //arCamera.enabled = false;

            Eventbroker.instance.onNotifyNextTurn += OnTurnChange;
            setupCanvas = GameObject.Find("SetupCanvas");
            lobbyCanvas = GameObject.Find("LobbyCanvas");
            lobbyEnvironment = GameObject.Find("LobbyEnvironment");
            lobbyGate = lobbyEnvironment.GetComponentInChildren<LobbyGate>();
            boardEnvironment = GameObject.Find("BoardEnvironment");
            playerCanvas = GameObject.Find("PlayerCanvas");
            endCanvas = GameObject.Find("EndCanvas");
            hostIPInput = GameObject.Find("HostIPInput").GetComponent<InputField>();
            hostButton = GameObject.Find("HostButton").GetComponent<Button>();
            joinButton = GameObject.Find("JoinButton").GetComponent<Button>();
            startMatchButton = GameObject.Find("StartMatchButton").GetComponent<Button>();
            nextTurnButton = GameObject.Find("NextTurnButton").GetComponent<Button>();
            debugButton = GameObject.Find("DebugButton").GetComponent<Button>();
            readyToggle = GameObject.Find("ReadyToggle").GetComponent<Toggle>();
            playerLabels[0] = GameObject.Find("Player1Label").GetComponent<Text>();
            playerLabels[1] = GameObject.Find("Player2Label").GetComponent<Text>();
            playerLabels[2] = GameObject.Find("Player3Label").GetComponent<Text>();
            playerLabels[3] = GameObject.Find("Player4Label").GetComponent<Text>();
            diceObject = GameObject.Find("Dice");
            stepsLeftLabel = GameObject.Find("StepsLeftLabel").GetComponent<Text>();

            hostButton.onClick.AddListener(() => OnHostButtonClick());
            joinButton.onClick.AddListener(() => OnJoinButtonClick());
            startMatchButton.onClick.AddListener(() => OnStartMatchButtonClick());
            nextTurnButton.onClick.AddListener(() => OnNextTurnButtonClick());
            debugButton.onClick.AddListener(() => OnDebugButtonClicked());
            readyToggle.onValueChanged.AddListener((value) => OnReadyToggleValueChanged(value));
            hostIPInput.text = serverIP;
            lobbyCanvas.SetActive(false);
            lobbyEnvironment.SetActive(false);
            boardEnvironment.SetActive(false);
            instance = this;
            isDebug = false;
            diceObject.SetActive(false);
        }

        private void Start()
        {
            playerCanvas.SetActive(false);
            endCanvas.SetActive(false);
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
            boardEnvironment.SetActive(false);
            setupCanvas.SetActive(false);
            lobbyCanvas.SetActive(true);
            lobbyEnvironment.SetActive(true);
            startMatchButton.interactable = false;
            AkSoundEngine.PostEvent("lobby_join", gameObject);
            AudioWwiseManager.instance.SetMusicGameState(GameState.Lobby);         
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
                lobbyGate.CloseGate();
            }
            else
            {
                lobbyGate.OpenGate();
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
            GetComponentInChildren<Text>().text = result;
        }

        private void HandleLobbyEnvironment()
        {
            boardEnvironment.SetActive(true);
            lobbyEnvironment.GetComponent<LobbyRocket>().StartBooster();
            SwipeManager.instance.canSwipe = false;
        }
    }
}
