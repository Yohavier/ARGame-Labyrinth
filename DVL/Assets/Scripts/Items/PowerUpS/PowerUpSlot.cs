using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//TODO: Exchange Items if wanted
//Active PowerUps: Communicator and Scanner
//to drop, deactivate it

public class PowerUpSlot : MonoBehaviour
{
    public GameObject storedPowerUp;

    public void UsePowerUp()
    {
        if (storedPowerUp != null) 
            storedPowerUp.GetComponent<PowerUpBase>().OnUse(LocalGameManager.instance.activePlayer.GetComponent<Player>(), this);
    }

    public void DropEverythingInSlot()
    {
        if(storedPowerUp!= null)
        {
            Destroy(storedPowerUp.gameObject);
            GetComponent<Button>().image = null;
            GetComponent<Button>().interactable = false;
            storedPowerUp = null;
        }
    }
}
