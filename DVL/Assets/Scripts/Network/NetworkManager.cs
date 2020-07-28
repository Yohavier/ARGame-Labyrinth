using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;
    public bool isServer;
    public bool isDebug;
    public string serverIP = "127.0.0.1";

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        serverIP = "192.168.194.169";
    }

    private void OnGUI()
    {
        GUILayout.Label("Player ID: " + LocalGameManager.instance.localPlayerIndex);
        GUILayout.Label("Player Turn ID: " + LocalGameManager.instance.currentTurnPlayer);

        serverIP = GUILayout.TextField(serverIP, 15);

        if (GUILayout.Button("Server") && !NetworkServer.instance.isSetup) //Click to host match
        {
            isServer = true;
            NetworkServer.instance.SetupServer();
            NetworkClient.instance.Connect(serverIP);
        }
        if (GUILayout.Button("Debug"))//click to toggle debug
        {
            isDebug = !isDebug;                 
        }
        if (GUILayout.Button("Client")) //Click to attempt joining a hosted match
        {
            NetworkClient.instance.Connect(serverIP);
        }

        if (isServer)
        {
            if (GUILayout.Button("Start Match")) //Click to start match
            {
                NetworkServer.instance.StartMatch();
            }
        }

        if (LocalGameManager.instance.GetTurn())
        {
            if (GUILayout.Button("Next Turn"))
            {
                NetworkClient.instance.SendTurnChange();
            }
        }
    }
}