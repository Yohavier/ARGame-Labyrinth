using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class NetworkManager : MonoBehaviour
{
    public bool isServer;
    public string serverIP = "127.0.0.1";

    private void Start()
    {
        serverIP = "192.168.178.35";
    }

    //TODO: Nicer GUI
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