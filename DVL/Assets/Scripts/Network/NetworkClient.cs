using Assets.Scripts.GameManagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class NetworkClient
{
    public static NetworkClient instance = new NetworkClient();
    private Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private byte[] buffer = new byte[1024];
    private int connectionAttempts = 0;
    public bool isSetup = false;

    public void Connect(string serverIP)
    {
        try
        {
            connectionAttempts++;
            clientSocket.Connect(new IPEndPoint(IPAddress.Parse(serverIP), 8080));
        }
        catch (SocketException)
        {
            UnityEngine.Debug.LogError("Connection failed, attempt " + connectionAttempts);
        }

        if (clientSocket.Connected)
        {
            isSetup = true;
            clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
        }
    }

    private void Send(Msg msg)
    {
        if (isSetup)
        {
            byte[] data = msg.Serialize();
            clientSocket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);
        }

        else
        {
            UnityEngine.Debug.LogWarning("Attempted to send without connection");
        }
    }

    private void SendCallback(IAsyncResult result)
    {
        clientSocket.EndSend(result);
    }

    private void ReceiveCallback(IAsyncResult result)
    {
        int numBytes = clientSocket.EndReceive(result);
        byte[] data = new byte[numBytes];
        Buffer.BlockCopy(buffer, 0, data, 0, numBytes);
        Msg msg = new Msg(data);
        HandleMessage(msg);
        clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
    }

    private void HandleMessage(Msg msg)
    {
        Debug.Log("Client: Received " + msg.opcode);
        switch (msg.opcode)
        {
            case MsgOpcode.opTileMove:
                HandleTileMove(msg);
                break;
            case MsgOpcode.opGridMove:
                HandleGridMove(msg);
                break;
            case MsgOpcode.opBoardSetup:
                HandleBoardSetup(msg);
                break;
            case MsgOpcode.opPlayerMove:
                HandlePlayerMove(msg);
                break;
            case MsgOpcode.opItemCollected:
                HandleItemCollected(msg);
                break;
            case MsgOpcode.opPlayerKilled:
                HandlePlayerKilled(msg);
                break;
            case MsgOpcode.opPlayerHealed:
                HandlePlayerHealed(msg);
                break;
            case MsgOpcode.opGeneratorRepaired:
                HandleGeneratorRepaired(msg);
                break;
            case MsgOpcode.opDoorHackUsed:
                HandleDoorHackUsed(msg);
                break;
            case MsgOpcode.opTurnChange:
                HandleTurnChange(msg);
                break;
            case MsgOpcode.opReadyChange:
                HandleReadyChange(msg);
                break;
            case MsgOpcode.opPlayerConnected:
                HandlePlayerConnected(msg);
                break;
            case MsgOpcode.opSetupPlayer:
                HandleSetupPlayer(msg);
                break;
        }
    }

    private void HandleTurnChange(Msg msg)
    {
        PlayerIndex currentTurnPlayer = (PlayerIndex)msg.ReadInt();
        //GUIManager.instance.nextTurnButton.interactable = currentTurnPlayer == LocalGameManager.instance.localPlayerIndex;
        LocalGameManager.instance.currentTurnPlayer = currentTurnPlayer;
    }
    private void HandleSetupPlayer(Msg msg)
    {
        PlayerIndex playerID = (PlayerIndex)msg.ReadInt();
        LocalGameManager.instance.localPlayerIndex = playerID;
    }
    private void HandleTileMove(Msg msg)
    {
        int index = msg.ReadInt();
        Tile tile = BoardGrid.instance.grid.Find(x => x.index == index);
        tile.transform.position = msg.ReadVector3();
    }

    private void HandleGridMove(Msg msg)
    {
        //UnityEngine.Debug.Log("Received Grid Move");
        int indexEntry = msg.ReadInt();
        int indexNew = msg.ReadInt();
        Tile tileEntry = BoardGrid.instance.grid.Find(x => x.index == indexEntry);
        Tile tileNew;
        if (indexNew == 0)
            tileNew = BoardGrid.instance.trackedTile;
        else
            tileNew = BoardGrid.instance.grid.Find(x => x.index == indexNew);
        BoardGrid.instance.InsertNewRoomPushing(tileEntry, tileNew);
        //tileNew.GetComponent<FindNearestGridSlot>().enabled = false;
    }

    private void HandleBoardSetup(Msg msg)
    {
        //InformationPanel.instance.enabled = true;
        BoardGrid.instance.seedList.Clear();
        BoardGrid.instance.seedCount = -1;
        for (int i = 0; i < 49; i++)
            BoardGrid.instance.seedList.Add(msg.ReadFloat());

        BoardGrid.instance.readyToSetup = true;//.SetUpGrid();
        GUIManager.instance.needsMenuUpdate = true;
    }

    private void HandlePlayerMove(Msg msg)
    {
        PlayerIndex playerIndex = (PlayerIndex)msg.ReadInt();
        int targetTileindex = msg.ReadInt();
        SelectARObjectWithFinger.instance.ManagePath(BoardGrid.instance.FindTileByIndex(targetTileindex), playerIndex);
    }

    private void HandleItemCollected(Msg msg)
    {
        PlayerIndex playerIndex = (PlayerIndex)msg.ReadInt();
        int targetTileindex = msg.ReadInt();

        var tile = BoardGrid.instance.FindTileByIndex(targetTileindex);
        var item = tile.GetComponentInChildren<Item>();
        if(tile && item)
            GameManager.instance.allPlayers[(int)playerIndex].GetComponent<CrewMember>().PickUpItem(item, tile);

        else
            Debug.LogError("No Item found");
    }

    private void HandlePlayerKilled(Msg msg)
    {
        PlayerIndex playerIndex = (PlayerIndex)msg.ReadInt();
        Player player = GameManager.instance.allPlayers[(int)playerIndex].GetComponent<Player>();
        GameManager.instance.KillPlayer(player);
    }

    private void HandlePlayerHealed(Msg msg)
    {
        PlayerIndex playerIndex = (PlayerIndex)msg.ReadInt();
        CrewMember player = GameManager.instance.allPlayers[(int)playerIndex].GetComponent<CrewMember>();
        player.GetHealedByOtherPlayer();
    }

    private void HandleGeneratorRepaired(Msg msg)
    {
        int repairSpeed = msg.ReadInt();
        Generator.instance.RepairGenerator(repairSpeed);
    }

    private void HandleDoorHackUsed(Msg msg)
    {
        int tileIndex = msg.ReadInt();
        Tile tile = BoardGrid.instance.FindTileByIndex(tileIndex);
        tile.ToggleDoors();
    }

    private void HandleReadyChange(Msg msg)
    {

    }

    private void HandlePlayerConnected(Msg msg)
    {
        int count = msg.ReadInt();
        if (count > 0)
        {
            string ip1 = msg.ReadString();
            GUIManager.instance.player1Text = "Player 1: " + ip1;
        }
        if (count > 1)
        {
            string ip2 = msg.ReadString();
            GUIManager.instance.player2Text = "Player 2: " + ip2;
        }
        if (count > 2)
        {
            string ip3 = msg.ReadString();
            GUIManager.instance.player3Text = "Player 3: " + ip3;

        }
        if (count > 3)
        {
            string ip4 = msg.ReadString();
            GUIManager.instance.player4Text = "Player 4: " + ip4;
        }
    }

    public void SendGridMove(Tile entryTile, Tile newRoom)
    {
        Msg msg = new Msg(MsgOpcode.opGridMove, 8);
        msg.Write(entryTile.index);
        msg.Write(newRoom.index);
        Send(msg);
    }

    public void SendTileMove(Tile activeTile)
    {
        Msg msg = new Msg(MsgOpcode.opTileMove, 16);
        msg.Write(activeTile.index);
        msg.Write(activeTile.transform.position);
        Send(msg);
    }

    public void SendTurnChange()
    {
        Msg msg = new Msg(MsgOpcode.opTurnChange, 0);
        Send(msg);
    }

    public void SendPlayerMove(Tile targetTile)
    {
        Msg msg = new Msg(MsgOpcode.opPlayerMove, 8);
        msg.Write((int)LocalGameManager.instance.localPlayerIndex);
        msg.Write(targetTile.index);
        Send(msg);
    }

    public void SendItemCollected(Tile itemTile)
    {
        Msg msg = new Msg(MsgOpcode.opItemCollected, 8);
        msg.Write((int)LocalGameManager.instance.localPlayerIndex);
        msg.Write(itemTile.index);
        Send(msg);
    }

    public void SendPlayerKilled(PlayerIndex playerIndex)
    {
        Msg msg = new Msg(MsgOpcode.opPlayerKilled, 4);
        msg.Write((int)playerIndex);
        Send(msg);
    }

    public void SendPlayerHealed(PlayerIndex playerIndex)
    {
        Msg msg = new Msg(MsgOpcode.opPlayerKilled, 4);
        msg.Write((int)playerIndex);
        Send(msg);
    }

    public void SendGeneratorRepaired(int repairSpeed)
    {
        Msg msg = new Msg(MsgOpcode.opGeneratorRepaired, 4);
        msg.Write(repairSpeed);
        Send(msg);
    }

    public void SendDoorHackUsed(Tile targetTile)
    {
        Msg msg = new Msg(MsgOpcode.opDoorHackUsed, 4);
        msg.Write(targetTile.index);
        Send(msg);
    }

    public void SendReadyChanged(bool value)
    {
        Msg msg = new Msg(MsgOpcode.opReadyChange, 4);
        msg.Write(Convert.ToInt32(value));
        Send(msg);
    }
}
