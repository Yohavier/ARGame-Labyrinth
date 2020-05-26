using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MsgOpcode //ID for Message Header
{
    //<100 Server sends message to all other clients
    opTileMove = 0,
    opGridMove = 1,

    //100-200 Server handles message, updates and sends reply to all clients
    opTurnChange = 100,

    //>=200 Server sends reply to one client
    opSetupPlayer = 200,
}
