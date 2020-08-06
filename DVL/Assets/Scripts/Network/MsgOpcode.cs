using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MsgOpcode //ID for Message Header
{
    //<100 Server sends message to all other clients
    opTileMove = 0,
    opGridMove = 1,
    opBoardSetup = 2,
    opPlayerMove = 3,
    opItemCollected = 4,
    opPlayerKilled = 5,
    opPlayerHealed = 6,
    opGeneratorRepaired = 7,
    opDoorHackUsed = 8,
    opPowerUpCollected = 9,
    opItemDropped = 10,
    opShutDownUsed = 11,

    //100-200 Server handles message, updates and sends reply to all clients
    opReadyChange = 101,
    opPlayerConnected = 102,
    opRoleChange = 103,
    opTurnChange = 104,
    opConnectionLost = 105,

    //>=200 Server sends reply to one client
    opSetupPlayer = 200,
    opPing = 201,
}
