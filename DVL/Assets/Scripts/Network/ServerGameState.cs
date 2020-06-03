using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Server data that should be reflected to all clients
public class ServerGameState
{
    public int playerCount;
    public int maxPlayerCount = 4;
    public List<float> gridSeed = new List<float>();
    public playingPlayer currentTurnPlayer = playingPlayer.Invalid;
}
