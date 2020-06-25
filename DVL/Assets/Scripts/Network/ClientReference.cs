﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class ClientReference
{
    public Socket clientSocket;
    public NetworkPlayerState playerState;

    public ClientReference(Socket socket)
    {
        clientSocket = socket;
        playerState = new NetworkPlayerState();
    }
}
