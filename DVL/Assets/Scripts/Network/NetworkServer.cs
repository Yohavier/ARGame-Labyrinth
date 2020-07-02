using Assets.Scripts.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class NetworkServer
{
    public static NetworkServer instance = new NetworkServer();
    public ServerGameState gameState = new ServerGameState();
    public bool isSetup = false;
    private List<ClientReference> clientList = new List<ClientReference>();
    private Socket serverSocket;
    private byte[] buffer = new byte[1024];

    //Initialize Server Socket, begin waiting for connections
    public void SetupServer()
    {
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        serverSocket.Bind(new IPEndPoint(IPAddress.Any, 3030));
        serverSocket.Listen(gameState.maxPlayerCount);
        serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        isSetup = true;
    }

    //Send client data after connecting
    private void SetupNewPlayer(ClientReference client)
    {
        PlayerIndex playerIndex = 3 - (PlayerIndex)gameState.playerCount;
        client.playerState.playerID = playerIndex;
        client.playerState.connected = true;
        client.playerState.ip = client.clientSocket.RemoteEndPoint.ToString().Split(':')[0];
        client.playerState.roleIndex = RoleIndex.Standart;

        Msg msg = BuildPlayerSetupMessage(playerIndex);
        UnicastMessage(msg.Serialize(), client.clientSocket);

        Msg msg2 = BuildPlayerStateListMessage();
        BroadcastMessage(msg2.Serialize());

        gameState.playerCount++;
    }

    //Send all clients match start data
    public void StartMatch()
    {
        for (int i = 0; i < clientList.Count; i++)
        {
            if (!clientList[i].playerState.isReady)
            {
                Debug.LogWarning("Not all Players ready");
                return;
            }
        }
        gameState.currentTurnPlayer = PlayerIndex.Player3;
        HandleChangeTurn();
        Msg boardMsg = BuildBoardSetupMessage();
        BroadcastMessage(boardMsg.Serialize());
    }

    private void SendReadyChange(ClientReference client)
    {
        Msg msg = new Msg(MsgOpcode.opReadyChange, 4 + sizeof(bool));
        msg.Write((int)client.playerState.playerID);
        msg.Write(client.playerState.isReady);
        BroadcastMessage(msg.Serialize());
    }

    private void HandleChangeTurn()
    {
        gameState.currentTurnIndex++;
        if (gameState.currentTurnIndex >= clientList.Count)
            gameState.currentTurnIndex = 0;

        NetworkPlayerState nextTurnPlayer = clientList[gameState.currentTurnIndex].playerState;
        PlayerIndex nextTurnPlayerIndex = nextTurnPlayer.playerID;
        Msg msg = BuildTurnChangeMessage(nextTurnPlayerIndex);
        gameState.currentTurnPlayer = nextTurnPlayerIndex;
        BroadcastMessage(msg.Serialize());
    }

    private void HandleReadyChange(Msg msg, Socket clientSocket)
    {
        bool value = Convert.ToBoolean(msg.ReadInt());
        ClientReference client = clientList.Find(x => x.clientSocket == clientSocket);
        client.playerState.isReady = value;
        SendReadyChange(client);
    }

    private void HandleRoleChange(Msg msg, Socket clientSocket)
    {
        PlayerIndex playerIndex = (PlayerIndex)msg.ReadInt();
        RoleIndex roleIndex = (RoleIndex)msg.ReadInt();
        ClientReference client = clientList.Find(x => x.clientSocket == clientSocket);
        if (client.playerState.playerID != playerIndex)
            Debug.LogError("Server: Received role change from wrong player ID " + client.playerState.playerID + " " + playerIndex);
        client.playerState.roleIndex = roleIndex;
        msg.readOffset = 0;
        BroadcastMessage(msg.Serialize());
    }

    //Accept connection, wait for receiving data
    private void AcceptCallback(IAsyncResult result)
    {
        Socket clientSocket = serverSocket.EndAccept(result);
        clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), clientSocket);
        ClientReference client = new ClientReference(clientSocket);
        clientList.Add(client);
        serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        Debug.Log("Client connected");
        SetupNewPlayer(client);
    }

    //Handle received data, continue receiving
    private void ReceiveCallback(IAsyncResult result)
    {
        Socket clientSocket = (Socket)result.AsyncState;
        int numBytes = clientSocket.EndReceive(result);
        byte[] data = new byte[numBytes];
        Buffer.BlockCopy(buffer, 0, data, 0, numBytes);
        HandleMessage(new Msg(data), clientSocket);
        clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), clientSocket);
    }

    private void HandleMessage(Msg msg, Socket clientSocket)
    {
        Debug.Log("Server: Received " + msg.opcode);
        if ((int)msg.opcode < 100) //Reflect received data to all other clients
        {
            BroadcastMessageExclusive(msg.Serialize(), clientSocket);
            return;
        }

        switch (msg.opcode)
        {
            case MsgOpcode.opTurnChange:
                HandleChangeTurn();
                break;
            case MsgOpcode.opReadyChange:
                HandleReadyChange(msg, clientSocket);
                break;
            case MsgOpcode.opRoleChange:
                HandleRoleChange(msg, clientSocket);
                break;
        }
    }

    private void SendCallback(IAsyncResult result)
    {
        Socket clientSocket = (Socket)result.AsyncState;
        clientSocket.EndSend(result);
    }

    //Send message to one client
    private void UnicastMessage(byte[] data, Socket clientSocket)
    {
        clientSocket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), clientSocket);
        Thread.Sleep(100);
    }

    //Send message to all clients
    private void BroadcastMessage(byte[] data)
    {
        for (int i = 0; i < clientList.Count; i++)
        {
            clientList[i].clientSocket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), clientList[i].clientSocket);
        }
        Thread.Sleep(100);
    }

    //Send message to all clients except one
    private void BroadcastMessageExclusive(byte[] data, Socket exclude)
    {
        for (int i = 0; i < clientList.Count; i++)
        {
            if (clientList[i].clientSocket == exclude)
                continue;

            clientList[i].clientSocket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), clientList[i].clientSocket);
        }
        Thread.Sleep(100);
    }

    private Msg BuildPlayerSetupMessage(PlayerIndex nextPlayer)
    {
        Msg msg = new Msg(MsgOpcode.opSetupPlayer, 4);
        msg.Write((int)nextPlayer);
        return msg;
    }

    private Msg BuildPlayerStateListMessage()
    {
        Msg msg = new Msg(MsgOpcode.opPlayerConnected, 256);
        msg.Write(clientList.Count);
        for (int i = 0; i < clientList.Count; i++)
        {
            msg.Write(clientList[i].playerState);
        }

        return msg;
    }

    private Msg BuildTurnChangeMessage(PlayerIndex nextTurnPlayer)
    {
        Msg msg = new Msg(MsgOpcode.opTurnChange, 4);
        msg.Write((int)nextTurnPlayer);
        return msg;
    }

    private Msg BuildBoardSetupMessage()
    {
        gameState.gridSeed.Clear();
        Msg msg = new Msg(MsgOpcode.opBoardSetup, 49 * sizeof(float));
        for (int i = 0; i < 49; i++)
        {
            float random = UnityEngine.Random.Range(0f, 1f);
            gameState.gridSeed.Add(random);
            msg.Write(random);
        }
        return msg;
    }
}
