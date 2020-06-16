using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
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
        serverSocket.Bind(new IPEndPoint(IPAddress.Any, 8080));
        serverSocket.Listen(gameState.maxPlayerCount);
        serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        isSetup = true;
    }

    //Send client data after connecting
    private void SetupNewPlayer(ClientReference client)
    {
        Msg msg = BuildPlayerSetupMessage((PlayerIndex)gameState.playerCount);
        UnicastMessage(msg.Serialize(), client.clientSocket);
        gameState.playerCount++;
    }

    //Send all clients match start data
    public void StartMatch()
    {
        HandleChangeTurn();
        Msg boardMsg = BuildBoardSetupMessage();
        BroadcastMessage(boardMsg.Serialize());
    }

    private void HandleChangeTurn()
    {
        PlayerIndex nextTurnPlayer = gameState.currentTurnPlayer + 1;
        if (nextTurnPlayer > PlayerIndex.Enemy || (int)nextTurnPlayer >= clientList.Count)
            nextTurnPlayer = PlayerIndex.Player1;
        Msg msg = BuildTurnChangeMessage(nextTurnPlayer);
        gameState.currentTurnPlayer = nextTurnPlayer;
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
        Debug.Log("Received Data " + numBytes);
        byte[] data = new byte[numBytes];
        Buffer.BlockCopy(buffer, 0, data, 0, numBytes);
        HandleMessage(new Msg(data), clientSocket);
        clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), clientSocket);
    }

    private void HandleMessage(Msg msg, Socket clientSocket)
    {
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
    }

    //Send message to all clients
    private void BroadcastMessage(byte[] data)
    {
        for (int i = 0; i < clientList.Count; i++)
        {
            clientList[i].clientSocket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), clientList[i].clientSocket);
        }
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
    }

    private Msg BuildPlayerSetupMessage(PlayerIndex nextPlayer)
    {
        Msg msg = new Msg(MsgOpcode.opSetupPlayer, 4);
        msg.Write((int)nextPlayer);
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
