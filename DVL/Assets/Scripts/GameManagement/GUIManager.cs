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
        public static GUIManager instance;
        public GameObject setupCanvas;
        public GameObject lobbyCanvas;
        public GameObject playerCanvas;
        public InputField hostIPInput;
        public Button hostButton;
        public Button joinButton;
        public Button startMatchButton;
        public Button nextTurnButton;
        public Button debugButton;
        public Toggle readyToggle;
        public Text player1Label;
        public Text player2Label;
        public Text player3Label;
        public Text player4Label;
        string serverIP = "192.168.194.169";
        public bool isServer = false;
        public bool isDebug = false;

        public string player1Text = "Player 1: ";
        public string player2Text = "Player 2: ";
        public string player3Text = "Player 3: ";
        public string player4Text = "Player 4: ";

        public bool needsMenuUpdate = false;
        public bool needsTurnUpdate = false;

        private void Awake()
        {
            Eventbroker.instance.onNotifyNextTurn += OnTurnChange;
            setupCanvas = GameObject.Find("SetupCanvas");
            lobbyCanvas = GameObject.Find("LobbyCanvas");
            playerCanvas = GameObject.Find("PlayerCanvas");
            hostIPInput = GameObject.Find("HostIPInput").GetComponent<InputField>();
            hostButton = GameObject.Find("HostButton").GetComponent<Button>();
            joinButton = GameObject.Find("JoinButton").GetComponent<Button>();
            startMatchButton = GameObject.Find("StartMatchButton").GetComponent<Button>();
            nextTurnButton = GameObject.Find("NextTurnButton").GetComponent<Button>();
            debugButton = GameObject.Find("DebugButton").GetComponent<Button>();
            readyToggle = GameObject.Find("ReadyToggle").GetComponent<Toggle>();
            player1Label = GameObject.Find("Player1Label").GetComponent<Text>();
            player2Label = GameObject.Find("Player2Label").GetComponent<Text>();
            player3Label = GameObject.Find("Player3Label").GetComponent<Text>();
            player4Label = GameObject.Find("Player4Label").GetComponent<Text>();

            hostButton.onClick.AddListener(() => OnHostButtonClick());
            joinButton.onClick.AddListener(() => OnJoinButtonClick());
            startMatchButton.onClick.AddListener(() => OnStartMatchButtonClick());
            nextTurnButton.onClick.AddListener(() => OnNextTurnButtonClick());
            debugButton.onClick.AddListener(() => OnDebugButtonClicked());
            readyToggle.onValueChanged.AddListener((value) => OnReadyToggleValueChanged(value));
            hostIPInput.text = serverIP;
            lobbyCanvas.SetActive(false);
            instance = this;
        }

        private void Start()
        {
            playerCanvas.SetActive(false);
        }

        private void Update()
        {
            if (needsMenuUpdate)
            {
                lobbyCanvas.SetActive(false);
                playerCanvas.SetActive(true);
                needsMenuUpdate = false;
            }

            player1Label.enabled = false;
            player1Label.text = player1Text;
            player2Label.text = player2Text;
            player3Label.text = player3Text;
            player4Label.text = player4Text;

            player1Label.enabled = true;
        }

        void OnTurnChange()
        {
            nextTurnButton.interactable = LocalGameManager.instance.GetTurn();
        }

        void OpenLobbyMenu()
        {
            setupCanvas.SetActive(false);
            lobbyCanvas.SetActive(true);
        }

        void OnDebugButtonClicked()
        {
            isDebug = !isDebug;
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
            startMatchButton.enabled = false;
            OpenLobbyMenu();
        }

        void OnReadyToggleValueChanged(bool value)
        {
            NetworkClient.instance.SendReadyChanged(value);
        }
    }
}
