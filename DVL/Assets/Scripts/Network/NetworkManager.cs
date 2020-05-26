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

    //TODO: Nicer GUI
    private void OnGUI()
    {
        GUILayout.Label("Player ID: " + GameManager.instance.viewOfPlayer);
        GUILayout.Label("Player Turn ID: " + GameManager.instance.currentTurnPlayer);

        if (GUILayout.Button("Server") && !NetworkServer.instance.isSetup) //Click to host match
        {
            isServer = true;
            NetworkServer.instance.SetupServer();
            NetworkClient.instance.Connect(new IPEndPoint(IPAddress.Loopback, 3999));
        }

        if (GUILayout.Button("Client")) //Click to attempt joining a hosted match
        {
            NetworkClient.instance.Connect(new IPEndPoint(IPAddress.Loopback, 3999));
        }

        if (isServer)
        {
            if (GUILayout.Button("Start Match")) //Click to start match
            {
                NetworkServer.instance.StartMatch();
            }
        }

        if (GameManager.instance.GetTurn())
        {
            if (GUILayout.Button("Next Turn"))
            {
                NetworkClient.instance.SendTurnChange();
            }
        }
    }
}