using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player: MonoBehaviour
{
    public playingPlayer player;
    public void SetUpPlayer(int count)
    {
        
        switch (count)
        {
            case 1:
                player = playingPlayer.Player1;
                break;
            case 2:
                player = playingPlayer.Player2;
                break;
            case 3:
                player = playingPlayer.Player3;
                break;
            case 4:
                player = playingPlayer.Enemy;
                break;
        }
        if(player != GameManager.GameManagerInstance.viewOfPlayer)
        {
            GetComponent<MeshRenderer>().enabled = false;
            Eventbroker.eventbroker.SignUpForFogOfWar(this.gameObject);
            GetComponent<SphereCollider>().radius = .5f;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (FogOfWar.fow.FogOfWarItems.Contains(other.gameObject))
            other.GetComponent<MeshRenderer>().enabled = true;
    }
    public void OnTriggerExit(Collider other)
    {
        if (FogOfWar.fow.FogOfWarItems.Contains(other.gameObject)   )
        {
            other.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
