using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
            storedPowerUp.GetComponent<PowerUpBase>().ReverseOnDrop(LocalGameManager.instance.activePlayer.GetComponent<Player>());
            Destroy(storedPowerUp.gameObject);
            GetComponent<Button>().image.sprite = null;
            GetComponent<Button>().interactable = false;
            storedPowerUp = null;
        }
    }
}
