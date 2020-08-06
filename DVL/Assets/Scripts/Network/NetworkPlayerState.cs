using Assets.Scripts.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class NetworkPlayerState
{
    public PlayerIndex playerID = PlayerIndex.Invalid;
    public RoleIndex roleIndex = RoleIndex.Invalid;
    public bool isReady;
    public bool connected;
    public string ip = "";
    public float lastPingTime = 0f;

    public NetworkPlayerState(PlayerIndex _playerID, RoleIndex _roleIndex, bool _isReady, bool _connected, string _ip)
    {
        playerID = _playerID;
        roleIndex = _roleIndex;
        isReady = _isReady;
        connected = _connected;
        ip = _ip;
    }

    public NetworkPlayerState()
    {
        playerID = PlayerIndex.Invalid;
        roleIndex = RoleIndex.Invalid;
        isReady = false;
        connected = false;
        ip = "";
    }
}

